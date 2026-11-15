using RentBuild.Platform.Subscriptions.Application.CommandServices;
using RentBuild.Platform.Subscriptions.Application.Internal.OutboundServices.Acl;
using RentBuild.Platform.Subscriptions.Domain.Model.Commands;
using RentBuild.Platform.Subscriptions.Domain.Model.Aggregates;
using RentBuild.Platform.Subscriptions.Domain.Repositories;
using RentBuild.Platform.Shared.Application.Internal;
using RentBuild.Platform.Shared.Application.Model;
using RentBuild.Platform.Shared.Application.OutboundServices;
using RentBuild.Platform.Shared.Domain.Model.Errors;
namespace RentBuild.Platform.Subscriptions.Application.Internal.CommandServices;

public sealed class SubscriptionCommandService(IUserSubscriptionRepository subscriptions, ISubscriptionPlanRepository plans, ISubscriberIdentity identity, ICurrentActor actor, CommandExecutor executor) : ISubscriptionCommandService
{
    async Task ValidatePlan(int id)
    {
        var plan = Rule.Found(await plans.FindByIdAsync(id), "Subscription plan");
        Rule.Require(plan.Status == "ACTIVE", "Plan is inactive", ErrorKind.Conflict);
    }
    public Task<Result<UserSubscription>> Handle(SubscribeCommand c) => executor.ExecuteAsync(async () => { actor.RequireRole("rental_company"); Rule.Owner(actor.UserId, c.UserId); Rule.Require(await identity.IsActiveRentalCompany(c.UserId), "An active rental company is required", ErrorKind.Forbidden); await ValidatePlan(c.PlanId); Rule.Require(!(await subscriptions.ListAsync()).Any(s => s.UserId == c.UserId && s.Status == "ACTIVE" && s.EndDate > DateTimeOffset.UtcNow), "The company already has an active subscription", ErrorKind.Conflict); var s = new UserSubscription(c.UserId, c.PlanId, DateTimeOffset.UtcNow, c.AutoRenew); await subscriptions.AddAsync(s); return s; });
    public Task<Result<UserSubscription>> Handle(ChangePlanCommand c) => executor.ExecuteAsync(async () => { actor.RequireRole("rental_company"); var s = Rule.Found(await subscriptions.FindByIdAsync(c.Id), "Subscription"); Rule.Owner(actor.UserId, s.UserId); await ValidatePlan(c.PlanId); s.ChangePlan(c.PlanId, DateTimeOffset.UtcNow); return s; });
    public Task<Result<UserSubscription>> Handle(CancelSubscriptionCommand c) => executor.ExecuteAsync(async () => { actor.RequireRole("rental_company"); var s = Rule.Found(await subscriptions.FindByIdAsync(c.Id), "Subscription"); Rule.Owner(actor.UserId, s.UserId); s.Cancel(); return s; });
}
