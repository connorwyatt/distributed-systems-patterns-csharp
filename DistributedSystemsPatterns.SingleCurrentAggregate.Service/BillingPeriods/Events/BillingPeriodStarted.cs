using DistributedSystemsPatterns.Shared.EventStore;

namespace DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.Events;

[Event("distributedSystemsPatterns.singleCurrentAggregate.billingPeriodStarted.v1")]
public class BillingPeriodStarted : IEvent
{
  public string BillingPeriodId { get; }

  public string UserId { get; }

  public BillingPeriodStarted(string billingPeriodId, string userId)
  {
    BillingPeriodId = billingPeriodId;
    UserId = userId;
  }
}
