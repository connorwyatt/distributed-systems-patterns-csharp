namespace DistributedSystemsPatterns.Shared.EventStore.Subscriptions;

public abstract class SubscriberBase : EventHandler, ISubscriber
{
  public async Task HandleEvent(EventEnvelope<IEvent> eventEnvelope)
  {
    if (!CanHandleEvent(eventEnvelope))
    {
      return;
    }

    await base.HandleEvent(eventEnvelope);
  }
}
