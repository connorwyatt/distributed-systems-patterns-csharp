using Microsoft.Extensions.DependencyInjection;
using MongoDb.Bson.NodaTime;
using MongoDB.Driver;

namespace DistributedSystemsPatterns.Shared.MongoDB;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddMongoDB(this IServiceCollection services, MongoDBOptions mongoDBOptions)
  {
    NodaTimeSerializers.Register();
    return services
      .AddSingleton<IMongoClient>(_ => new MongoClient(mongoDBOptions.ConnectionString))
      .AddSingleton<IMongoDatabase>(serviceProvider => serviceProvider.GetRequiredService<IMongoClient>().GetDatabase(mongoDBOptions.DatabaseName));
  }
}
