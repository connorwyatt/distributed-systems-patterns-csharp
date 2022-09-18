using DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.Domain.Exceptions;

namespace DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.Domain;

public class Charges
{
  private readonly IDictionary<string, double> _charges = new Dictionary<string, double>();

  public bool HasCharge(string chargeId) => _charges.ContainsKey(chargeId);

  public double GetChargeAmount(string chargeId)
  {
    if (!_charges.TryGetValue(chargeId, out var amount))
    {
      throw new ChargeNotAddedException();
    }

    return amount;
  }

  public double TotalAmount() =>
    _charges.Values.Aggregate(0.0, (aggregatedAmount, amount) => aggregatedAmount + amount);

  public void AddCharge(string chargeId, double amount)
  {
    if (!_charges.TryAdd(chargeId, amount))
    {
      throw new ChargeAlreadyAddedException();
    }
  }

  public void RemoveCharge(string chargeId)
  {
    if (!_charges.Remove(chargeId))
    {
      throw new ChargeNotAddedException();
    }
  }

  public void AssertHasCharge(string chargeId)
  {
    if (!HasCharge(chargeId))
    {
      throw new ChargeNotAddedException();
    }
  }

  public void AssertDoesNotHaveCharge(string chargeId)
  {
    if (HasCharge(chargeId))
    {
      throw new ChargeAlreadyAddedException();
    }
  }
}
