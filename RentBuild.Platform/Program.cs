using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RentBuild.Platform.Shared.Application.Internal;
using RentBuild.Platform.Shared.Application.OutboundServices;
using RentBuild.Platform.Shared.Domain.Repositories;
using RentBuild.Platform.Shared.Infrastructure.Configuration;
using RentBuild.Platform.Shared.Infrastructure.Integration;
using RentBuild.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using RentBuild.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using RentBuild.Platform.Shared.Infrastructure.Pipeline;
using RentBuild.Platform.Shared.Infrastructure.Security;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentActor, CurrentActor>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<CommandExecutor>();
builder.Services.AddSingleton<CommandGate>();
var modules = new PlatformModuleCatalog();
builder.Services.AddSingleton(modules);
var data = Path.Combine(builder.Environment.ContentRootPath, "data");
Directory.CreateDirectory(data);
builder.Services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(data, "keys")));
builder.Services.AddDbContext<AppDbContext>((services, options) =>
{
    var config = services.GetRequiredService<IConfiguration>();
    var provider = config["Database:Provider"] ?? "Sqlite";
    var connection = config.GetConnectionString("DefaultConnection") ?? "Data Source=data/rentbuild.db";
    if (provider.Equals("MySql", StringComparison.OrdinalIgnoreCase)) options.UseMySQL(connection);
    else if (provider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase))
    {
        var sqlite = new Microsoft.Data.Sqlite.SqliteConnectionStringBuilder(connection);
        if (!Path.IsPathRooted(sqlite.DataSource) && sqlite.DataSource != ":memory:") sqlite.DataSource = Path.Combine(builder.Environment.ContentRootPath, sqlite.DataSource);
        options.UseSqlite(sqlite.ToString());
    }
    else throw new InvalidOperationException("Supported database providers: Sqlite, MySql");
});
var key = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(key))
{
    if (!builder.Environment.IsDevelopment() && !builder.Environment.IsEnvironment("Testing")) throw new InvalidOperationException("Configure Jwt__Key with at least 32 random bytes");
    var keyFile = Path.Combine(data, ".dev-jwt-key");
    if (!File.Exists(keyFile)) File.WriteAllText(keyFile, Convert.ToBase64String(RandomNumberGenerator.GetBytes(48)));
    key = File.ReadAllText(keyFile);
}
if (Encoding.UTF8.GetByteCount(key) < 32) throw new InvalidOperationException("Jwt__Key must contain at least 32 bytes");
builder.Configuration["Jwt:Key"] = key;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => options.TokenValidationParameters = new()
{
    ValidateIssuer = true, ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "RentBuild",
    ValidateAudience = true, ValidAudience = builder.Configuration["Jwt:Audience"] ?? "RentBuild.WebApp",
    ValidateIssuerSigningKey = true, IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
    ValidateLifetime = true, ClockSkew = TimeSpan.FromSeconds(15)
});
builder.Services.AddAuthorization();
var origins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ?? ["http://127.0.0.1:5173", "http://localhost:5173"];
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.WithOrigins(origins).AllowAnyMethod().AllowAnyHeader()));
builder.Services.AddPendingContextFacades();
foreach (var module in modules.Modules) module.Register(builder.Services, builder.Configuration);
var app = builder.Build();
app.UseExceptionHandler();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Testing")) { app.MapOpenApi(); app.MapScalarApiReference(); }
if (modules.Modules.Count > 0 && app.Configuration.GetValue<bool>("Database:InitializeSchema"))
{
    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreatedAsync();
}
app.MapGet("/health", () => Results.Ok(new { status = "ok", application = "RentBuild.Platform", modules = modules.Modules.Select(module => module.Name), stage = "initial-base" })).AllowAnonymous();
app.MapControllers();
app.Run();
public partial class Program;
