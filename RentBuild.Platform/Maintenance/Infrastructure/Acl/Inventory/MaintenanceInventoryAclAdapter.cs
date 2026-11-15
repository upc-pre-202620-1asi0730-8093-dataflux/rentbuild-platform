using RentBuild.Platform.Maintenance.Application.Internal.OutboundServices.Acl;
using RentBuild.Platform.Inventory.Application.Acl;
namespace RentBuild.Platform.Maintenance.Infrastructure.Acl.Inventory;

public sealed class MaintenanceInventoryAclAdapter(IInventoryContextFacade inventory) : IMaintenanceInventory
{
    public async Task<MaintenanceEquipment> Find(int id)
    {
        var e = await inventory.Find(id);
        return new(e.Id, e.OwnerUserId, e.Code, e.Name, e.Status);
    }
    public Task Restrict(int id) => inventory.Restrict(id); public Task Reactivate(int id) => inventory.Reactivate(id);
}
