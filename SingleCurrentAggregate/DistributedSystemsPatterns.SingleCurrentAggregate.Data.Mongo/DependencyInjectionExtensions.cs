using Microsoft.Extensions.DependencyInjection;

namespace DistributedSystemsPatterns.SingleCurrentAggregate.Data.Mongo;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddMongoRepositories(this IServiceCollection services) =>
    services
      .AddTransient<IBillingPeriodsRepository, MongoBillingPeriodsRepository>()
      .AddTransient<IChargesRepository, MongoChargesRepository>()
      .AddTransient<IUserCurrentBillingPeriodsRepository, MongoUserCurrentBillingPeriodsRepository>();
}
