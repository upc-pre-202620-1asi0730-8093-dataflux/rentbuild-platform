using System.Security.Cryptography;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RentBuild.Platform.Iam.Infrastructure.Tokens.Jwt.Services;
using RentBuild.Platform.Iam.Domain.Repositories;
using RentBuild.Platform.Shared.Infrastructure.Configuration;
using RentBuild.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using RentBuild.Platform.Shared.Infrastructure.Pipeline;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddBoundedContexts();
builder.Services.AddOpenApi();
Directory.CreateDirectory(Path.Combine(builder.Environment.ContentRootPath, "data"));
builder.Services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "data", "keys")));
builder.Services.AddDbContext<AppDbContext>((services, options) =>
{
    var configuration = services.GetRequiredService<IConfiguration>();
    var provider = configuration["Database:Provider"] ?? "Sqlite";
    var connection = configuration.GetConnectionString("DefaultConnection") ?? "Data Source=data/rentbuild.db";
    if (provider.Equals("MySql", StringComparison.OrdinalIgnoreCase))
        options.UseMySQL(connection);
    else if (provider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase))
    {
        var sqlite = new Microsoft.Data.Sqlite.SqliteConnectionStringBuilder(connection);
        if (!Path.IsPathRooted(sqlite.DataSource) && sqlite.DataSource != ":memory:")
            sqlite.DataSource = Path.Combine(builder.Environment.ContentRootPath, sqlite.DataSource);
        options.UseSqlite(sqlite.ToString());
    }
    else
        throw new InvalidOperationException("Supported database providers: Sqlite, MySql");
});
var key = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(key))
{
    if (!builder.Environment.IsDevelopment() && !builder.Environment.IsEnvironment("Testing"))
        throw new InvalidOperationException("Configure Jwt__Key with at least 32 random characters");
    var path = Path.Combine(builder.Environment.ContentRootPath, "data", ".dev-jwt-key");
    if (!File.Exists(path))
        File.WriteAllText(path, Convert.ToBase64String(RandomNumberGenerator.GetBytes(48)));
    key = File.ReadAllText(path);
}
if (Encoding.UTF8.GetByteCount(key) < 32) throw new InvalidOperationException("Jwt key must be at least 32 bytes");
var jwt = new JwtSettings(key, builder.Configuration["Jwt:Issuer"] ?? "RentBuild", builder.Configuration["Jwt:Audience"] ?? "RentBuild.WebApp");
builder.Services.AddSingleton(jwt);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new()
    {
        ValidateIssuer = true,
        ValidIssuer = jwt.Issuer,
        ValidateAudience = true,
        ValidAudience = jwt.Audience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromSeconds(15)
    };
    options.Events = new JwtBearerEvents { OnTokenValidated = async context => { var id = int.TryParse(context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier), out var value) ? value : 0; var user = await context.HttpContext.RequestServices.GetRequiredService<IUserRepository>().FindByIdAsNoTrackingAsync(id); if (user is null || user.Status != "active" || context.Principal?.FindFirstValue("tokenVersion") != user.TokenVersion.ToString()) context.Fail("Account or token is no longer active"); } };
});
builder.Services.AddAuthorization();
var origins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ?? ["http://127.0.0.1:5173", "http://localhost:5173"];
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.WithOrigins(origins).AllowAnyMethod().AllowAnyHeader()));
var app = builder.Build();
app.UseExceptionHandler();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Testing")) { app.MapOpenApi(); app.MapScalarApiReference(); }
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (builder.Configuration.GetValue("Database:InitializeSchema", false))
        await db.Database.EnsureCreatedAsync();
    await DatabaseSeeder.Seed(db);
}
app.MapGet("/health", () => Results.Ok(new { status = "ok", application = "RentBuild.Platform" })).AllowAnonymous();
app.MapControllers();
app.Run();
public partial class Program;
