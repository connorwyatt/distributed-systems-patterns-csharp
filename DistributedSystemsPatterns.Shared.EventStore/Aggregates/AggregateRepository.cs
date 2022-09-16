using EventStore.Client;

namespace DistributedSystemsPatterns.Shared.EventStore.Aggregates;

public class AggregateRepository
{
  private readonly EventStoreClient _eventStoreClient;

  public AggregateRepository(EventStoreClient eventStoreClient) => _eventStoreClient = eventStoreClient;

  public async Task<T> LoadAggregate<T>(string id) where T : Aggregate, new()
  {
    var streamName = AggregateUtilities.GetStreamName<T>(id);

    var stream = await _eventStoreClient.ReadStreamAsync(
      Direction.Forwards,
      streamName,
      StreamPosition.Start);

    var aggregate = new T
    {
      Id = id,
    };

    if (stream == null)
    {
      return aggregate;
    }

    await foreach (var @event in stream)
    {
      aggregate.ReplayEvent(@event);
    }

    return aggregate;
  }

  public async Task SaveAggregate<T>(T aggregate) where T : Aggregate
  {
    var unsavedEvents = aggregate.GetUnsavedEvents();
    var hasUnsavedEvents = unsavedEvents.Any();

    if (!hasUnsavedEvents)
    {
      return;
    }

    await _eventStoreClient.AppendToStreamAsync(
      AggregateUtilities.GetStreamName<T>(aggregate.Id),
      aggregate.LatestSavedEventVersion() ?? StreamRevision.None,
      unsavedEvents);
  }
}
