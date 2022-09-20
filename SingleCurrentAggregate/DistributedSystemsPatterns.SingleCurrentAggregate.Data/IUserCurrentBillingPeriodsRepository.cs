namespace DistributedSystemsPatterns.SingleCurrentAggregate.Data;

public interface IUserCurrentBillingPeriodsRepository
{
  Task<bool> UserHasCurrentBillingPeriod(string userId);

  Task<string?> GetUserCurrentBillingPeriod(string userId);

  Task UpsertUserCurrentBillingPeriod(string userId, string billingPeriodId);

  Task RemoveBillingPeriod(string billingPeriodId);
}
