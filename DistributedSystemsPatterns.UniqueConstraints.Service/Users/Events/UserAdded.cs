using DistributedSystemsPatterns.Shared.EventStore;

namespace DistributedSystemsPatterns.UniqueConstraints.Service.Users.Events;

[Event("distributedSystemsPatterns.uniqueConstraints.userAdded.v1")]
public class UserAdded : IEvent
{
  public string UserId { get; }

  public string Name { get; }

  public string EmailAddress { get; }

  public UserAdded(string userId, string name, string emailAddress)
  {
    UserId = userId;
    Name = name;
    EmailAddress = emailAddress;
  }
}
