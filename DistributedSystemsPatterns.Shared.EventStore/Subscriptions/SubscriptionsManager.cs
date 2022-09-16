using Microsoft.Extensions.Hosting;

namespace DistributedSystemsPatterns.Shared.EventStore.Subscriptions;

public class SubscriptionsManager : IHostedService
{
  private readonly EventStoreClient _eventStoreClient;
  private readonly ISubscriptionCursorsRepository _subscriptionCursorsRepository;
  private readonly IEnumerable<ISubscriber> _subscribers;

  private IList<SubscriptionManager> _subscriptionManagers = new List<SubscriptionManager>();

  public SubscriptionsManager(
    EventStoreClient eventStoreClient,
    ISubscriptionCursorsRepository subscriptionCursorsRepository,
    IEnumerable<ISubscriber> subscribers)
  {
    _eventStoreClient = eventStoreClient;
    _subscriptionCursorsRepository = subscriptionCursorsRepository;
    _subscribers = subscribers;
  }

  public async Task StartAsync(CancellationToken cancellationToken)
  {
    _subscriptionManagers = _subscribers
      .Select(subscriber => new SubscriptionManager(_eventStoreClient, _subscriptionCursorsRepository, subscriber))
      .ToList();

    await Task.WhenAll(
      _subscriptionManagers.Select(subscriptionManager => subscriptionManager.StartAsync(cancellationToken)));
  }

  public async Task StopAsync(CancellationToken cancellationToken)
  {
    await Task.WhenAll(
      _subscriptionManagers.Select(subscriptionManager => subscriptionManager.StopAsync(cancellationToken)));
  }
}
