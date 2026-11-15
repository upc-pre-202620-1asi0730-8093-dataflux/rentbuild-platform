using Microsoft.EntityFrameworkCore;
using RentBuild.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Xunit;
namespace RentBuild.Platform.Tests.Integration;

public sealed class MySqlModelTests
{
    [Fact]
    public void MySqlProviderGeneratesRelationalSchemaWithoutConnectingToAServer()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>().UseMySQL("Server=localhost;Database=rentbuild;User=unused;Password=unused").Options;
        using var db = new AppDbContext(options);
        var script = db.Database.GenerateCreateScript();
        Assert.Contains("iam_users", script);
        Assert.Contains("inventory_availability_blocks", script);
        Assert.Contains("rentals_rentals", script);
        Assert.Contains("AUTO_INCREMENT", script, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("maintenance_records", script);
    }
}
