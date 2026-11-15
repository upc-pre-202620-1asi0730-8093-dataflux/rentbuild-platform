using RentBuild.Platform.Subscriptions.Domain.Model.Aggregates;
using RentBuild.Platform.Shared.Domain.Repositories;
namespace RentBuild.Platform.Subscriptions.Domain.Repositories;

public interface IUserSubscriptionRepository : IBaseRepository<UserSubscription>
{
}
