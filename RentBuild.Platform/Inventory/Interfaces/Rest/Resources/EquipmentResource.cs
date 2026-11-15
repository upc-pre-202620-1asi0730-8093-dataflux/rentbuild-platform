namespace RentBuild.Platform.Inventory.Interfaces.Rest.Resources;

public sealed record AvailabilityBlockResource(int Id, string StartDate, string EndDate);
public sealed record EquipmentResource(int Id, int UserId, string Code, string Name, string Description, int CategoryId, string Location, decimal DailyRate, decimal WeeklyRate, string Currency = "PEN", string? Status = null, List<AvailabilityBlockResource>? AvailabilityBlocks = null);
public sealed record ReserveResource(string StartDate, string EndDate);
public sealed record OperationalStatusResource(string Status);
