using DistributedSystemsPatterns.Shared.EventStore;

namespace DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.Events;

[Event("distributedSystemsPatterns.singleCurrentAggregate.chargeRemoved.v1")]
public class ChargeRemoved : IEvent
{
  public string BillingPeriodId { get; }

  public string ChargeId { get; }

  public double TotalAmount { get; }

  public ChargeRemoved(string billingPeriodId, string chargeId, double totalAmount)
  {
    BillingPeriodId = billingPeriodId;
    ChargeId = chargeId;
    TotalAmount = totalAmount;
  }
}
