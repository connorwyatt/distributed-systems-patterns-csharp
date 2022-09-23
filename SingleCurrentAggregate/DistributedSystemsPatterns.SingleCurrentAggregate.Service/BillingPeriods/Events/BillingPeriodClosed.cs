using ConnorWyatt.EventSourcing;

namespace DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.Events;

[Event("DistributedSystemsPatterns.SingleCurrentAggregate.BillingPeriodClosed.V1")]
public class BillingPeriodClosed : IEvent
{
  public string BillingPeriodId { get; }

  public string UserId { get; }

  public double TotalAmount { get; }

  public BillingPeriodClosed(string billingPeriodId, string userId, double totalAmount)
  {
    BillingPeriodId = billingPeriodId;
    UserId = userId;
    TotalAmount = totalAmount;
  }
}
