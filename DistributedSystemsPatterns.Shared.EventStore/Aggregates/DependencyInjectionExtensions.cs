using Microsoft.Extensions.DependencyInjection;

namespace DistributedSystemsPatterns.Shared.EventStore.Aggregates;

internal static class DependencyInjectionExtensions
{
  public static IServiceCollection AddAggregateRepository(this IServiceCollection services) =>
    services.AddTransient<AggregateRepository>();
}
