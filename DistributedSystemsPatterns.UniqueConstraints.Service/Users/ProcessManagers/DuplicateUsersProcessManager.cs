using DistributedSystemsPatterns.Shared.EventStore;
using DistributedSystemsPatterns.Shared.EventStore.Subscriptions;
using DistributedSystemsPatterns.UniqueConstraints.Mongo;
using DistributedSystemsPatterns.UniqueConstraints.Service.Users.Commands;
using DistributedSystemsPatterns.UniqueConstraints.Service.Users.Events;
using MediatR;

namespace DistributedSystemsPatterns.UniqueConstraints.Service.Users.ProcessManagers;

[SubscriberName("DuplicateUsersProcessManager")]
[Subscription("$ce-users")]
public class DuplicateUsersProcessManager : SubscriberBase
{
  private readonly IMediator _mediator;
  private readonly MongoDuplicateUserTrackingDataRepository _duplicateUserTrackingDataRepository;

  public DuplicateUsersProcessManager(
    IMediator mediator,
    MongoDuplicateUserTrackingDataRepository duplicateUserTrackingDataRepository)
  {
    _mediator = mediator;
    _duplicateUserTrackingDataRepository = duplicateUserTrackingDataRepository;

    When<UserAdded>(Handle);
  }

  private async Task Handle(UserAdded @event, EventMetadata metadata)
  {
    var alreadyExisted = await _duplicateUserTrackingDataRepository.HasUserWithEmailAddress(@event.EmailAddress);

    await _duplicateUserTrackingDataRepository.AddUser(
      new DuplicateUserTrackingDatum(@event.UserId, @event.EmailAddress));

    if (alreadyExisted)
    {
      /* Using a string reason here for simplicity. */
      await _mediator.Send(new DeactivateUser(@event.UserId, "There is already a user with this email address."));
    }
  }
}
