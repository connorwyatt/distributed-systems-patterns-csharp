namespace DistributedSystemsPatterns.Shared.EventStore.Subscriptions;

public interface ISubscriber
{
  public Task HandleEvent(EventEnvelope<IEvent> eventEnvelope);
}
