namespace RentBuild.Platform.Rentals.Interfaces.Rest.Resources;

public sealed record RentalRequestResource(int Id, int EquipmentId, int ConstructionUserId, int RentalCompanyUserId, DateTimeOffset StartDate, DateTimeOffset EndDate, string Status = "PENDING", DateTimeOffset? CreatedAt = null);
public sealed record RentalResource(int Id, int EquipmentId, int ConstructionUserId, int RentalCompanyUserId, DateTimeOffset StartDate, DateTimeOffset EndDate, string Status);
public sealed record PeriodResource(DateTimeOffset StartDate, DateTimeOffset EndDate);
public sealed record DeliveryResource(int Id, int RentalId, DateTimeOffset DeliveredAt, string Notes = "");
public sealed record ReturnResource(int Id, int RentalId, DateTimeOffset ReturnedAt, string Notes = "", bool MaintenanceRequired = false);
