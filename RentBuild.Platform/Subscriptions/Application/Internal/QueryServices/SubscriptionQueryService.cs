using RentBuild.Platform.Subscriptions.Application.QueryServices;
using RentBuild.Platform.Subscriptions.Domain.Model.Aggregates;
using RentBuild.Platform.Subscriptions.Domain.Repositories;
using RentBuild.Platform.Shared.Domain.Model.Errors;
namespace RentBuild.Platform.Subscriptions.Application.Internal.QueryServices;

public sealed class SubscriptionQueryService(ISubscriptionPlanRepository plans, IUserSubscriptionRepository subscriptions) : ISubscriptionQueryService
{
    public Task<List<SubscriptionPlan>> Plans() => plans.ListAsNoTrackingAsync(); public Task<List<UserSubscription>> Subscriptions() => subscriptions.ListAsNoTrackingAsync(); public async Task<UserSubscription> Find(int id) => Rule.Found(await subscriptions.FindByIdAsNoTrackingAsync(id), "Subscription");
}
