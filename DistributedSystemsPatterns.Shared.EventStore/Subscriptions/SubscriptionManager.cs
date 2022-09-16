using EventStore.Client;

namespace DistributedSystemsPatterns.Shared.EventStore.Subscriptions;

public class SubscriptionManager
{
  private readonly EventStoreClient _eventStoreClient;
  private readonly ISubscriptionCursorsRepository _subscriptionCursorsRepository;
  private readonly ISubscriber _subscriber;
  private readonly IList<StreamSubscription> _streamSubscriptions = new List<StreamSubscription>();

  public SubscriptionManager(
    EventStoreClient eventStoreClient,
    ISubscriptionCursorsRepository subscriptionCursorsRepository,
    ISubscriber subscriber)
  {
    _eventStoreClient = eventStoreClient;
    _subscriptionCursorsRepository = subscriptionCursorsRepository;
    _subscriber = subscriber;
  }

  public async Task StartAsync(CancellationToken cancellationToken)
  {
    var subscriptionAttributes = SubscriberUtilities.GetSubscriptionAttributes(_subscriber.GetType());

    var streamNames = subscriptionAttributes
      .Select(subscriptionAttribute => subscriptionAttribute.StreamName)
      .Distinct();

    foreach (var streamName in streamNames)
    {
      var currentStreamPosition = await GetCursor(streamName);

      var streamSubscription = await _eventStoreClient.SubscribeToStreamAsync(
        streamName,
        currentStreamPosition.HasValue ? FromStream.After(currentStreamPosition.Value) : FromStream.Start,
        async (_, @event, _) =>
        {
          await _subscriber.HandleEvent(@event);
          await UpdateCursor(streamName, @event);
        },
        true,
        cancellationToken);

      _streamSubscriptions.Add(streamSubscription);
    }
  }

  public Task StopAsync(CancellationToken cancellationToken)
  {
    foreach (var streamSubscription in _streamSubscriptions)
    {
      streamSubscription.Dispose();
    }

    return Task.CompletedTask;
  }

  private async Task<ulong?> GetCursor(string streamName)
  {
    var subscriberType = _subscriber.GetType();
    var subscriberName = SubscriberUtilities.GetSubscriberName(subscriberType);

    return await _subscriptionCursorsRepository.GetSubscriptionCursor(
      subscriberName,
      streamName);
  }

  private async Task UpdateCursor(string streamName, EventEnvelope<IEvent> @event)
  {
    var subscriberType = _subscriber.GetType();

    await _subscriptionCursorsRepository.UpsertSubscriptionCursor(
      SubscriberUtilities.GetSubscriberName(subscriberType),
      streamName,
      @event.Metadata.AggregatedStreamPosition);
  }
}
