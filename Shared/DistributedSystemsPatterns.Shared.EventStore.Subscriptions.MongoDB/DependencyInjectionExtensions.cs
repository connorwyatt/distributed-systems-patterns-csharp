using ConnorWyatt.EventSourcing.Subscriptions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using NodaTime;

namespace DistributedSystemsPatterns.Shared.EventStore.Subscriptions.MongoDB;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection
    AddMongoSubscriptionCursorsRepository(this IServiceCollection services, string collectionName) =>
    services.AddTransient<ISubscriptionCursorsRepository>(
      serviceProvider => new MongoSubscriptionCursorsRepository(
        serviceProvider.GetRequiredService<IMongoDatabase>(),
        serviceProvider.GetRequiredService<IClock>(),
        collectionName));
}
