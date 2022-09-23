using ConnorWyatt.EventSourcing;
using ConnorWyatt.EventSourcing.Subscriptions;
using DistributedSystemsPatterns.SingleCurrentAggregate.Data;
using DistributedSystemsPatterns.SingleCurrentAggregate.Data.Models;
using DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.Events;

namespace DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.Projections;

[SubscriberName("BillingPeriodsProjection")]
[Subscription("$ce-singlecurrentaggregate.billingperiods")]
public class BillingPeriodsProjection : SubscriberBase
{
  private readonly IBillingPeriodsRepository _billingPeriodsRepository;

  public BillingPeriodsProjection(IBillingPeriodsRepository billingPeriodsRepository)
  {
    _billingPeriodsRepository = billingPeriodsRepository;

    When<BillingPeriodOpened>(Handle);
    When<BillingPeriodClosed>(Handle);
    When<ChargeAdded>(Handle);
    When<ChargeRemoved>(Handle);
  }

  private async Task Handle(BillingPeriodOpened @event, EventMetadata metadata)
  {
    var billingPeriod = await _billingPeriodsRepository.GetBillingPeriod(@event.BillingPeriodId);

    if (billingPeriod is not null)
    {
      return;
    }

    await _billingPeriodsRepository.InsertBillingPeriod(
      new BillingPeriod(
        @event.BillingPeriodId,
        @event.UserId,
        BillingPeriodStatus.Open,
        0,
        metadata.Timestamp,
        null,
        metadata.Timestamp,
        0));
  }

  private async Task Handle(BillingPeriodClosed @event, EventMetadata metadata)
  {
    var billingPeriod = await _billingPeriodsRepository.GetBillingPeriod(@event.BillingPeriodId);

    if (billingPeriod is null || !TryUpdateVersion(billingPeriod, metadata.StreamPosition, out billingPeriod))
    {
      return;
    }

    billingPeriod = billingPeriod with
    {
      Status = BillingPeriodStatus.Closed,
      ClosedAt = metadata.Timestamp,
      UpdatedAt = metadata.Timestamp,
    };

    await _billingPeriodsRepository.UpdateBillingPeriod(billingPeriod);
  }

  private async Task Handle(ChargeAdded @event, EventMetadata metadata)
  {
    var billingPeriod = await _billingPeriodsRepository.GetBillingPeriod(@event.BillingPeriodId);

    if (billingPeriod is null || !TryUpdateVersion(billingPeriod, metadata.StreamPosition, out billingPeriod))
    {
      return;
    }

    billingPeriod = billingPeriod with
    {
      TotalAmount = @event.TotalAmount,
      UpdatedAt = metadata.Timestamp,
    };

    await _billingPeriodsRepository.UpdateBillingPeriod(billingPeriod);
  }

  private async Task Handle(ChargeRemoved @event, EventMetadata metadata)
  {
    var billingPeriod = await _billingPeriodsRepository.GetBillingPeriod(@event.BillingPeriodId);

    if (billingPeriod is null || !TryUpdateVersion(billingPeriod, metadata.StreamPosition, out billingPeriod))
    {
      return;
    }

    billingPeriod = billingPeriod with
    {
      TotalAmount = @event.TotalAmount,
      UpdatedAt = metadata.Timestamp,
    };

    await _billingPeriodsRepository.UpdateBillingPeriod(billingPeriod);
  }

  private static bool TryUpdateVersion(
    BillingPeriod billingPeriod,
    ulong streamPosition,
    out BillingPeriod newBillingPeriod)
  {
    var expectedVersion = streamPosition - 1;
    if (billingPeriod.Version >= streamPosition)
    {
      newBillingPeriod = billingPeriod;
      return false;
    }

    if (billingPeriod.Version != expectedVersion)
    {
      throw new InvalidOperationException($"Version mismatch, expected {expectedVersion}, saw {billingPeriod.Version}");
    }

    newBillingPeriod = billingPeriod with
    {
      Version = streamPosition,
    };
    return true;
  }
}
