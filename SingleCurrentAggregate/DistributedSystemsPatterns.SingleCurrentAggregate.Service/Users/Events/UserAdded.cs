using ConnorWyatt.EventSourcing;

namespace DistributedSystemsPatterns.SingleCurrentAggregate.Service.Users.Events;

[Event("DistributedSystemsPatterns.SingleCurrentAggregate.UserAdded.V1")]
public class UserAdded : IEvent
{
  public string UserId { get; }

  public UserAdded(string userId) => UserId = userId;
}
