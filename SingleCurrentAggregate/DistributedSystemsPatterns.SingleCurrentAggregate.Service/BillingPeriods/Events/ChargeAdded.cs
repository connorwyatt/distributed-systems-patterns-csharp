using ConnorWyatt.EventSourcing;

namespace DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.Events;

[Event("DistributedSystemsPatterns.SingleCurrentAggregate.ChargeAdded.V1")]
public class ChargeAdded : IEvent
{
  public string BillingPeriodId { get; }

  public string UserId { get; }

  public string ChargeId { get; }

  public double Amount { get; }

  public double TotalAmount { get; }

  public ChargeAdded(string billingPeriodId, string userId, string chargeId, double amount, double totalAmount)
  {
    BillingPeriodId = billingPeriodId;
    UserId = userId;
    ChargeId = chargeId;
    Amount = amount;
    TotalAmount = totalAmount;
  }
}
