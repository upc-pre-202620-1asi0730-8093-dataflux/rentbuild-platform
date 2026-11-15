using RentBuild.Platform.Shared.Domain.Model.Errors;
namespace RentBuild.Platform.Inventory.Domain.Model.Entities;

public sealed class AvailabilityBlock
{
    public int Id
    {
        get; private set;
    }
    public int EquipmentId
    {
        get; private set;
    }
    public DateOnly StartDate
    {
        get; private set;
    }
    public DateOnly EndDate
    {
        get; private set;
    }
    public bool Released
    {
        get; private set;
    }
    private AvailabilityBlock()
    {
    }
    public AvailabilityBlock(DateOnly start, DateOnly end)
    {
        Rule.Require(end >= start, "Invalid availability range");
        StartDate = start;
        EndDate = end;
    }
    public bool Overlaps(DateOnly start, DateOnly end) => !Released && StartDate <= end && start <= EndDate;
    public void Release() => Released = true;
}
