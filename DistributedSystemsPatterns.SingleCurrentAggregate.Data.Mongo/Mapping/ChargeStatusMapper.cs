using DistributedSystemsPatterns.SingleCurrentAggregate.Data.Models;

namespace DistributedSystemsPatterns.SingleCurrentAggregate.Data.Mongo.Mapping;

public static class ChargeStatusMapper
{
  public static ChargeStatus ToDataModel(Models.ChargeStatus chargeStatus) =>
    chargeStatus switch
    {
      Models.ChargeStatus.Added => ChargeStatus.Added,
      Models.ChargeStatus.Removed => ChargeStatus.Removed,
      _ => throw new ArgumentOutOfRangeException(nameof(chargeStatus), chargeStatus, null),
    };

  public static Models.ChargeStatus FromDataModel(ChargeStatus chargeStatus) =>
    chargeStatus switch
    {
      ChargeStatus.Added => Models.ChargeStatus.Added,
      ChargeStatus.Removed => Models.ChargeStatus.Removed,
      _ => throw new ArgumentOutOfRangeException(nameof(chargeStatus), chargeStatus, null),
    };
}
