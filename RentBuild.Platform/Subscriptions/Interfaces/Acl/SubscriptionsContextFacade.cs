using RentBuild.Platform.Subscriptions.Application.Acl;
using RentBuild.Platform.Subscriptions.Domain.Repositories;
namespace RentBuild.Platform.Subscriptions.Interfaces.Acl;

public sealed class SubscriptionsContextFacade(IUserSubscriptionRepository subscriptions) : ISubscriptionsContextFacade
{
    public async Task<bool> HasActive(int userId, int minimumPlan = 1) => (await subscriptions.ListAsNoTrackingAsync()).Any(s => s.UserId == userId && s.PlanId >= minimumPlan && s.IsActive(DateTimeOffset.UtcNow));
}
