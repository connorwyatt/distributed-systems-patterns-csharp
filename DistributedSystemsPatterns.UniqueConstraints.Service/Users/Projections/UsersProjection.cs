using DistributedSystemsPatterns.Shared.EventStore;
using DistributedSystemsPatterns.Shared.EventStore.Subscriptions;
using DistributedSystemsPatterns.UniqueConstraints.Mongo;
using DistributedSystemsPatterns.UniqueConstraints.Service.Users.Events;

namespace DistributedSystemsPatterns.UniqueConstraints.Service.Users.Projections;

[SubscriberName("UsersProjection")]
[Subscription("$ce-users")]
public class UsersProjection : SubscriberBase
{
  private readonly MongoUsersRepository _usersRepository;

  public UsersProjection(MongoUsersRepository usersRepository)
  {
    _usersRepository = usersRepository;

    When<UserAdded>(AddUser);
    When<UserDeactivated>(DeactivateUser);
  }

  private async Task AddUser(UserAdded @event, EventMetadata metadata)
  {
    var user = await _usersRepository.GetUser(@event.UserId);

    if (user != null)
    {
      return;
    }

    await _usersRepository.InsertUser(
      new User(@event.UserId, UserStatus.Active, @event.Name, @event.EmailAddress, metadata.Timestamp, 0));
  }

  private async Task DeactivateUser(UserDeactivated @event, EventMetadata metadata)
  {
    var user = await _usersRepository.GetUser(@event.UserId);

    if (user == null || !TryUpdateVersion(user, metadata.StreamPosition, out user))
    {
      return;
    }

    user = user with
    {
      Status = UserStatus.Deactivated,
    };

    await _usersRepository.UpdateUser(user);
  }

  private static bool TryUpdateVersion(User user, ulong streamPosition, out User newUser)
  {
    var expectedVersion = streamPosition - 1;
    if (user.Version >= streamPosition)
    {
      newUser = user;
      return false;
    }

    if (user.Version != expectedVersion)
    {
      throw new InvalidOperationException($"Version mismatch, expected {expectedVersion}, saw {user.Version}");
    }

    newUser = user with
    {
      Version = streamPosition,
    };
    return true;
  }
}
