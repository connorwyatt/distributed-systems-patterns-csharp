using System.Collections.Immutable;

namespace DistributedSystemsPatterns.Shared.EventStore.Aggregates;

public abstract class Aggregate
{
  public string Id { get => _id ?? throw new InvalidOperationException(); init => _id = value; }

  private ImmutableArray<EventEnvelope<IEvent>> SavedEvents = ImmutableArray<EventEnvelope<IEvent>>.Empty;

  private ImmutableArray<IEvent> UnsavedEvents = ImmutableArray<IEvent>.Empty;

  private readonly IDictionary<Type, Action<IEvent>> _handlers = new Dictionary<Type, Action<IEvent>>();

  private readonly string? _id;

  public void ReplayEvent(EventEnvelope<IEvent> eventEnvelope)
  {
    ApplyEvent(eventEnvelope.Event);
    SavedEvents = SavedEvents.Add(eventEnvelope);
  }

  public IReadOnlyCollection<IEvent> GetUnsavedEvents() => UnsavedEvents;

  public ulong? LatestSavedEventVersion() => SavedEvents.LastOrDefault()?.Metadata.StreamPosition;

  protected void When<T>(Action<T> handler) where T : class, IEvent
  {
    _handlers.Add(
      typeof(T),
      e => handler(
        e as T ??
        throw new InvalidOperationException($"Cannot apply event, as event cannot be cast to \"{typeof(T).Name}\".")));
  }

  protected void RaiseEvent(IEvent @event)
  {
    ApplyEvent(@event);
    UnsavedEvents = UnsavedEvents.Add(@event);
  }

  private void ApplyEvent(IEvent @event)
  {
    var eventType = @event.GetType();
    if (!_handlers.TryGetValue(eventType, out var handler))
    {
      throw new InvalidOperationException($"No handler for {eventType.Name}.");
    }

    handler.Invoke(@event);
  }
}
