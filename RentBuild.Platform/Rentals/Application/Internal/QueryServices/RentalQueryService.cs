using RentBuild.Platform.Rentals.Application.QueryServices;
using RentBuild.Platform.Rentals.Domain.Model.Aggregates;
using RentBuild.Platform.Rentals.Domain.Repositories;
using RentBuild.Platform.Shared.Application.OutboundServices;
using RentBuild.Platform.Shared.Domain.Model.Errors;
namespace RentBuild.Platform.Rentals.Application.Internal.QueryServices;

public sealed class RentalQueryService(IRentalRequestRepository requests, IRentalRepository rentals, IDeliveryRepository deliveries, IEquipmentReturnRepository returns, ICurrentActor actor) : IRentalQueryService
{
    public async Task<List<RentalRequest>> Requests() => (await requests.ListAsNoTrackingAsync()).Where(r => r.ConstructionUserId == actor.UserId || r.RentalCompanyUserId == actor.UserId).ToList();
    public async Task<RentalRequest> Request(int id)
    {
        var r = Rule.Found(await requests.FindByIdAsNoTrackingAsync(id), "Rental request");
        Rule.Require(r.ConstructionUserId == actor.UserId || r.RentalCompanyUserId == actor.UserId, "Request belongs to another company", ErrorKind.Forbidden);
        return r;
    }
    public async Task<List<Rental>> Rentals() => (await rentals.ListAsNoTrackingAsync()).Where(r => r.ConstructionUserId == actor.UserId || r.RentalCompanyUserId == actor.UserId).ToList();
    public async Task<Rental> Rental(int id)
    {
        var r = Rule.Found(await rentals.FindByIdAsNoTrackingAsync(id), "Rental");
        Rule.Require(r.ConstructionUserId == actor.UserId || r.RentalCompanyUserId == actor.UserId, "Rental belongs to another company", ErrorKind.Forbidden);
        return r;
    }
    public async Task<List<Delivery>> Deliveries()
    {
        var ids = (await Rentals()).Select(r => r.Id).ToHashSet();
        return (await deliveries.ListAsNoTrackingAsync()).Where(d => ids.Contains(d.RentalId)).ToList();
    }
    public async Task<List<EquipmentReturn>> Returns()
    {
        var ids = (await Rentals()).Select(r => r.Id).ToHashSet();
        return (await returns.ListAsNoTrackingAsync()).Where(d => ids.Contains(d.RentalId)).ToList();
    }
}
