using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RentBuild.Platform.Iam.Application.Internal.OutboundServices;
namespace RentBuild.Platform.Tests.Integration;

public sealed class TestMailSender : IRecoveryMailSender
{
    public bool IsConfigured => true; public readonly Dictionary<string, string> Tokens = new(); public Task Send(string email, string token)
    {
        Tokens[email] = token;
        return Task.CompletedTask;
    }
}
public sealed class ApiFactory : WebApplicationFactory<Program>
{
    private readonly string database = Path.Combine(Path.GetTempPath(), "rentbuild-test-" + Guid.NewGuid() + ".db");
    public TestMailSender Mail { get; } = new();
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, config) => config.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:DefaultConnection"] = "Data Source=" + database,
            ["Database:Provider"] = "Sqlite",
            ["Database:InitializeSchema"] = "true",
            ["Jwt:Key"] = "RentBuild-Integration-Test-Key-Only-0123456789-0123456789"
        }));
        builder.ConfigureServices(services => { services.RemoveAll<IRecoveryMailSender>(); services.AddSingleton<IRecoveryMailSender>(Mail); });
    }
}
