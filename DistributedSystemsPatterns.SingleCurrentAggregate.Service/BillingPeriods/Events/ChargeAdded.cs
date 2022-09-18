using DistributedSystemsPatterns.Shared.EventStore;

namespace DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.Events;

[Event("distributedSystemsPatterns.singleCurrentAggregate.chargeAdded.v1")]
public class ChargeAdded : IEvent
{
  public string BillingPeriodId { get; }

  public string ChargeId { get; }

  public double Amount { get; }

  public double TotalAmount { get; }

  public ChargeAdded(string billingPeriodId, string chargeId, double amount, double totalAmount)
  {
    BillingPeriodId = billingPeriodId;
    ChargeId = chargeId;
    Amount = amount;
    TotalAmount = totalAmount;
  }
}
