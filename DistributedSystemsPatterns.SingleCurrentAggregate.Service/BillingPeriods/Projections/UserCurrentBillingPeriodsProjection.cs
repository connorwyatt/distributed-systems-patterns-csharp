using DistributedSystemsPatterns.Shared.EventStore;
using DistributedSystemsPatterns.Shared.EventStore.Subscriptions;
using DistributedSystemsPatterns.SingleCurrentAggregate.Data;
using DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.Events;

namespace DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.Projections;

[SubscriberName("UserCurrentBillingPeriodsProjection")]
[Subscription("$ce-singlecurrentaggregate.billingperiods")]
public class UserCurrentBillingPeriodsProjection : SubscriberBase
{
  private readonly IUserCurrentBillingPeriodsRepository _repository;

  public UserCurrentBillingPeriodsProjection(IUserCurrentBillingPeriodsRepository repository)
  {
    _repository = repository;

    When<BillingPeriodOpened>(Handle);
    When<BillingPeriodClosed>(Handle);
  }

  private async Task Handle(BillingPeriodOpened @event, EventMetadata metadata)
  {
    await _repository.UpsertUserCurrentBillingPeriod(@event.UserId, @event.BillingPeriodId);
  }

  private async Task Handle(BillingPeriodClosed @event, EventMetadata metadata)
  {
    await _repository.RemoveBillingPeriod(@event.BillingPeriodId);
  }
}
