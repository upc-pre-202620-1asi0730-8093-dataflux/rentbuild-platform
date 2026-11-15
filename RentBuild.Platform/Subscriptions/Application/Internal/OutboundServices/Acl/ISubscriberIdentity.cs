namespace RentBuild.Platform.Subscriptions.Application.Internal.OutboundServices.Acl;

public interface ISubscriberIdentity
{
    Task<bool> IsActiveRentalCompany(int userId);
}
