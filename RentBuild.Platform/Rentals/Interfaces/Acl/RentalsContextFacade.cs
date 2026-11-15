using RentBuild.Platform.Rentals.Application.Acl;
using RentBuild.Platform.Rentals.Domain.Repositories;
using RentBuild.Platform.Rentals.Domain.Model.ValueObjects;
namespace RentBuild.Platform.Rentals.Interfaces.Acl;

public sealed class RentalsContextFacade(IRentalRepository rentals) : IRentalsContextFacade
{
    public async Task<bool> HasActiveRental(int equipmentId) => (await rentals.ListAsNoTrackingAsync()).Any(r => r.EquipmentId == equipmentId && r.Status == RentalStatus.ACTIVE); public async Task<List<RentalHistoryItem>> History(int equipmentId) => (await rentals.ListAsNoTrackingAsync()).Where(r => r.EquipmentId == equipmentId).Select(r => new RentalHistoryItem(r.Id, r.Period.StartDate, "Rental #" + r.Id, r.Status.ToString())).ToList();
}
