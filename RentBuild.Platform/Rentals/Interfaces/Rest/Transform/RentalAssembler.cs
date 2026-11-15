using RentBuild.Platform.Rentals.Domain.Model.Aggregates;
using RentBuild.Platform.Rentals.Domain.Model.Commands;
using RentBuild.Platform.Rentals.Interfaces.Rest.Resources;
namespace RentBuild.Platform.Rentals.Interfaces.Rest.Transform;

public static class RentalAssembler
{
    public static RentalRequestResource Resource(RentalRequest r) => new(r.Id, r.EquipmentId, r.ConstructionUserId, r.RentalCompanyUserId, r.Period.StartDate, r.Period.EndDate, r.Status.ToString(), r.CreatedAt);
    public static RentalResource Resource(Rental r) => new(r.Id, r.EquipmentId, r.ConstructionUserId, r.RentalCompanyUserId, r.Period.StartDate, r.Period.EndDate, r.Status.ToString());
    public static DeliveryResource Resource(Delivery d) => new(d.Id, d.RentalId, d.DeliveredAt, d.Notes);
    public static ReturnResource Resource(EquipmentReturn r) => new(r.Id, r.RentalId, r.ReturnedAt, r.Notes, r.MaintenanceRequired);
    public static SubmitRentalRequestCommand Command(RentalRequestResource r) => new(r.EquipmentId, r.ConstructionUserId, r.RentalCompanyUserId, r.StartDate, r.EndDate);
}
