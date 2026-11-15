namespace RentBuild.Platform.Maintenance.Interfaces.Rest.Resources;

public sealed record MaintenanceResource(int Id, int EquipmentId, DateTimeOffset PerformedAt, string Type, string Status, string Technician = "", DateTimeOffset? AcknowledgedAt = null);
public sealed record MaintenancePatchResource(string? Status = null, string? Technician = null, bool Acknowledge = false, string Notes = "");
public sealed record IncidentResource(int Id, int EquipmentId, string Description, DateTimeOffset? ReportedAt = null, bool BlocksRental = false, string Status = "OPEN", DateTimeOffset? ResolvedAt = null);
public sealed record IncidentPatchResource(string? Status = null, bool? BlocksRental = null, DateTimeOffset? ResolvedAt = null);
public sealed record ReactivationResource(bool InspectionConfirmed);
