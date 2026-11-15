using RentBuild.Platform.Maintenance.Application.CommandServices;
using RentBuild.Platform.Maintenance.Application.Internal.OutboundServices.Acl;
using RentBuild.Platform.Maintenance.Domain.Model.Commands;
using RentBuild.Platform.Maintenance.Domain.Model.Aggregates;
using RentBuild.Platform.Maintenance.Domain.Repositories;
using RentBuild.Platform.Shared.Application.Internal;
using RentBuild.Platform.Shared.Application.Model;
using RentBuild.Platform.Shared.Application.OutboundServices;
using RentBuild.Platform.Shared.Domain.Model.Errors;
using RentBuild.Platform.Shared.Domain.Repositories;
namespace RentBuild.Platform.Maintenance.Application.Internal.CommandServices;

public sealed class MaintenanceCommandService(IMaintenanceRecordRepository records, IIncidentRepository incidents, IMaintenanceActivityRepository activities, IMaintenanceInventory inventory, IMaintenanceSubscriptions subscriptions, IMaintenanceRentalActivity rentals, ICurrentActor actor, CommandExecutor executor, IUnitOfWork work) : IMaintenanceCommandService
{
    async Task Eligible()
    {
        actor.RequireRole("rental_company");
        Rule.Require(await subscriptions.HasGrowth(actor.UserId), "Growth plan required", ErrorKind.Forbidden);
    }
    async Task<MaintenanceEquipment> Owned(int id)
    {
        var e = await inventory.Find(id);
        Rule.Owner(actor.UserId, e.OwnerUserId);
        return e;
    }
    public Task<Result<MaintenanceRecord>> Handle(RegisterMaintenanceCommand c) => executor.ExecuteAsync(async () => { await Eligible(); await Owned(c.EquipmentId); Rule.Require(!await rentals.HasActiveRental(c.EquipmentId), "Cannot maintain equipment during an active rental", ErrorKind.Conflict); var record = new MaintenanceRecord(c.EquipmentId, actor.UserId, c.PerformedAt, c.Type, c.Status); await records.AddAsync(record); await work.FlushAsync(); await activities.AddAsync(new(record.Id, record.Status, record.Technician, "Maintenance registered")); await inventory.Restrict(c.EquipmentId); return record; });
    public Task<Result<MaintenanceRecord>> Handle(AdvanceMaintenanceCommand c) => executor.ExecuteAsync(async () => { await Eligible(); var record = Rule.Found(await records.FindByIdAsync(c.Id), "Maintenance"); Rule.Owner(actor.UserId, record.OwnerUserId); await Owned(record.EquipmentId); Rule.Require(!await rentals.HasActiveRental(record.EquipmentId), "Active rental prevents maintenance", ErrorKind.Conflict); Rule.Require(c.Status is not null || !string.IsNullOrWhiteSpace(c.Technician) || c.Acknowledge || !string.IsNullOrWhiteSpace(c.Notes), "Empty maintenance update"); record.Advance(c.Status, c.Technician, c.Acknowledge, DateTimeOffset.UtcNow); Rule.Require((c.Notes ?? "").Length <= 3000, "Notes too long"); await activities.AddAsync(new(record.Id, record.Status, record.Technician, c.Notes ?? "")); return record; });
    public Task<Result<Incident>> Handle(ReportIncidentCommand c) => executor.ExecuteAsync(async () => { await Eligible(); await Owned(c.EquipmentId); var incident = new Incident(c.EquipmentId, actor.UserId, c.Description, c.BlocksRental); await incidents.AddAsync(incident); if (c.BlocksRental) await inventory.Restrict(c.EquipmentId); return incident; });
    public Task<Result<Incident>> Handle(PatchIncidentCommand c) => executor.ExecuteAsync(async () => { await Eligible(); var incident = Rule.Found(await incidents.FindByIdAsync(c.Id), "Incident"); Rule.Owner(actor.UserId, incident.OwnerUserId); await Owned(incident.EquipmentId); Rule.Require(c.Status is null or "RESOLVED", "Only RESOLVED may be requested"); Rule.Require(c.BlocksRental is null or true, "Rental restrictions cannot be cleared by patch"); Rule.Require(c.Status is not null || c.BlocksRental == true, "Empty incident update"); if (c.BlocksRental == true) { incident.Restrict(); await inventory.Restrict(incident.EquipmentId); } if (c.Status == "RESOLVED") incident.Resolve(); return incident; });
    public Task<Result<bool>> Handle(ReactivateEquipmentCommand c) => executor.ExecuteAsync(async () => { await Eligible(); var e = await Owned(c.EquipmentId); Rule.Require(c.InspectionConfirmed, "Confirm technical inspection before reactivation"); Rule.Require(e.Status == "MAINTENANCE", "Only equipment in maintenance can reactivate", ErrorKind.Conflict); Rule.Require(!(await incidents.ListAsync()).Any(i => i.EquipmentId == e.Id && i.Status == "OPEN" && i.BlocksRental), "Unresolved blocking incidents prevent reactivation", ErrorKind.Conflict); Rule.Require(!(await records.ListAsync()).Any(m => m.EquipmentId == e.Id && m.Status != "COMPLETED"), "Pending maintenance prevents reactivation", ErrorKind.Conflict); Rule.Require(!await rentals.HasActiveRental(e.Id), "Active rental prevents reactivation", ErrorKind.Conflict); await inventory.Reactivate(e.Id); return true; });
}
