using RentBuild.Platform.Shared.Domain.Model.Aggregates;
namespace RentBuild.Platform.Maintenance.Domain.Model.Aggregates;

public sealed class MaintenanceActivity : AggregateRoot
{
    public int MaintenanceId
    {
        get; private set;
    }
    public DateTimeOffset OccurredAt
    {
        get; private set;
    }
    public string Status { get; private set; } = ""; public string Technician { get; private set; } = ""; public string Notes { get; private set; } = ""; private MaintenanceActivity()
    {
    }
    public MaintenanceActivity(int id, string status, string technician, string notes)
    {
        MaintenanceId = id;
        Status = status;
        Technician = technician;
        Notes = notes;
        OccurredAt = DateTimeOffset.UtcNow;
    }
}
