using RentBuild.Platform.Subscriptions.Domain.Model.Aggregates;
namespace RentBuild.Platform.Subscriptions.Application.QueryServices;

public interface ISubscriptionQueryService
{
    Task<List<SubscriptionPlan>> Plans(); Task<List<UserSubscription>> Subscriptions(); Task<UserSubscription> Find(int id);
}
