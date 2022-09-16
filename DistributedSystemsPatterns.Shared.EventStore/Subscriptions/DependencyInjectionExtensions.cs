using Microsoft.Extensions.DependencyInjection;

namespace DistributedSystemsPatterns.Shared.EventStore.Subscriptions;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddSubscriber<T>(this IServiceCollection services) where T : class, ISubscriber =>
    services.AddTransient<ISubscriber, T>();

  internal static IServiceCollection AddSubscriptions(this IServiceCollection services) =>
    services.AddHostedService<SubscriptionsManager>();
}
