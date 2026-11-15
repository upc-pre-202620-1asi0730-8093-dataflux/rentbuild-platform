using RentBuild.Platform.Shared.Domain.Model.Aggregates;
using RentBuild.Platform.Shared.Domain.Model.Errors;
using RentBuild.Platform.Rentals.Domain.Model.ValueObjects;
namespace RentBuild.Platform.Rentals.Domain.Model.Aggregates;

public sealed class RentalRequest : AggregateRoot
{
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
    public RentalPeriod Period { get; private set; } = null!; public RentalRequestStatus Status { get; private set; } = RentalRequestStatus.PENDING; public DateTimeOffset CreatedAt
    {
        get; private set;
    }
    private RentalRequest()
    {
    }
    public RentalRequest(int equipment, int construction, int owner, RentalPeriod period)
    {
        Rule.Require(equipment > 0 && construction > 0 && owner > 0, "Invalid participant");
        Rule.Require(construction != owner, "A construction company cannot rent equipment from itself");
        period.RequireNotPast();
        EquipmentId = equipment;
        ConstructionUserId = construction;
        RentalCompanyUserId = owner;
        Period = period;
        CreatedAt = DateTimeOffset.UtcNow;
        Raise("RentalRequestSubmitted");
    }
    public void Decide(RentalRequestStatus target)
    {
        Rule.Require(Status == RentalRequestStatus.PENDING && target != RentalRequestStatus.PENDING, "Only pending requests can be decided", ErrorKind.Conflict);
        Status = target;
        Raise("RentalRequest" + target);
    }
    public void Reschedule(RentalPeriod period)
    {
        Rule.Require(Status == RentalRequestStatus.PENDING, "Only pending requests can be rescheduled", ErrorKind.Conflict);
        period.RequireNotPast();
        Period = period;
        Raise("RentalRequestRescheduled");
    }
}
