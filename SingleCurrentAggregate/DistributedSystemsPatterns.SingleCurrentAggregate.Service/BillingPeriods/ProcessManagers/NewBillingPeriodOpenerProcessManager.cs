using DistributedSystemsPatterns.Shared.EventStore;
using DistributedSystemsPatterns.Shared.EventStore.Subscriptions;
using DistributedSystemsPatterns.Shared.Ids;
using DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.Commands;
using DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.Events;
using DistributedSystemsPatterns.SingleCurrentAggregate.Service.Users.Events;
using MediatR;

namespace DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.ProcessManagers;

[SubscriberName("NewBillingPeriodOpenerProcessManager")]
[Subscription("$ce-singlecurrentaggregate.billingperiods")]
[Subscription("$ce-singlecurrentaggregate.users")]
public class NewBillingPeriodOpenerProcessManager : SubscriberBase
{
  private readonly IMediator _mediator;

  public NewBillingPeriodOpenerProcessManager(IMediator mediator)
  {
    _mediator = mediator;

    When<UserAdded>(Handle);
    When<BillingPeriodClosed>(Handle);
  }

  private async Task Handle(UserAdded @event, EventMetadata metadata)
  {
    await OpenBillingPeriod(@event.UserId);
  }

  private async Task Handle(BillingPeriodClosed @event, EventMetadata metadata)
  {
    await OpenBillingPeriod(@event.UserId);
  }

  private async Task OpenBillingPeriod(string userId) =>
    await _mediator.Send(new OpenBillingPeriod(HashId.NewHashId(), userId));
}
