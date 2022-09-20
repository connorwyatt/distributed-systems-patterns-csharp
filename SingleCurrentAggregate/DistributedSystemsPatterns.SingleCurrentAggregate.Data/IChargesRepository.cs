using DistributedSystemsPatterns.SingleCurrentAggregate.Data.Models;

namespace DistributedSystemsPatterns.SingleCurrentAggregate.Data;

public interface IChargesRepository
{
  Task<Charge?> GetCharge(string chargeId);

  Task<IList<Charge>> GetCharges();

  Task InsertCharge(Charge charge);

  Task UpdateCharge(Charge charge);
}
