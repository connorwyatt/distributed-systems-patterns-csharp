using DistributedSystemsPatterns.Shared.EventStore;
using DistributedSystemsPatterns.Shared.EventStore.Subscriptions;
using DistributedSystemsPatterns.SingleCurrentAggregate.Data;
using DistributedSystemsPatterns.SingleCurrentAggregate.Data.Models;
using DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.Events;

namespace DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.Projections;

[SubscriberName("ChargesProjection")]
[Subscription("$ce-singlecurrentaggregate.billingperiods")]
public class ChargesProjection : SubscriberBase
{
  private readonly IChargesRepository _chargesRepository;

  public ChargesProjection(IChargesRepository chargesRepository)
  {
    _chargesRepository = chargesRepository;

    When<ChargeAdded>(Handle);
    When<ChargeRemoved>(Handle);
  }

  private async Task Handle(ChargeAdded @event, EventMetadata metadata)
  {
    var charge = await _chargesRepository.GetCharge(@event.ChargeId);

    if (charge is not null)
    {
      return;
    }

    charge = new Charge(
      @event.ChargeId,
      @event.BillingPeriodId,
      @event.UserId,
      ChargeStatus.Added,
      @event.Amount,
      metadata.Timestamp,
      metadata.Timestamp,
      metadata.StreamPosition);

    await _chargesRepository.InsertCharge(charge);
  }

  private async Task Handle(ChargeRemoved @event, EventMetadata metadata)
  {
    var charge = await _chargesRepository.GetCharge(@event.ChargeId);

    if (charge is null || !TryUpdateVersion(charge, metadata.StreamPosition, out charge))
    {
      return;
    }

    charge = charge with
    {
      Status = ChargeStatus.Removed,
      UpdatedAt = metadata.Timestamp,
    };

    await _chargesRepository.UpdateCharge(charge);
  }

  private static bool TryUpdateVersion(
    Charge charge,
    ulong streamPosition,
    out Charge newCharge)
  {
    if (charge.Version >= streamPosition)
    {
      newCharge = charge;
      return false;
    }

    newCharge = charge with
    {
      Version = streamPosition,
    };
    return true;
  }
}
