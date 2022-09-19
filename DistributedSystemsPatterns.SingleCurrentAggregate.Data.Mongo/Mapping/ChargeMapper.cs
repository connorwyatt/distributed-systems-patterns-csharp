using DistributedSystemsPatterns.SingleCurrentAggregate.Data.Models;

namespace DistributedSystemsPatterns.SingleCurrentAggregate.Data.Mongo.Mapping;

public static class ChargeMapper
{
  public static Charge ToDataModel(Models.Charge charge) =>
    new(
      charge.ChargeId,
      charge.BillingPeriodId,
      charge.UserId,
      ChargeStatusMapper.ToDataModel(charge.Status),
      charge.Amount,
      charge.AddedAt,
      charge.UpdatedAt,
      charge.Version);

  public static Models.Charge FromDataModel(Charge charge) =>
    new(
      charge.ChargeId,
      charge.BillingPeriodId,
      charge.UserId,
      ChargeStatusMapper.FromDataModel(charge.Status),
      charge.Amount,
      charge.AddedAt,
      charge.UpdatedAt,
      charge.Version);
}
