using RentBuild.Platform.Shared.Domain.Model.Aggregates;
namespace RentBuild.Platform.Rentals.Domain.Model.Aggregates;

public sealed class Delivery : AggregateRoot
{
    public int RentalId
    {
        get; private set;
    }
    public DateTimeOffset DeliveredAt
    {
        get; private set;
    }
    public string Notes { get; private set; } = "";
    private Delivery()
    {
    }
    public Delivery(int rentalId, DateTimeOffset at, string notes)
    {
        RentalId = rentalId;
        DeliveredAt = at;
        Notes = notes.Trim();
    }
}
