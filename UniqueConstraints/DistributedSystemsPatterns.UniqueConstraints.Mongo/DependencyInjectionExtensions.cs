using Microsoft.Extensions.DependencyInjection;

namespace DistributedSystemsPatterns.UniqueConstraints.Mongo;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddMongoUsersRepository(this IServiceCollection services) =>
    services.AddTransient<MongoUsersRepository>();

  public static IServiceCollection AddMongoDuplicateUserTrackingDataRepository(this IServiceCollection services) =>
    services.AddTransient<MongoDuplicateUserTrackingDataRepository>();
}
