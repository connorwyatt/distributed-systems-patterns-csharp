using DistributedSystemsPatterns.Shared.EventStore;

namespace DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.Events;

[Event("DistributedSystemsPatterns.SingleCurrentAggregate.ChargeRemoved.V1")]
public class ChargeRemoved : IEvent
{
  public string BillingPeriodId { get; }

  public string UserId { get; }

  public string ChargeId { get; }

  public double TotalAmount { get; }

  public ChargeRemoved(string billingPeriodId, string userId, string chargeId, double totalAmount)
  {
    BillingPeriodId = billingPeriodId;
    UserId = userId;
    ChargeId = chargeId;
    TotalAmount = totalAmount;
  }
}
