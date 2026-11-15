using RentBuild.Platform.Shared.Domain.Model.Errors;
namespace RentBuild.Platform.Shared.Domain.Model.ValueObjects;

public static class BusinessCalendar
{
    public static readonly TimeZoneInfo Lima = TimeZoneInfo.FindSystemTimeZoneById("America/Lima");
    public static DateOnly Day(DateTimeOffset instant) => DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(instant, Lima).DateTime);
    public static DateOnly Today => Day(DateTimeOffset.UtcNow);
    public static DateOnly Parse(string input)
    {
        Rule.Require(DateOnly.TryParseExact(input, "yyyy-MM-dd", out _), "Expected YYYY-MM-DD");
        return DateOnly.ParseExact(input, "yyyy-MM-dd");
    }
    public static DateTimeOffset Start(DateOnly day) => new(day.ToDateTime(TimeOnly.MinValue), TimeSpan.FromHours(-5));
    public static DateTimeOffset End(DateOnly day) => new(day.ToDateTime(new TimeOnly(23, 59, 59, 999)), TimeSpan.FromHours(-5));
}
