using DistributedSystemsPatterns.SingleCurrentAggregate.Data.Mongo.Mapping;
using DistributedSystemsPatterns.SingleCurrentAggregate.Data.Mongo.Models;
using MongoDB.Driver;
using BillingPeriodStatus = DistributedSystemsPatterns.SingleCurrentAggregate.Data.Models.BillingPeriodStatus;

namespace DistributedSystemsPatterns.SingleCurrentAggregate.Data.Mongo;

public class MongoBillingPeriodsRepository : IBillingPeriodsRepository
{
  private const string CollectionName = "billing_periods";

  private readonly IMongoCollection<BillingPeriod> _collection;

  public MongoBillingPeriodsRepository(IMongoDatabase database) =>
    _collection = database.GetCollection<BillingPeriod>(CollectionName);

  public async Task<Data.Models.BillingPeriod?> GetBillingPeriod(string billingPeriodId)
  {
    var billingPeriod = await _collection.Find(bp => bp.BillingPeriodId == billingPeriodId).SingleOrDefaultAsync();
    return billingPeriod is not null ? BillingPeriodMapper.ToDataModel(billingPeriod) : null;
  }

  public async Task<IList<Data.Models.BillingPeriod>> GetBillingPeriods(BillingPeriodStatus? status = null)
  {
    Models.BillingPeriodStatus? mappedStatus =
      status.HasValue ? BillingPeriodStatusMapper.FromDataModel(status.Value) : null;

    var filterDefinitions = new FilterDefinition<BillingPeriod>?[]
      {
        mappedStatus.HasValue
          ? new ExpressionFilterDefinition<BillingPeriod>(bp => bp.Status == mappedStatus.Value)
          : null,
      }
      .Where(x => x is not null)
      .OfType<FilterDefinition<BillingPeriod>>()
      .ToArray();

    var billingPeriodsQuery = _collection.Find(
      filterDefinitions.Any()
        ? new FilterDefinitionBuilder<BillingPeriod>().And(filterDefinitions)
        : FilterDefinition<BillingPeriod>.Empty);
    var billingPeriods = await billingPeriodsQuery.ToListAsync();
    return billingPeriods.Select(BillingPeriodMapper.ToDataModel).ToArray();
  }

  public async Task InsertBillingPeriod(Data.Models.BillingPeriod billingPeriod)
  {
    await _collection.InsertOneAsync(BillingPeriodMapper.FromDataModel(billingPeriod));
  }

  public async Task UpdateBillingPeriod(Data.Models.BillingPeriod billingPeriod)
  {
    await _collection.FindOneAndReplaceAsync(
      bp => bp.BillingPeriodId == billingPeriod.BillingPeriodId,
      BillingPeriodMapper.FromDataModel(billingPeriod));
  }
}
