using RentBuild.Platform.Shared.Domain.Model.Aggregates;
namespace RentBuild.Platform.Inventory.Domain.Model.Aggregates;

public sealed class EquipmentCategory : AggregateRoot
{
    public string Name { get; private set; } = ""; private EquipmentCategory()
    {
    }
    public EquipmentCategory(int id, string name)
    {
        Id = id;
        Name = name;
    }
}
