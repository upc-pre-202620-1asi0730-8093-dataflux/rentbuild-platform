using RentBuild.Platform.Shared.Domain.Model.Aggregates;
namespace RentBuild.Platform.Rentals.Domain.Model.Aggregates;

public sealed class EquipmentReturn : AggregateRoot
{
    public int RentalId
    {
        get; private set;
    }
    public DateTimeOffset ReturnedAt
    {
        get; private set;
    }
    public string Notes { get; private set; } = ""; public bool MaintenanceRequired
    {
        get; private set;
    }
    private EquipmentReturn()
    {
    }
    public EquipmentReturn(int rentalId, DateTimeOffset at, string notes, bool maintenanceRequired)
    {
        RentalId = rentalId;
        ReturnedAt = at;
        Notes = notes.Trim();
        MaintenanceRequired = maintenanceRequired;
    }
}
