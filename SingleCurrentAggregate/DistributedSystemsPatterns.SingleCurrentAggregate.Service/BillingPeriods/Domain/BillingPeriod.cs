using ConnorWyatt.EventSourcing.Aggregates;
using DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.Domain.Exceptions;
using DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.Events;

namespace DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.Domain;

[Category("singlecurrentaggregate.billingperiods")]
public class BillingPeriod : Aggregate
{
  private bool _started;
  private bool _closed;

  private string? _userId;

  private readonly Charges _charges = new();

  public BillingPeriod()
  {
    When<BillingPeriodOpened>(Apply);
    When<BillingPeriodClosed>(Apply);
    When<ChargeAdded>(Apply);
    When<ChargeRemoved>(Apply);
  }

  public void OpenBillingPeriod(string userId)
  {
    if (_started)
    {
      throw new BillingPeriodAlreadyOpenedException();
    }

    if (_closed)
    {
      throw new BillingPeriodClosedException();
    }

    RaiseEvent(new BillingPeriodOpened(Id, userId));
  }

  public void CloseBillingPeriod()
  {
    if (_closed)
    {
      throw new BillingPeriodAlreadyClosedException();
    }

    RaiseEvent(new BillingPeriodClosed(Id, GetUserIdOrThrow(), _charges.TotalAmount()));
  }

  public void AddCharge(string chargeId, double amount)
  {
    AssertBillingPeriodOpen();

    _charges.AssertDoesNotHaveCharge(chargeId);

    var totalAmount = _charges.TotalAmount() + amount;

    RaiseEvent(new ChargeAdded(Id, GetUserIdOrThrow(), chargeId, amount, totalAmount));
  }

  public void RemoveCharge(string chargeId)
  {
    AssertBillingPeriodOpen();

    _charges.AssertHasCharge(chargeId);

    var totalAmount = _charges.TotalAmount() - _charges.GetChargeAmount(chargeId);

    RaiseEvent(new ChargeRemoved(Id, GetUserIdOrThrow(), chargeId, totalAmount));
  }

  private void AssertBillingPeriodOpen()
  {
    if (!_started || _closed)
    {
      throw new BillingPeriodNotOpenException();
    }
  }

  private string GetUserIdOrThrow() => _userId ?? throw new InvalidOperationException("UserId is null.");

  private void Apply(BillingPeriodOpened @event)
  {
    _started = true;
    _userId = @event.UserId;
  }

  private void Apply(BillingPeriodClosed @event)
  {
    _closed = true;
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
