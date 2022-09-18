using DistributedSystemsPatterns.Shared.EventStore;

namespace DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.Events;

[Event("distributedSystemsPatterns.singleCurrentAggregate.billingPeriodEnded.v1")]
public class BillingPeriodEnded : IEvent
{
  public string BillingPeriodId { get; }

  public BillingPeriodEnded(string billingPeriodId) => BillingPeriodId = billingPeriodId;
}
