using DistributedSystemsPatterns.Shared.EventStore.Aggregates;
using DistributedSystemsPatterns.UniqueConstraints.Service.Users.Events;

namespace DistributedSystemsPatterns.UniqueConstraints.Service.Users.Domain;

[Category("users")]
public class User : Aggregate
{
  private bool _added;
  private bool _deactivated;

  public User()
  {
    When<UserAdded>(Apply);
    When<UserDeactivated>(Apply);
  }

  public void AddUser(string name, string emailAddress)
  {
    if (_added)
    {
      return;
    }

    RaiseEvent(new UserAdded(Id, name, emailAddress));
  }

  public void DeactivateUser(string reason)
  {
    if (_deactivated)
    {
      return;
    }

    RaiseEvent(new UserDeactivated(Id, reason));
  }

  private void Apply(UserAdded @event)
  {
    _added = true;
  }

  private void Apply(UserDeactivated @event)
  {
    _deactivated = true;
  }
}
