using EventStore.Client;
using NodaTime;

namespace DistributedSystemsPatterns.Shared.EventStore;

public record EventMetadata(Instant Timestamp, ulong StreamPosition, ulong AggregatedStreamPosition)
{
  public static EventMetadata FromResolvedEvent(ResolvedEvent resolvedEvent) =>
    new(
      resolvedEvent.Event.CreatedInstant(),
      resolvedEvent.Event.EventNumber,
      resolvedEvent.OriginalEventNumber);
}
