using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
namespace RentBuild.Platform.Tests;
public sealed class InitialFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder) => builder.UseEnvironment("Testing");
}
public sealed class StartupTests(InitialFactory factory) : IClassFixture<InitialFactory>
{
    [Fact]
    public async Task BaseStartsWithoutImplementationsOfOtherContexts()
    {
        using var client = factory.CreateClient();
        using var response = await client.GetAsync("/health");
        response.EnsureSuccessStatusCode();
        var json = JsonNode.Parse(await response.Content.ReadAsStringAsync())!;
        Assert.Equal("ok", json["status"]!.GetValue<string>());
    }
    [Fact]
    public async Task OpenApiDocumentIsAvailable()
    {
        using var client = factory.CreateClient();
        using var response = await client.GetAsync("/openapi/v1.json");
        response.EnsureSuccessStatusCode();
        Assert.Contains("/health", await response.Content.ReadAsStringAsync());
    }
}
