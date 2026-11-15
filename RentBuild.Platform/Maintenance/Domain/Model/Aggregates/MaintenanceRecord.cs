using RentBuild.Platform.Shared.Domain.Model.Aggregates;
using RentBuild.Platform.Shared.Domain.Model.Errors;
using RentBuild.Platform.Shared.Domain.Model.ValueObjects;
namespace RentBuild.Platform.Maintenance.Domain.Model.Aggregates;

public sealed class MaintenanceRecord : AggregateRoot
{
    public int EquipmentId
    {
        get; private set;
    }
    public int OwnerUserId
    {
        get; private set;
    }
    public DateTimeOffset PerformedAt
    {
        get; private set;
    }
    public string Type { get; private set; } = ""; public string Status { get; private set; } = ""; public string Technician { get; private set; } = ""; public DateTimeOffset? AcknowledgedAt
    {
        get; private set;
    }
    private MaintenanceRecord()
    {
    }
    public MaintenanceRecord(int equipmentId, int owner, DateTimeOffset at, string type, string status)
    {
        Rule.Require(at != default, "Maintenance date required");
        Rule.Require(status is "SCHEDULED" or "COMPLETED", "Invalid initial maintenance status");
        Rule.Require(status == "SCHEDULED" ? BusinessCalendar.Day(at) >= BusinessCalendar.Today : BusinessCalendar.Day(at) <= BusinessCalendar.Today, "Invalid maintenance date");
        EquipmentId = equipmentId;
        OwnerUserId = owner;
        PerformedAt = at;
        Type = Rule.Text(type, "Maintenance type", 200);
        Status = status;
        Raise("MaintenanceRegistered");
    }
    public void Advance(string? target, string? technician, bool acknowledge, DateTimeOffset now)
    {
        Rule.Require(Status != "COMPLETED", "Completed maintenance cannot change", ErrorKind.Conflict);
        if (!string.IsNullOrWhiteSpace(technician))
            Technician = Rule.Text(technician, "Technician", 180);
        if (acknowledge)
        {
            Rule.Require(Technician.Length > 0, "Assign a technician first");
            Rule.Require(AcknowledgedAt is null, "Work already acknowledged", ErrorKind.Conflict);
            AcknowledgedAt = now;
        }
        if (target is not null)
        {
            Rule.Require((Status == "SCHEDULED" && target is "IN_PROGRESS" or "COMPLETED") || (Status == "IN_PROGRESS" && target == "COMPLETED"), "Invalid maintenance transition", ErrorKind.Conflict);
            Rule.Require(BusinessCalendar.Day(now) >= BusinessCalendar.Day(PerformedAt), "Maintenance cannot advance before its scheduled date", ErrorKind.Conflict);
            Rule.Require(target != "COMPLETED" || Technician.Length > 0, "Technician is required to complete maintenance");
            Status = target;
        }
        Raise("MaintenanceAdvanced");
    }
}
