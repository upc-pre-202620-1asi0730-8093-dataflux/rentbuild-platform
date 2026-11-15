namespace RentBuild.Platform.Inventory.Application.Internal.OutboundServices.Acl;

public interface IInventoryAccess
{
    Task<bool> HasSubscription(int userId);
}
