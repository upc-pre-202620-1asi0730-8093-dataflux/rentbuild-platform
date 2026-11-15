using RentBuild.Platform.Maintenance.Application.Acl;
using RentBuild.Platform.Maintenance.Domain.Repositories;
using RentBuild.Platform.Maintenance.Domain.Model.Aggregates;
namespace RentBuild.Platform.Maintenance.Interfaces.Acl;

public sealed class MaintenanceContextFacade(IIncidentRepository incidents, IMaintenanceRecordRepository maintenance) : IMaintenanceContextFacade
{
    public async Task<bool> HasBlockingIncident(int equipmentId) => (await incidents.ListAsNoTrackingAsync()).Any(i => i.EquipmentId == equipmentId && i.BlocksRental && i.Status == "OPEN"); public Task ScheduleReturnWork(int equipmentId, int owner) => maintenance.AddAsync(new MaintenanceRecord(equipmentId, owner, DateTimeOffset.UtcNow, "Inspection after equipment return", "SCHEDULED"));
}
