namespace RentBuild.Platform.Shared.Domain.Model.Aggregates;

public sealed record DomainEvent(string Name, DateTimeOffset OccurredAt);
public abstract class AggregateRoot
{
    public int Id
    {
        get; protected set;
    }
    private readonly List<DomainEvent> _events = [];
    public IReadOnlyCollection<DomainEvent> DomainEvents => _events.AsReadOnly();
    protected void Raise(string name) => _events.Add(new(name, DateTimeOffset.UtcNow));
    public void ClearDomainEvents() => _events.Clear();
}