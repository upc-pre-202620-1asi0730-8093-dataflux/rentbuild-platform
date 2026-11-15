using RentBuild.Platform.Maintenance.Domain.Model.Aggregates;
namespace RentBuild.Platform.Maintenance.Application.QueryServices;

public sealed record EquipmentHistoryEntry(int Id, string Kind, DateTimeOffset OccurredAt, string Description, string Status);
public sealed record EquipmentHistory(int EquipmentId, string EquipmentCode, string EquipmentName, List<EquipmentHistoryEntry> Entries);
public interface IMaintenanceQueryService
{
    Task<List<MaintenanceRecord>> Records(int? equipmentId); Task<MaintenanceRecord> Record(int id); Task<List<Incident>> Incidents(int? equipmentId); Task<Incident> Incident(int id); Task<List<MaintenanceActivity>> Activities(int id); Task<EquipmentHistory> History(int equipmentId);
}
