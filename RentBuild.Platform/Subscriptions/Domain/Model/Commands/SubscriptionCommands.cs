namespace RentBuild.Platform.Subscriptions.Domain.Model.Commands;

public sealed record SubscribeCommand(int UserId, int PlanId, bool AutoRenew);
public sealed record ChangePlanCommand(int Id, int PlanId);
public sealed record CancelSubscriptionCommand(int Id);
