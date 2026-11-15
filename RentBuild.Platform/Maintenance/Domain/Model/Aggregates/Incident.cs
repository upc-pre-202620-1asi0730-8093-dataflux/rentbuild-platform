using RentBuild.Platform.Shared.Domain.Model.Aggregates;
using RentBuild.Platform.Shared.Domain.Model.Errors;
namespace RentBuild.Platform.Maintenance.Domain.Model.Aggregates;

public sealed class Incident : AggregateRoot
{
    public int EquipmentId
    {
        get; private set;
    }
    public int OwnerUserId
    {
        get; private set;
    }
    public string Description { get; private set; } = ""; public DateTimeOffset ReportedAt
    {
        get; private set;
    }
    public bool BlocksRental
    {
        get; private set;
    }
    public string Status { get; private set; } = "OPEN"; public DateTimeOffset? ResolvedAt
    {
        get; private set;
    }
    private Incident()
    {
    }
    public Incident(int equipmentId, int owner, string description, bool blocks)
    {
        EquipmentId = equipmentId;
        OwnerUserId = owner;
        Description = Rule.Text(description, "Incident description", 3000);
        BlocksRental = blocks;
        ReportedAt = DateTimeOffset.UtcNow;
        Raise("IncidentReported");
    }
    public void Resolve()
    {
        Rule.Require(Status == "OPEN", "Incident is already resolved", ErrorKind.Conflict);
        Status = "RESOLVED";
        ResolvedAt = DateTimeOffset.UtcNow;
        Raise("IncidentResolved");
    }
    public void Restrict()
    {
        Rule.Require(Status == "OPEN", "Only open incidents can restrict rentals", ErrorKind.Conflict);
        BlocksRental = true;
        Raise("IncidentBecameBlocking");
    }
}
