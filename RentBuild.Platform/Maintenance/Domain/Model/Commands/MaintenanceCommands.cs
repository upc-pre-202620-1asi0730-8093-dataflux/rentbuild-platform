namespace RentBuild.Platform.Maintenance.Domain.Model.Commands;

public sealed record RegisterMaintenanceCommand(int EquipmentId, DateTimeOffset PerformedAt, string Type, string Status);
public sealed record AdvanceMaintenanceCommand(int Id, string? Status, string? Technician, bool Acknowledge, string Notes);
public sealed record ReportIncidentCommand(int EquipmentId, string Description, bool BlocksRental);
public sealed record PatchIncidentCommand(int Id, string? Status, bool? BlocksRental);
public sealed record ReactivateEquipmentCommand(int EquipmentId, bool InspectionConfirmed);
