using DistributedSystemsPatterns.Shared.EventStore.Aggregates;
using DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.Domain.Exceptions;
using DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.Events;

namespace DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.Domain;

[Category("billingperiods")]
public class BillingPeriod : Aggregate
{
  private bool _started;
  private bool _ended;

  private readonly Charges _charges = new();

  public BillingPeriod()
  {
    When<BillingPeriodStarted>(Apply);
    When<BillingPeriodEnded>(Apply);
    When<ChargeAdded>(Apply);
    When<ChargeRemoved>(Apply);
  }

  public void StartBillingPeriod(string userId)
  {
    if (_started)
    {
      throw new BillingPeriodAlreadyStartedException();
    }

    if (_ended)
    {
      throw new BillingPeriodEndedException();
    }

    RaiseEvent(new BillingPeriodStarted(Id, userId));
  }

  public void EndBillingPeriod()
  {
    if (_ended)
    {
      throw new BillingPeriodAlreadyEndedException();
    }

    RaiseEvent(new BillingPeriodEnded(Id));
  }

  public void AddCharge(string chargeId, double amount)
  {
    AssertBillingPeriodOpen();

    _charges.AssertDoesNotHaveCharge(chargeId);

    var totalAmount = _charges.TotalAmount() + amount;

    RaiseEvent(new ChargeAdded(Id, chargeId, amount, totalAmount));
  }

  public void RemoveCharge(string chargeId)
  {
    AssertBillingPeriodOpen();

    _charges.AssertHasCharge(chargeId);

    var totalAmount = _charges.TotalAmount() - _charges.GetChargeAmount(chargeId);

    RaiseEvent(new ChargeRemoved(Id, chargeId, totalAmount));
  }

  private void AssertBillingPeriodOpen()
  {
    if (!_started || _ended)
    {
      throw new BillingPeriodNotOpenException();
    }
  }

  private void Apply(BillingPeriodStarted @event)
  {
    _started = true;
  }

  private void Apply(BillingPeriodEnded @event)
  {
    _ended = true;
  }

  private void Apply(ChargeAdded @event)
  {
    _charges.AddCharge(@event.ChargeId, @event.Amount);
  }

  private void Apply(ChargeRemoved @event)
  {
    _charges.RemoveCharge(@event.ChargeId);
  }
}
