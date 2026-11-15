namespace RentBuild.Platform.Subscriptions.Interfaces.Rest.Resources;

public sealed record SubscriptionResource(int Id, int UserId, int PlanId, DateTimeOffset StartDate, DateTimeOffset EndDate, string Status = "ACTIVE", bool AutoRenew = true);
public sealed record PlanResource(int Id, string Name, string Description, decimal PriceAmount, string PriceCurrency, string BillingCycle, string Status);
