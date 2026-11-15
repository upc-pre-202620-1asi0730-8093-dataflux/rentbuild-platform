using RentBuild.Platform.Shared.Domain.Model.Aggregates;
using RentBuild.Platform.Shared.Domain.Model.Errors;
using RentBuild.Platform.Rentals.Domain.Model.ValueObjects;
namespace RentBuild.Platform.Rentals.Domain.Model.Aggregates;

public sealed class Rental : AggregateRoot
{
    public int RentalRequestId
    {
        get; private set;
    }
    public int AvailabilityBlockId
    {
        get; private set;
    }
    public int EquipmentId
    {
        get; private set;
    }
    public int ConstructionUserId
    {
        get; private set;
    }
    public int RentalCompanyUserId
    {
        get; private set;
    }
    public RentalPeriod Period { get; private set; } = null!; public RentalStatus Status { get; private set; } = RentalStatus.CONFIRMED;
    private Rental()
    {
    }
    public Rental(RentalRequest request, int blockId)
    {
        Rule.Require(request.Status == RentalRequestStatus.APPROVED, "Request is not approved", ErrorKind.Conflict);
        RentalRequestId = request.Id;
        EquipmentId = request.EquipmentId;
        ConstructionUserId = request.ConstructionUserId;
        RentalCompanyUserId = request.RentalCompanyUserId;
        Period = request.Period;
        AvailabilityBlockId = blockId;
        Raise("RentalConfirmed");
    }
    public void Deliver(DateTimeOffset at, DateTimeOffset now)
    {
        Rule.Require(Status == RentalStatus.CONFIRMED, "Only confirmed rentals can be delivered", ErrorKind.Conflict);
        Rule.Require(at >= Period.StartDate && at <= Period.EndDate && at <= now, "Delivery must be within the rental period and not in the future");
        Status = RentalStatus.ACTIVE;
        Raise("RentalDelivered");
    }
    public void Return(DateTimeOffset at, DateTimeOffset deliveredAt, DateTimeOffset now)
    {
        Rule.Require(Status == RentalStatus.ACTIVE, "Only active rentals can be returned", ErrorKind.Conflict);
        Rule.Require(at >= Period.StartDate && at >= deliveredAt && at <= now, "Return cannot precede delivery or be in the future");
        Status = RentalStatus.COMPLETED;
        Raise("RentalCompleted");
    }
}
