using DistributedSystemsPatterns.SingleCurrentAggregate.Data.Mongo.Models;
using MongoDB.Driver;

namespace DistributedSystemsPatterns.SingleCurrentAggregate.Data.Mongo;

public class MongoUserCurrentBillingPeriodsRepository : IUserCurrentBillingPeriodsRepository
{
  private const string CollectionName = "user_current_billing_periods";

  private readonly IMongoCollection<UserCurrentBillingPeriod> _collection;

  public MongoUserCurrentBillingPeriodsRepository(IMongoDatabase database) =>
    _collection = database.GetCollection<UserCurrentBillingPeriod>(CollectionName);

  public async Task<bool> UserHasCurrentBillingPeriod(string userId) =>
    await GetUserCurrentBillingPeriod(userId) is not null;

  public async Task<string?> GetUserCurrentBillingPeriod(string userId) =>
    (await _collection.Find(u => u.UserId == userId).SingleOrDefaultAsync())?.BillingPeriodId;

  public async Task UpsertUserCurrentBillingPeriod(string userId, string billingPeriodId) =>
    await _collection.FindOneAndReplaceAsync<UserCurrentBillingPeriod>(
      u => u.UserId == userId,
      new UserCurrentBillingPeriod(userId, billingPeriodId),
      new FindOneAndReplaceOptions<UserCurrentBillingPeriod>
      {
        IsUpsert = true,
      });

  public async Task RemoveBillingPeriod(string billingPeriodId)
  {
    await _collection.FindOneAndDeleteAsync(u => u.BillingPeriodId == billingPeriodId);
  }
}
