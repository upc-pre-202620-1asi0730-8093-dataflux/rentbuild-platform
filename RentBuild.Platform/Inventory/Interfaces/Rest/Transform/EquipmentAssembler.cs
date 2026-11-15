using RentBuild.Platform.Inventory.Domain.Model.Aggregates;
using RentBuild.Platform.Inventory.Domain.Model.Commands;
using RentBuild.Platform.Inventory.Interfaces.Rest.Resources;
namespace RentBuild.Platform.Inventory.Interfaces.Rest.Transform;

public static class EquipmentAssembler
{
    public static EquipmentResource Resource(Equipment e) => new(e.Id, e.UserId, e.Code, e.Name, e.Description, e.CategoryId, e.Location, e.RentalRate.DailyRate, e.RentalRate.WeeklyRate, e.RentalRate.Currency, e.Status.ToString(), e.AvailabilityBlocks.Where(b => !b.Released).Select(b => new AvailabilityBlockResource(b.Id, b.StartDate.ToString("yyyy-MM-dd"), b.EndDate.ToString("yyyy-MM-dd"))).ToList()); public static EquipmentFields Fields(EquipmentResource r) => new(r.Code, r.Name, r.Description, r.CategoryId, r.Location, r.DailyRate, r.WeeklyRate, r.Currency);
}
