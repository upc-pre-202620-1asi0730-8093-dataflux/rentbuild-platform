using RentBuild.Platform.Shared.Domain.Model.Errors;
namespace RentBuild.Platform.Inventory.Domain.Model.ValueObjects;

public sealed record RentalRate
{
    private RentalRate()
    {
        Currency = "";
    }
    public decimal DailyRate
    {
        get;
    }
    public decimal WeeklyRate
    {
        get;
    }
    public string Currency
    {
        get;
    }
    public RentalRate(decimal daily, decimal weekly, string currency = "PEN")
    {
        Rule.Require(daily > 0 && weekly > 0 && daily <= 999999999 && weekly <= 999999999, "Rental prices must be positive");
        currency = currency.Trim().ToUpperInvariant();
        Rule.Require(System.Text.RegularExpressions.Regex.IsMatch(currency, "^[A-Z]{3}$"), "Invalid currency");
        DailyRate = daily;
        WeeklyRate = weekly;
        Currency = currency;
    }
}
public enum EquipmentStatus
{
    AVAILABLE, RENTED, MAINTENANCE
}
