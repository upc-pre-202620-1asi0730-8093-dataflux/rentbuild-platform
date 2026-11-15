using RentBuild.Platform.Maintenance.Domain.Model.Aggregates;
using RentBuild.Platform.Maintenance.Domain.Model.Commands;
using RentBuild.Platform.Maintenance.Interfaces.Rest.Resources;
namespace RentBuild.Platform.Maintenance.Interfaces.Rest.Transform;

public static class MaintenanceAssembler
{
    public static MaintenanceResource Resource(MaintenanceRecord m) => new(m.Id, m.EquipmentId, m.PerformedAt, m.Type, m.Status, m.Technician, m.AcknowledgedAt); public static IncidentResource Resource(Incident i) => new(i.Id, i.EquipmentId, i.Description, i.ReportedAt, i.BlocksRental, i.Status, i.ResolvedAt); public static RegisterMaintenanceCommand Command(MaintenanceResource r) => new(r.EquipmentId, r.PerformedAt, r.Type, r.Status);
}
