using MongoDB.Driver;

namespace DistributedSystemsPatterns.UniqueConstraints.Mongo;

public class MongoUsersRepository
{
  private const string CollectionName = "users";

  private readonly IMongoCollection<User> _collection;

  public MongoUsersRepository(IMongoDatabase database) => _collection = database.GetCollection<User>(CollectionName);

  public async Task<User?> GetUser(string userId)
  {
    return await _collection.Find(u => u.UserId == userId).SingleOrDefaultAsync();
  }

  public async Task<IList<User>> GetUsers()
  {
    return await _collection.Find(FilterDefinition<User>.Empty).SortByDescending(u => u.JoinedAt).ToListAsync();
  }

  public async Task InsertUser(User user)
  {
    await _collection.InsertOneAsync(user);
  }

  public async Task UpdateUser(User user)
  {
    await _collection.ReplaceOneAsync(u => u.UserId == user.UserId, user);
  }
}
