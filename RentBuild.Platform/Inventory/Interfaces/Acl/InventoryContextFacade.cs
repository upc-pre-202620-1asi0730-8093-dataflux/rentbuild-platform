using RentBuild.Platform.Inventory.Application.Acl;
using RentBuild.Platform.Inventory.Domain.Repositories;
using RentBuild.Platform.Shared.Domain.Model.Errors;
using RentBuild.Platform.Shared.Domain.Repositories;
namespace RentBuild.Platform.Inventory.Interfaces.Acl;

public sealed class InventoryContextFacade(IEquipmentRepository equipment, IUnitOfWork work) : IInventoryContextFacade
{
    public async Task<EquipmentSnapshot> Find(int id)
    {
        var e = Rule.Found(await equipment.FindByIdAsync(id), "Equipment");
        return new(e.Id, e.UserId, e.Code, e.Name, e.Status.ToString());
    }
    public async Task<bool> IsAvailable(int id, DateOnly start, DateOnly end) => Rule.Found(await equipment.FindByIdAsync(id), "Equipment").IsAvailable(start, end);
    public async Task<int> Reserve(int id, DateOnly start, DateOnly end)
    {
        var e = Rule.Found(await equipment.FindByIdAsync(id), "Equipment");
        var block = e.Reserve(start, end);
        await work.FlushAsync();
        return block.Id;
    }
    public async Task MarkRented(int id) => Rule.Found(await equipment.FindByIdAsync(id), "Equipment").MarkRented();
    public async Task FinishRental(int id, int blockId, bool maintenance)
    {
        var e = Rule.Found(await equipment.FindByIdAsync(id), "Equipment");
        e.FinishRental(maintenance);
        e.Release(blockId);
    }
    public async Task Restrict(int id) => Rule.Found(await equipment.FindByIdAsync(id), "Equipment").Restrict();
    public async Task Reactivate(int id) => Rule.Found(await equipment.FindByIdAsync(id), "Equipment").Reactivate();
}
