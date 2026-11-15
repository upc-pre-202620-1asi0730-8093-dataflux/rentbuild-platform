using RentBuild.Platform.Inventory.Domain.Model.Aggregates;
namespace RentBuild.Platform.Inventory.Application.QueryServices;

public interface IEquipmentQueryService
{
    Task<List<Equipment>> List(int? userId); Task<Equipment> Find(int id); Task<List<EquipmentCategory>> Categories(); Task<bool> Availability(int id, DateOnly start, DateOnly end);
}
