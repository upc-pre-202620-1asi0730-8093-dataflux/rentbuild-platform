using RentBuild.Platform.Shared.Domain.Model.Aggregates;
using RentBuild.Platform.Shared.Domain.Model.Errors;
using RentBuild.Platform.Inventory.Domain.Model.Entities;
using RentBuild.Platform.Inventory.Domain.Model.ValueObjects;
namespace RentBuild.Platform.Inventory.Domain.Model.Aggregates;

public sealed class Equipment : AggregateRoot
{
    public int UserId
    {
        get; private set;
    }
    public string Code { get; private set; } = ""; public string Name { get; private set; } = ""; public string Description { get; private set; } = ""; public int CategoryId
    {
        get; private set;
    }
    public string Location { get; private set; } = ""; public RentalRate RentalRate { get; private set; } = null!; public EquipmentStatus Status { get; private set; } = EquipmentStatus.AVAILABLE;
    private readonly List<AvailabilityBlock> _blocks = []; public IReadOnlyCollection<AvailabilityBlock> AvailabilityBlocks => _blocks.AsReadOnly();
    private Equipment()
    {
    }
    public Equipment(int userId, string code, string name, string description, int categoryId, string location, RentalRate rate)
    {
        Rule.Require(userId > 0, "Invalid owner");
        UserId = userId;
        Update(code, name, description, categoryId, location, rate);
        Raise("EquipmentRegistered");
    }
    public void Update(string code, string name, string description, int categoryId, string location, RentalRate rate)
    {
        Code = Rule.Text(code, "Code", 60);
        Name = Rule.Text(name, "Name", 180);
        Description = Rule.Text(description, "Description", 3000);
        Rule.Require(categoryId > 0, "Invalid category");
        CategoryId = categoryId;
        Location = Rule.Text(location, "Location");
        RentalRate = rate;
    }
    public bool IsAvailable(DateOnly start, DateOnly end)
    {
        Rule.Require(end >= start, "Invalid availability range");
        return Status == EquipmentStatus.AVAILABLE && !_blocks.Any(b => b.Overlaps(start, end));
    }
    public AvailabilityBlock Reserve(DateOnly start, DateOnly end)
    {
        Rule.Require(IsAvailable(start, end), "Equipment is already reserved for the selected period", ErrorKind.Conflict);
        var block = new AvailabilityBlock(start, end);
        _blocks.Add(block);
        Raise("EquipmentReserved");
        return block;
    }
    public void Release(int blockId)
    {
        var block = Rule.Found(_blocks.FirstOrDefault(b => b.Id == blockId), "Reservation");
        block.Release();
        Raise("ReservationReleased");
    }
    public void MarkRented()
    {
        Rule.Require(Status == EquipmentStatus.AVAILABLE, "Equipment cannot be delivered", ErrorKind.Conflict);
        Status = EquipmentStatus.RENTED;
        Raise("EquipmentRented");
    }
    public void Restrict()
    {
        Status = EquipmentStatus.MAINTENANCE;
        Raise("EquipmentRestricted");
    }
    public void FinishRental(bool maintenanceRequired)
    {
        Status = maintenanceRequired ? EquipmentStatus.MAINTENANCE : EquipmentStatus.AVAILABLE;
        Raise("EquipmentReturned");
    }
    public void Reactivate()
    {
        Rule.Require(Status == EquipmentStatus.MAINTENANCE, "Equipment is not in maintenance", ErrorKind.Conflict);
        Status = EquipmentStatus.AVAILABLE;
        Raise("EquipmentReactivated");
    }
    public bool CanDelete => Status == EquipmentStatus.AVAILABLE && _blocks.Count == 0;
}
