using Microsoft.Extensions.DependencyInjection;
using RentBuild.Platform.Inventory.Application.Acl;
using RentBuild.Platform.Maintenance.Application.CommandServices;
using RentBuild.Platform.Maintenance.Application.QueryServices;
using RentBuild.Platform.Maintenance.Domain.Model.Aggregates;
using RentBuild.Platform.Shared.Domain.Model.Errors;
using Xunit;

namespace RentBuild.Platform.Tests;

public sealed class MaintenanceFeatureTests(InitialFactory factory) : IClassFixture<InitialFactory>
{
    [Fact]
    public void ScheduledMaintenanceCannotAdvanceBeforeItsDay()
    {
        var record = new MaintenanceRecord(1, 1, DateTimeOffset.UtcNow.AddDays(2), "Inspection", "SCHEDULED");
        Assert.Throws<DomainRuleException>(() => record.Advance("COMPLETED", "Technician", false, DateTimeOffset.UtcNow));
    }

    [Fact]
    public void CompletedMaintenanceCannotBeReopened()
    {
        var record = new MaintenanceRecord(1, 1, DateTimeOffset.UtcNow, "Inspection", "SCHEDULED");
        record.Advance("COMPLETED", "Technician", true, DateTimeOffset.UtcNow);
        Assert.Throws<DomainRuleException>(() => record.Advance("IN_PROGRESS", null, false, DateTimeOffset.UtcNow));
    }

    [Fact]
    public void ResolutionPreservesTheBlockingIncidentHistory()
    {
        var incident = new Incident(1, 1, "Hydraulic fault", true);
        incident.Resolve();
        Assert.Equal("RESOLVED", incident.Status);
        Assert.True(incident.BlocksRental);
    }

    [Fact]
    public void MaintenanceServicesResolveUsingTheCommonBase()
    {
        using var scope = factory.Services.CreateScope();
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IMaintenanceCommandService>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IMaintenanceQueryService>());
    }

    [Fact]
    public async Task MissingInventoryIsExplicitInsteadOfReturningFakeEquipment()
    {
        using var scope = factory.Services.CreateScope();
        var inventory = scope.ServiceProvider.GetRequiredService<IInventoryContextFacade>();
        var error = await Assert.ThrowsAsync<DomainRuleException>(async () => await inventory.Find(1));
        Assert.Equal("CONTEXT_PENDING", error.Code);
        Assert.Equal(ErrorKind.Unavailable, error.Kind);
    }
}
