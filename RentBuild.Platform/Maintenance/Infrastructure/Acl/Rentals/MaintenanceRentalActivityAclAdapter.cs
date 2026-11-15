using RentBuild.Platform.Maintenance.Application.Internal.OutboundServices.Acl;
using RentBuild.Platform.Rentals.Application.Acl;
namespace RentBuild.Platform.Maintenance.Infrastructure.Acl.Rentals;

public sealed class MaintenanceRentalActivityAclAdapter(IRentalsContextFacade rentals) : IMaintenanceRentalActivity
{
    public Task<bool> HasActiveRental(int id) => rentals.HasActiveRental(id); public async Task<List<HistoricalRental>> History(int id) => (await rentals.History(id)).Select(r => new HistoricalRental(r.Id, r.OccurredAt, r.Description, r.Status)).ToList();
}
