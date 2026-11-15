using RentBuild.Platform.Rentals.Domain.Model.Aggregates;
namespace RentBuild.Platform.Rentals.Application.QueryServices;

public interface IRentalQueryService
{
    Task<List<RentalRequest>> Requests(); Task<RentalRequest> Request(int id); Task<List<Rental>> Rentals(); Task<Rental> Rental(int id); Task<List<Delivery>> Deliveries(); Task<List<EquipmentReturn>> Returns();
}
