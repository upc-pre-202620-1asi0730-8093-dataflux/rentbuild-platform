using RentBuild.Platform.Shared.Domain.Model.Aggregates;
using RentBuild.Platform.Shared.Domain.Model.Errors;
namespace RentBuild.Platform.Subscriptions.Domain.Model.Aggregates;

public sealed class UserSubscription : AggregateRoot
{
    public int UserId
    {
        get; private set;
    }
    public int PlanId
    {
        get; private set;
    }
    public DateTimeOffset StartDate
    {
        get; private set;
    }
    public DateTimeOffset EndDate
    {
        get; private set;
    }
    public string Status { get; private set; } = "ACTIVE"; public bool AutoRenew
    {
        get; private set;
    }
    private UserSubscription()
    {
    }
    public UserSubscription(int userId, int planId, DateTimeOffset now, bool autoRenew)
    {
        UserId = userId;
        PlanId = planId;
        StartDate = now;
        EndDate = now.AddMonths(1);
        AutoRenew = autoRenew;
        Raise("SubscriptionActivated");
    }
    public bool IsActive(DateTimeOffset now) => Status == "ACTIVE" && StartDate <= now && now <= EndDate;
    public void ChangePlan(int planId, DateTimeOffset now)
    {
        Rule.Require(IsActive(now), "Only an active subscription can change plan", ErrorKind.Conflict);
        Rule.Require(planId != PlanId, "The selected plan is already active", ErrorKind.Conflict);
        PlanId = planId;
        Raise("SubscriptionPlanChanged");
    }
    public void Cancel()
    {
        Rule.Require(Status == "ACTIVE", "Subscription is not active", ErrorKind.Conflict);
        Status = "CANCELLED";
        AutoRenew = false;
        Raise("SubscriptionCancelled");
    }
}
