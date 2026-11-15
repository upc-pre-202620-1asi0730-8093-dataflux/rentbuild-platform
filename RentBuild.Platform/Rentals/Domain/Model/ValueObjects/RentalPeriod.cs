using RentBuild.Platform.Shared.Domain.Model.Errors;
using RentBuild.Platform.Shared.Domain.Model.ValueObjects;
namespace RentBuild.Platform.Rentals.Domain.Model.ValueObjects;

public sealed record RentalPeriod
{
    private RentalPeriod()
    {
    }
    public DateTimeOffset StartDate
    {
        get;
    }
    public DateTimeOffset EndDate
    {
        get;
    }
    public RentalPeriod(DateTimeOffset start, DateTimeOffset end)
    {
        Rule.Require(start != default && end != default && end >= start, "Invalid rental period");
        StartDate = BusinessCalendar.Start(BusinessCalendar.Day(start));
        EndDate = BusinessCalendar.End(BusinessCalendar.Day(end));
    }
    public bool Overlaps(RentalPeriod other) => StartDate <= other.EndDate && other.StartDate <= EndDate;
    public void RequireNotPast() => Rule.Require(BusinessCalendar.Day(StartDate) >= BusinessCalendar.Today, "Rental start date cannot be in the past");
}
public enum RentalRequestStatus
{
    PENDING, APPROVED, REJECTED
}
public enum RentalStatus
{
    CONFIRMED, ACTIVE, COMPLETED
}
