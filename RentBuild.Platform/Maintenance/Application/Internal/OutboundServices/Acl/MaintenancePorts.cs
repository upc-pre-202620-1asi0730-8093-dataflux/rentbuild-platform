namespace RentBuild.Platform.Maintenance.Application.Internal.OutboundServices.Acl;

public sealed record MaintenanceEquipment(int Id, int OwnerUserId, string Code, string Name, string Status);
public sealed record HistoricalRental(int Id, DateTimeOffset OccurredAt, string Description, string Status);
public interface IMaintenanceInventory
{
    Task<MaintenanceEquipment> Find(int id); Task Restrict(int id); Task Reactivate(int id);
}
public interface IMaintenanceSubscriptions
{
    Task<bool> HasGrowth(int owner);
}
public interface IMaintenanceRentalActivity
{
    Task<bool> HasActiveRental(int id); Task<List<HistoricalRental>> History(int id);
}
