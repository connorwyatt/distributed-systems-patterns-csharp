namespace DistributedSystemsPatterns.Shared.EventStore;

public class EventHandler
{
  private readonly IDictionary<string, Func<IEvent, EventMetadata, Task>> _handlers =
    new Dictionary<string, Func<IEvent, EventMetadata, Task>>();

  protected void When<T>(Func<T, EventMetadata, Task> handler) where T : class, IEvent
  {
    _handlers.Add(
      EventUtilities.GetType<T>(),
      async (@event, metadata) => await handler((T)@event, metadata));
  }

  protected Task HandleEvent<T>(EventEnvelope<T> eventEnvelope) where T : class, IEvent
  {
    if (!_handlers.TryGetValue(EventUtilities.GetType(eventEnvelope.Event.GetType()), out var handler))
    {
      throw new InvalidOperationException();
    }

    return ((Func<T, EventMetadata, Task>)handler).Invoke(eventEnvelope.Event, eventEnvelope.Metadata);
  }

  protected bool CanHandleEvent<T>(EventEnvelope<T> eventEnvelope) where T : class, IEvent =>
    _handlers.ContainsKey(EventUtilities.GetType(eventEnvelope.Event.GetType()));
}
