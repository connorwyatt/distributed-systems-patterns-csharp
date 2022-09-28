using DistributedSystemsPatterns.CryptoShredding.Data.Mongo.Mapping;
using DistributedSystemsPatterns.CryptoShredding.Data.Mongo.Models;
using MongoDB.Driver;

namespace DistributedSystemsPatterns.CryptoShredding.Data.Mongo;

public class MongoCustomersRepository : ICustomersRepository
{
  private const string CollectionName = "customers";

  private readonly IMongoCollection<Customer> _collection;

  public MongoCustomersRepository(IMongoDatabase database) =>
    _collection = database.GetCollection<Customer>(CollectionName);

  public async Task<Data.Models.Customer?> GetCustomer(string customerId)
  {
    var customer = await _collection.Find(c => c.CustomerId == customerId).SingleOrDefaultAsync();
    return customer is not null ? CustomerMapper.ToDataModel(customer) : null;
  }

  public async Task<IList<Data.Models.Customer>> GetCustomers()
  {
    var customersQuery = _collection.Find(FilterDefinition<Customer>.Empty);
    var customers = await customersQuery.ToListAsync();
    return customers.Select(CustomerMapper.ToDataModel).ToArray();
  }

  public async Task InsertCustomer(Data.Models.Customer customer)
  {
    await _collection.InsertOneAsync(CustomerMapper.FromDataModel(customer));
  }
}
