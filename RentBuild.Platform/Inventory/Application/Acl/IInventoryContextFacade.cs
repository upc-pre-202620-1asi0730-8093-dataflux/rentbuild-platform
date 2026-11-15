namespace RentBuild.Platform.Inventory.Application.Acl;

public sealed record EquipmentSnapshot(int Id, int OwnerUserId, string Code, string Name, string Status);
public interface IInventoryContextFacade
{
    Task<EquipmentSnapshot> Find(int id); Task<bool> IsAvailable(int id, DateOnly start, DateOnly end); Task<int> Reserve(int id, DateOnly start, DateOnly end); Task MarkRented(int id); Task FinishRental(int id, int blockId, bool maintenance); Task Restrict(int id); Task Reactivate(int id);
}
