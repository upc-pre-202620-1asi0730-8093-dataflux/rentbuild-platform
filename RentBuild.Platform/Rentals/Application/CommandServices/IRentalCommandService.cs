using RentBuild.Platform.Rentals.Domain.Model.Aggregates;
using RentBuild.Platform.Rentals.Domain.Model.Commands;
using RentBuild.Platform.Shared.Application.Model;
namespace RentBuild.Platform.Rentals.Application.CommandServices;

public interface IRentalCommandService
{
    Task<Result<RentalRequest>> Handle(SubmitRentalRequestCommand c); Task<Result<RentalRequest>> Handle(DecideRentalRequestCommand c); Task<Result<RentalRequest>> Handle(RescheduleRentalRequestCommand c); Task<Result<Delivery>> Handle(RegisterDeliveryCommand c); Task<Result<EquipmentReturn>> Handle(RegisterReturnCommand c);
}
