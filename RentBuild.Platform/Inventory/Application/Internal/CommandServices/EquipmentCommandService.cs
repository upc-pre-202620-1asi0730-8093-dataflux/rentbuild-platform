using RentBuild.Platform.Inventory.Application.CommandServices;
using RentBuild.Platform.Inventory.Application.Internal.OutboundServices.Acl;
using RentBuild.Platform.Inventory.Domain.Model.Aggregates;
using RentBuild.Platform.Inventory.Domain.Model.Commands;
using RentBuild.Platform.Inventory.Domain.Model.ValueObjects;
using RentBuild.Platform.Inventory.Domain.Repositories;
using RentBuild.Platform.Shared.Application.Internal;
using RentBuild.Platform.Shared.Application.Model;
using RentBuild.Platform.Shared.Application.OutboundServices;
using RentBuild.Platform.Shared.Domain.Model.Errors;
namespace RentBuild.Platform.Inventory.Application.Internal.CommandServices;

public sealed class EquipmentCommandService(IEquipmentRepository equipment, IEquipmentCategoryRepository categories, IInventoryAccess access, ICurrentActor actor, CommandExecutor executor) : IEquipmentCommandService
{
    async Task Eligible()
    {
        actor.RequireRole("rental_company");
        Rule.Require(await access.HasSubscription(actor.UserId), "An active subscription is required", ErrorKind.Forbidden, "SUBSCRIPTION_REQUIRED");
    }
    async Task<Equipment> Owned(int id)
    {
        var e = Rule.Found(await equipment.FindByIdAsync(id), "Equipment");
        Rule.Owner(actor.UserId, e.UserId);
        return e;
    }
    async Task Fields(EquipmentFields f, int? except = null)
    {
        Rule.Found(await categories.FindByIdAsync(f.CategoryId), "Equipment category");
        Rule.Require(!(await equipment.ListAsync()).Any(e => e.UserId == actor.UserId && e.Id != except && e.Code.Equals(f.Code.Trim(), StringComparison.OrdinalIgnoreCase)), "Equipment code already exists", ErrorKind.Conflict);
    }
    public Task<Result<Equipment>> Handle(CreateEquipmentCommand c) => executor.ExecuteAsync(async () => { await Eligible(); Rule.Owner(actor.UserId, c.UserId); await Fields(c.Fields); var f = c.Fields; var e = new Equipment(c.UserId, f.Code, f.Name, f.Description, f.CategoryId, f.Location, new RentalRate(f.DailyRate, f.WeeklyRate, f.Currency)); await equipment.AddAsync(e); return e; });
    public Task<Result<Equipment>> Handle(UpdateEquipmentCommand c) => executor.ExecuteAsync(async () => { await Eligible(); var e = await Owned(c.Id); await Fields(c.Fields, c.Id); var f = c.Fields; e.Update(f.Code, f.Name, f.Description, f.CategoryId, f.Location, new RentalRate(f.DailyRate, f.WeeklyRate, f.Currency)); return e; });
    public Task<Result<Equipment>> Handle(ReserveEquipmentCommand c) => executor.ExecuteAsync(async () => { await Eligible(); var e = await Owned(c.Id); e.Reserve(c.StartDate, c.EndDate); return e; });
    public Task<Result<Equipment>> Handle(RestrictEquipmentCommand c) => executor.ExecuteAsync(async () => { await Eligible(); var e = await Owned(c.Id); Rule.Require(c.Status == "MAINTENANCE", "Use rental lifecycle or maintenance reactivation to change operational status", ErrorKind.Conflict); e.Restrict(); return e; });
    public Task<Result<bool>> Handle(DeleteEquipmentCommand c) => executor.ExecuteAsync(async () => { await Eligible(); var e = await Owned(c.Id); Rule.Require(e.CanDelete, "Equipment has operational history or reservations", ErrorKind.Conflict); equipment.Remove(e); return true; });
}
