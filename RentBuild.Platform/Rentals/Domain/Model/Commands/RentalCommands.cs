using RentBuild.Platform.Rentals.Domain.Model.ValueObjects;
namespace RentBuild.Platform.Rentals.Domain.Model.Commands;

public sealed record SubmitRentalRequestCommand(int EquipmentId, int ConstructionUserId, int RentalCompanyUserId, DateTimeOffset StartDate, DateTimeOffset EndDate);
public sealed record DecideRentalRequestCommand(int Id, RentalRequestStatus Status);
public sealed record RescheduleRentalRequestCommand(int Id, DateTimeOffset StartDate, DateTimeOffset EndDate);
public sealed record RegisterDeliveryCommand(int RentalId, DateTimeOffset DeliveredAt, string Notes);
public sealed record RegisterReturnCommand(int RentalId, DateTimeOffset ReturnedAt, string Notes, bool MaintenanceRequired);
