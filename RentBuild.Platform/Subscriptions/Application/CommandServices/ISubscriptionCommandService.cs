using RentBuild.Platform.Subscriptions.Domain.Model.Commands;
using RentBuild.Platform.Subscriptions.Domain.Model.Aggregates;
using RentBuild.Platform.Shared.Application.Model;
namespace RentBuild.Platform.Subscriptions.Application.CommandServices;

public interface ISubscriptionCommandService
{
    Task<Result<UserSubscription>> Handle(SubscribeCommand c); Task<Result<UserSubscription>> Handle(ChangePlanCommand c); Task<Result<UserSubscription>> Handle(CancelSubscriptionCommand c);
}
