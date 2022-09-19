using DistributedSystemsPatterns.SingleCurrentAggregate.Data.Mongo.Mapping;
using DistributedSystemsPatterns.SingleCurrentAggregate.Data.Mongo.Models;
using MongoDB.Driver;

namespace DistributedSystemsPatterns.SingleCurrentAggregate.Data.Mongo;

public class MongoChargesRepository : IChargesRepository
{
  private const string CollectionName = "charges";

  private readonly IMongoCollection<Charge> _collection;

  public MongoChargesRepository(IMongoDatabase database) =>
    _collection = database.GetCollection<Charge>(CollectionName);

  public async Task<Data.Models.Charge?> GetCharge(string chargeId)
  {
    var charge = await _collection.Find(c => c.ChargeId == chargeId).SingleOrDefaultAsync();
    return charge is not null ? ChargeMapper.ToDataModel(charge) : null;
  }

  public async Task<IList<Data.Models.Charge>> GetCharges()
  {
    var chargesQuery = _collection.Find(FilterDefinition<Charge>.Empty);
    var charges = await chargesQuery.ToListAsync();
    return charges.Select(ChargeMapper.ToDataModel).ToArray();
  }

  public async Task InsertCharge(Data.Models.Charge charge)
  {
    await _collection.InsertOneAsync(ChargeMapper.FromDataModel(charge));
  }

  public async Task UpdateCharge(Data.Models.Charge charge)
  {
    await _collection.FindOneAndReplaceAsync(
      c => c.ChargeId == charge.ChargeId,
      ChargeMapper.FromDataModel(charge));
  }
}
