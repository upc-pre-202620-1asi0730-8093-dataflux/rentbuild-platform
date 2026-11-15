using RentBuild.Platform.Maintenance.Application.Internal.OutboundServices.Acl;
using RentBuild.Platform.Subscriptions.Application.Acl;
namespace RentBuild.Platform.Maintenance.Infrastructure.Acl.Subscriptions;

public sealed class MaintenanceSubscriptionsAclAdapter(ISubscriptionsContextFacade subscriptions) : IMaintenanceSubscriptions
{
    public Task<bool> HasGrowth(int owner) => subscriptions.HasActive(owner, 3);
}
