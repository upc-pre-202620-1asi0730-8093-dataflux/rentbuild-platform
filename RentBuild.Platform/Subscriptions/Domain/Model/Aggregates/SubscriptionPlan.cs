using RentBuild.Platform.Shared.Domain.Model.Aggregates;
namespace RentBuild.Platform.Subscriptions.Domain.Model.Aggregates;

public sealed class SubscriptionPlan : AggregateRoot
{
    public string Name { get; private set; } = ""; public string Description { get; private set; } = ""; public decimal PriceAmount
    {
        get; private set;
    }
    public string PriceCurrency { get; private set; } = "PEN"; public string BillingCycle { get; private set; } = "MONTHLY"; public string Status { get; private set; } = "ACTIVE";
    private SubscriptionPlan()
    {
    }
    public SubscriptionPlan(int id, string name, decimal price)
    {
        Id = id;
        Name = name;
        Description = name + " rental management plan";
        PriceAmount = price;
    }
}
