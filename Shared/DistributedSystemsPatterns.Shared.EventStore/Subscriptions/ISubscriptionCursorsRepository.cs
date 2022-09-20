namespace DistributedSystemsPatterns.Shared.EventStore.Subscriptions;

public interface ISubscriptionCursorsRepository
{
  Task<ulong?> GetSubscriptionCursor(string subscriberName, string streamName);

  Task UpsertSubscriptionCursor(string subscriberName, string streamName, ulong position);
}
