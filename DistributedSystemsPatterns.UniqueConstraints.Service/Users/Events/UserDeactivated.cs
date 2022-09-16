using DistributedSystemsPatterns.Shared.EventStore;

namespace DistributedSystemsPatterns.UniqueConstraints.Service.Users.Events;

[Event("distributedSystemsPatterns.uniqueConstraints.userDeactivated.v1")]
public class UserDeactivated : IEvent
{
  public string UserId { get; }

  public string Reason { get; }

  public UserDeactivated(string userId, string reason)
  {
    UserId = userId;
    Reason = reason;
  }
}
