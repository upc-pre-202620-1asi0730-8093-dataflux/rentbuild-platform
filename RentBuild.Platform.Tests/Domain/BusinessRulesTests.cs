using Xunit;
using RentBuild.Platform.Shared.Domain.Model.Errors;
using RentBuild.Platform.Shared.Domain.Model.ValueObjects;
using RentBuild.Platform.Inventory.Domain.Model.Aggregates;
using RentBuild.Platform.Inventory.Domain.Model.ValueObjects;
using RentBuild.Platform.Rentals.Domain.Model.Aggregates;
using RentBuild.Platform.Rentals.Domain.Model.ValueObjects;
using RentBuild.Platform.Maintenance.Domain.Model.Aggregates;
using RentBuild.Platform.Subscriptions.Domain.Model.Aggregates;
using RentBuild.Platform.Iam.Domain.Model.ValueObjects;
namespace RentBuild.Platform.Tests.Domain;

public sealed class BusinessRulesTests
{
    [Theory][InlineData(0, 10)][InlineData(-1, 20)][InlineData(20, 0)] public void PricesMustBePositive(decimal daily, decimal weekly) => Assert.Throws<DomainRuleException>(() => new RentalRate(daily, weekly));
    [Theory][InlineData("2026-02-30")][InlineData("invalid")][InlineData("12/11/2026")] public void CalendarRejectsInvalidDays(string day) => Assert.Throws<DomainRuleException>(() => BusinessCalendar.Parse(day));
    [Fact] public void CalendarUsesLima() => Assert.Equal(new DateOnly(2026, 11, 12), BusinessCalendar.Day(DateTimeOffset.Parse("2026-11-13T02:00:00Z")));
    [Fact]
    public void PeriodIncludesEntireFinalLimaDay()
    {
        var p = new RentalPeriod(DateTimeOffset.Parse("2030-11-12T05:00:00Z"), DateTimeOffset.Parse("2030-11-14T04:59:59.999Z"));
        Assert.Equal(DateTimeOffset.Parse("2030-11-14T04:59:59.999Z"), p.EndDate);
    }
    [Fact]
    public void InclusiveReservationsRejectTouchingFinalDay()
    {
        var e = new Equipment(1, "E1", "Machine", "Description", 1, "Lima", new(10, 50));
        e.Reserve(new(2030, 1, 1), new(2030, 1, 5));
        Assert.False(e.IsAvailable(new(2030, 1, 5), new(2030, 1, 7)));
        Assert.True(e.IsAvailable(new(2030, 1, 6), new(2030, 1, 7)));
    }
    [Fact]
    public void OperationalRestrictionsBlockRental()
    {
        var e = new Equipment(1, "E1", "Machine", "Description", 1, "Lima", new(10, 50));
        e.Restrict();
        Assert.False(e.IsAvailable(BusinessCalendar.Today, BusinessCalendar.Today));
        Assert.Throws<DomainRuleException>(() => e.MarkRented());
        e.Reactivate();
        Assert.True(e.IsAvailable(BusinessCalendar.Today, BusinessCalendar.Today));
    }
    [Fact]
    public void CancellationRevokesEntitlementImmediately()
    {
        var now = DateTimeOffset.UtcNow;
        var s = new UserSubscription(1, 3, now, true);
        s.Cancel();
        Assert.False(s.IsActive(now));
        Assert.False(s.AutoRenew);
    }
    [Fact]
    public void PlanChangeKeepsPaidPeriod()
    {
        var now = DateTimeOffset.UtcNow;
        var s = new UserSubscription(1, 2, now, true);
        var end = s.EndDate;
        s.ChangePlan(3, now);
        Assert.Equal(end, s.EndDate);
    }
    [Theory][InlineData("invalid")][InlineData("a@")][InlineData("")] public void EmailRejectsMalformedInput(string email) => Assert.Throws<DomainRuleException>(() => new EmailAddress(email));
    [Fact]
    public void RequestCanOnlyBeDecidedOnce()
    {
        var now = DateTimeOffset.UtcNow;
        var r = new RentalRequest(1, 2, 3, new(now, now.AddDays(1)));
        r.Decide(RentalRequestStatus.APPROVED);
        Assert.Throws<DomainRuleException>(() => r.Decide(RentalRequestStatus.REJECTED));
    }
    [Fact]
    public void CompletedMaintenanceDoesNotReopen()
    {
        var m = new MaintenanceRecord(1, 1, DateTimeOffset.UtcNow, "Inspection", "SCHEDULED");
        m.Advance("COMPLETED", "Technician", true, DateTimeOffset.UtcNow);
        Assert.Throws<DomainRuleException>(() => m.Advance("IN_PROGRESS", null, false, DateTimeOffset.UtcNow));
    }
    [Fact]
    public void MaintenanceCannotAdvanceBeforeScheduledDay()
    {
        var m = new MaintenanceRecord(1, 1, DateTimeOffset.UtcNow.AddDays(2), "Inspection", "SCHEDULED");
        Assert.Throws<DomainRuleException>(() => m.Advance("COMPLETED", "Tech", false, DateTimeOffset.UtcNow));
    }
    [Fact]
    public void ResolvingAnIncidentPreservesItsBlockingHistory()
    {
        var i = new Incident(1, 1, "Fault", true);
        i.Resolve();
        Assert.True(i.BlocksRental);
        Assert.Equal("RESOLVED", i.Status);
    }
}
