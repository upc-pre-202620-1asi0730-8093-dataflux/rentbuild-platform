using RentBuild.Platform.Maintenance.Application.QueryServices;
using RentBuild.Platform.Maintenance.Application.Internal.OutboundServices.Acl;
using RentBuild.Platform.Maintenance.Domain.Model.Aggregates;
using RentBuild.Platform.Maintenance.Domain.Repositories;
using RentBuild.Platform.Shared.Application.OutboundServices;
using RentBuild.Platform.Shared.Domain.Model.Errors;
namespace RentBuild.Platform.Maintenance.Application.Internal.QueryServices;

public sealed class MaintenanceQueryService(IMaintenanceRecordRepository records, IIncidentRepository incidents, IMaintenanceActivityRepository activities, IMaintenanceInventory inventory, IMaintenanceRentalActivity rentals, ICurrentActor actor) : IMaintenanceQueryService
{
    public async Task<List<MaintenanceRecord>> Records(int? equipmentId)
    {
        actor.RequireRole("rental_company");
        return (await records.ListAsNoTrackingAsync()).Where(m => m.OwnerUserId == actor.UserId && (equipmentId is null || m.EquipmentId == equipmentId)).ToList();
    }
    public async Task<MaintenanceRecord> Record(int id)
    {
        actor.RequireRole("rental_company");
        var m = Rule.Found(await records.FindByIdAsNoTrackingAsync(id), "Maintenance");
        Rule.Owner(actor.UserId, m.OwnerUserId);
        return m;
    }
    public async Task<List<Incident>> Incidents(int? equipmentId)
    {
        var all = await incidents.ListAsNoTrackingAsync();
        return all.Where(i => equipmentId is not null ? i.EquipmentId == equipmentId : i.OwnerUserId == actor.UserId).ToList();
    }
    public async Task<Incident> Incident(int id)
    {
        var incident = Rule.Found(await incidents.FindByIdAsNoTrackingAsync(id), "Incident");
        Rule.Owner(actor.UserId, incident.OwnerUserId);
        return incident;
    }
    public async Task<List<MaintenanceActivity>> Activities(int id)
    {
        await Record(id);
        return (await activities.ListAsNoTrackingAsync()).Where(a => a.MaintenanceId == id).OrderBy(a => a.OccurredAt).ToList();
    }
    public async Task<EquipmentHistory> History(int equipmentId)
    {
        actor.RequireRole("rental_company");
        var e = await inventory.Find(equipmentId);
        Rule.Owner(actor.UserId, e.OwnerUserId);
        var items = (await rentals.History(equipmentId)).Select(r => new EquipmentHistoryEntry(r.Id, "RENTAL", r.OccurredAt, r.Description, r.Status)).ToList();
        items.AddRange((await records.ListAsNoTrackingAsync()).Where(m => m.EquipmentId == equipmentId).Select(m => new EquipmentHistoryEntry(m.Id, "MAINTENANCE", m.PerformedAt, m.Type, m.Status)));
        items.AddRange((await incidents.ListAsNoTrackingAsync()).Where(i => i.EquipmentId == equipmentId).Select(i => new EquipmentHistoryEntry(i.Id, "INCIDENT", i.ReportedAt, i.Description, i.Status)));
        return new(e.Id, e.Code, e.Name, items.OrderByDescending(i => i.OccurredAt).ToList());
    }
}
