using System.Reflection;
using DistributedSystemsPatterns.Shared.EventStore;
using DistributedSystemsPatterns.Shared.EventStore.Subscriptions.MongoDB;
using DistributedSystemsPatterns.Shared.MongoDB;
using MediatR;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;

namespace DistributedSystemsPatterns.SingleCurrentAggregate.Service;

public class Startup
{
  private readonly IConfiguration _configuration;

  public Startup(IConfiguration configuration) => _configuration = configuration;

  public void ConfigureServices(IServiceCollection services)
  {
    var executingAssembly = Assembly.GetExecutingAssembly();

    services.AddControllers()
      .AddJsonOptions(options => { options.JsonSerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb); });
    services.AddMediatR(executingAssembly);
    services.AddTransient<IClock>(_ => SystemClock.Instance);

    var eventStoreConfiguration = _configuration.GetRequiredSection("EventStore");
    var eventStoreClientOptions = GetEventStoreClientOptions(eventStoreConfiguration);

    var mongoDBConfiguration = _configuration.GetRequiredSection("MongoDB");
    var mongoDBOptions = GetMongoDBOptions(mongoDBConfiguration);

    services.AddEventStore(
        eventStoreClientOptions,
        new[] { GetType().Assembly, })
      .AddMongoSubscriptionCursorsRepository(
        mongoDBConfiguration.GetValue<string>("SubscriptionCursorsCollectionName"));

    services.AddMongoDB(mongoDBOptions);
  }

  public void Configure(WebApplication app)
  {
    app.UseHttpsRedirection();

    app.MapControllers();
  }

  private static EventStoreClientOptions GetEventStoreClientOptions(IConfigurationSection eventStoreConfiguration) =>
    new(eventStoreConfiguration.GetValue<string>("ConnectionString"));

  private static MongoDBOptions GetMongoDBOptions(IConfiguration configuration) =>
    new(
      configuration.GetValue<string>("ConnectionString"),
      configuration.GetValue<string>("DatabaseName"));
}
