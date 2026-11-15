using RentBuild.Platform.Inventory.Application.Internal.OutboundServices.Acl;
using RentBuild.Platform.Subscriptions.Application.Acl;
namespace RentBuild.Platform.Inventory.Infrastructure.Acl.Subscriptions;

public sealed class InventoryAccessAclAdapter(ISubscriptionsContextFacade subscriptions) : IInventoryAccess
{
    public Task<bool> HasSubscription(int userId) => subscriptions.HasActive(userId);
}
