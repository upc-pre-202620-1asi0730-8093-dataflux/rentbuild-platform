using RentBuild.Platform.Rentals.Application.Internal.OutboundServices.Acl;
using RentBuild.Platform.Subscriptions.Application.Acl;
namespace RentBuild.Platform.Rentals.Infrastructure.Acl.Subscriptions;

public sealed class RentalSubscriptionsAclAdapter(ISubscriptionsContextFacade subscriptions) : IRentalSubscriptions
{
    public Task<bool> Active(int owner, int minimumPlan = 1) => subscriptions.HasActive(owner, minimumPlan);
}
