using System.Reflection;
using DistributedSystemsPatterns.Shared.EventStore.Aggregates;
using DistributedSystemsPatterns.Shared.EventStore.Subscriptions;
using Microsoft.Extensions.DependencyInjection;

namespace DistributedSystemsPatterns.Shared.EventStore;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection
    AddEventStore(
      this IServiceCollection services,
      EventStoreClientOptions eventStoreClientOptions,
      IEnumerable<Assembly> assemblies) =>
    services.AddEventStoreClient(eventStoreClientOptions.ConnectionString)
      .AddTransient<EventStoreClient>()
      .AddSingleton(_ => new EventSerializer(assemblies))
      .AddAggregateRepository()
      .AddSubscriptions();
}
