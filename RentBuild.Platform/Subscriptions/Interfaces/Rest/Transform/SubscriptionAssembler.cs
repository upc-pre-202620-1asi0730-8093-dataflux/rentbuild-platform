using RentBuild.Platform.Subscriptions.Domain.Model.Aggregates;
using RentBuild.Platform.Subscriptions.Domain.Model.Commands;
using RentBuild.Platform.Subscriptions.Interfaces.Rest.Resources;
namespace RentBuild.Platform.Subscriptions.Interfaces.Rest.Transform;

public static class SubscriptionAssembler
{
    public static SubscriptionResource Resource(UserSubscription s) => new(s.Id, s.UserId, s.PlanId, s.StartDate, s.EndDate, s.Status, s.AutoRenew); public static PlanResource Resource(SubscriptionPlan p) => new(p.Id, p.Name, p.Description, p.PriceAmount, p.PriceCurrency, p.BillingCycle, p.Status); public static SubscribeCommand Command(SubscriptionResource r) => new(r.UserId, r.PlanId, r.AutoRenew);
}
