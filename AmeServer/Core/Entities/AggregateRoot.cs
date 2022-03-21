using AmeServer.Core.Events;

namespace AmeServer.Core.Entities;

public abstract class AggregateRoot<T>
{
    public T Id { get; protected set; }
    public int Version { get; protected set; }
    public IEnumerable<IDomainEvent> Events => events;

    private readonly List<IDomainEvent> events = new();
    private bool versionIncremented;

    protected void AddEvent(IDomainEvent @event)
    {
        if (!events.Any() && !versionIncremented)
        {
            Version++;
            versionIncremented = true;
        }

        events.Add(@event);
    }

    public void ClearEvents() => events.Clear();

    protected void IncrementVersion()
    {
        if (versionIncremented)
        {
            return;
        }

        Version++;
        versionIncremented = true;
    }
}