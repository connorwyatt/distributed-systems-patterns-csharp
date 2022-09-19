using DistributedSystemsPatterns.SingleCurrentAggregate.Data.Models;

namespace DistributedSystemsPatterns.SingleCurrentAggregate.Data;

public interface IBillingPeriodsRepository
{
  Task<BillingPeriod?> GetBillingPeriod(string billingPeriodId);

  Task<IList<BillingPeriod>> GetBillingPeriods(BillingPeriodStatus? status = null);

  Task InsertBillingPeriod(BillingPeriod billingPeriod);

  Task UpdateBillingPeriod(BillingPeriod billingPeriod);
}
