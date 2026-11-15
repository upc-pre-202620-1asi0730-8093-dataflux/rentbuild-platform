using RentBuild.Platform.Rentals.Application.Internal.OutboundServices.Acl;
using RentBuild.Platform.Inventory.Application.Acl;
namespace RentBuild.Platform.Rentals.Infrastructure.Acl.Inventory;

public sealed class RentalInventoryAclAdapter(IInventoryContextFacade inventory) : IRentalInventory
{
    public async Task<RentalEquipment> Find(int id)
    {
        var e = await inventory.Find(id);
        return new(e.Id, e.OwnerUserId, e.Status);
    }
    public Task<bool> Available(int id, DateOnly start, DateOnly end) => inventory.IsAvailable(id, start, end); public Task<int> Reserve(int id, DateOnly start, DateOnly end) => inventory.Reserve(id, start, end); public Task Deliver(int id) => inventory.MarkRented(id); public Task Return(int id, int blockId, bool maintenance) => inventory.FinishRental(id, blockId, maintenance);
}
