using Microsoft.EntityFrameworkCore;
using RentBuild.Platform.Maintenance.Application.Acl;
using RentBuild.Platform.Maintenance.Application.CommandServices;
using RentBuild.Platform.Maintenance.Application.Internal.CommandServices;
using RentBuild.Platform.Maintenance.Application.Internal.OutboundServices.Acl;
using RentBuild.Platform.Maintenance.Application.Internal.QueryServices;
using RentBuild.Platform.Maintenance.Application.QueryServices;
using RentBuild.Platform.Maintenance.Domain.Model.Aggregates;
using RentBuild.Platform.Maintenance.Domain.Repositories;
using RentBuild.Platform.Maintenance.Infrastructure.Acl.Inventory;
using RentBuild.Platform.Maintenance.Infrastructure.Acl.Rentals;
using RentBuild.Platform.Maintenance.Infrastructure.Acl.Subscriptions;
using RentBuild.Platform.Maintenance.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using RentBuild.Platform.Maintenance.Interfaces.Acl;
using RentBuild.Platform.Shared.Infrastructure.Configuration;

namespace RentBuild.Platform.Maintenance.Infrastructure.Configuration;

/// <summary>Composition owned by member 4; other modules register their real facades separately.</summary>
public sealed class MaintenanceModule : IPlatformModule
{
    public string Name => "Maintenance";

    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IMaintenanceRecordRepository, MaintenanceRecordRepository>();
        services.AddScoped<IIncidentRepository, IncidentRepository>();
        services.AddScoped<IMaintenanceActivityRepository, MaintenanceActivityRepository>();
        services.AddScoped<IMaintenanceCommandService, MaintenanceCommandService>();
        services.AddScoped<IMaintenanceQueryService, MaintenanceQueryService>();
        services.AddScoped<IMaintenanceInventory, MaintenanceInventoryAclAdapter>();
        services.AddScoped<IMaintenanceSubscriptions, MaintenanceSubscriptionsAclAdapter>();
        services.AddScoped<IMaintenanceRentalActivity, MaintenanceRentalActivityAclAdapter>();
        services.AddScoped<IMaintenanceContextFacade, MaintenanceContextFacade>();
    }

    public void ConfigureModel(ModelBuilder model)
    {
        model.Entity<MaintenanceRecord>().ToTable("maintenance_records");
        model.Entity<Incident>().ToTable("maintenance_incidents");
        model.Entity<MaintenanceActivity>().ToTable("maintenance_activities");
    }
}
