namespace RentBuild.Platform.Rentals.Application.Internal.OutboundServices.Acl;

public sealed record RentalEquipment(int Id, int OwnerUserId, string Status);
public interface IRentalInventory
{
    Task<RentalEquipment> Find(int id); Task<bool> Available(int id, DateOnly start, DateOnly end); Task<int> Reserve(int id, DateOnly start, DateOnly end); Task Deliver(int id); Task Return(int id, int blockId, bool maintenance);
}
public interface IRentalSubscriptions
{
    Task<bool> Active(int owner, int minimumPlan = 1);
}
public interface IRentalMaintenance
{
    Task<bool> BlockingIncident(int equipmentId); Task ScheduleReturnWork(int equipmentId, int owner);
}
