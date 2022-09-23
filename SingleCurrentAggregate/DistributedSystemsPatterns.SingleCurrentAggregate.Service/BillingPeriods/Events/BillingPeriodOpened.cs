using ConnorWyatt.EventSourcing;

namespace DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.Events;

[Event("DistributedSystemsPatterns.SingleCurrentAggregate.BillingPeriodOpened.V1")]
public class BillingPeriodOpened : IEvent
{
  public string BillingPeriodId { get; }

  public string UserId { get; }

  public BillingPeriodOpened(string billingPeriodId, string userId)
  {
    BillingPeriodId = billingPeriodId;
    UserId = userId;
  }
}
