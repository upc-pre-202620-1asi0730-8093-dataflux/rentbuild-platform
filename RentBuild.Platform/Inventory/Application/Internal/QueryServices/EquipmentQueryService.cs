using RentBuild.Platform.Inventory.Application.QueryServices;
using RentBuild.Platform.Inventory.Domain.Model.Aggregates;
using RentBuild.Platform.Inventory.Domain.Repositories;
using RentBuild.Platform.Shared.Domain.Model.Errors;
namespace RentBuild.Platform.Inventory.Application.Internal.QueryServices;

public sealed class EquipmentQueryService(IEquipmentRepository equipment, IEquipmentCategoryRepository categories) : IEquipmentQueryService
{
    public async Task<List<Equipment>> List(int? userId) => (await equipment.ListAsNoTrackingAsync()).Where(e => userId is null || e.UserId == userId).ToList(); public async Task<Equipment> Find(int id) => Rule.Found(await equipment.FindByIdAsNoTrackingAsync(id), "Equipment"); public Task<List<EquipmentCategory>> Categories() => categories.ListAsNoTrackingAsync(); public async Task<bool> Availability(int id, DateOnly start, DateOnly end) => (await Find(id)).IsAvailable(start, end);
}
