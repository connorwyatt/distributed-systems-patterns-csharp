using Microsoft.Extensions.DependencyInjection;

namespace DistributedSystemsPatterns.CryptoShredding.Data.Mongo;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddMongoRepositories(this IServiceCollection services) =>
    services
      .AddTransient<ICustomersRepository, MongoCustomersRepository>()
      .AddTransient<ICryptoKeysRepository, MongoCryptoKeysRepository>();
}
