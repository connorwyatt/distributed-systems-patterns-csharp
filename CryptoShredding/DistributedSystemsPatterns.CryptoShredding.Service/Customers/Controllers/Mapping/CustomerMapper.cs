using DistributedSystemsPatterns.CryptoShredding.Service.Crypto;
using DistributedSystemsPatterns.CryptoShredding.Service.Customers.Controllers.Models;

namespace DistributedSystemsPatterns.CryptoShredding.Service.Customers.Controllers.Mapping;

public static class CustomerMapper
{
  public static async Task<Customer> ToApiModel(Data.Models.Customer charge, CryptoService cryptoService) =>
    new(
      charge.CustomerId,
      charge.Name,
      await cryptoService.Decrypt(charge.SensitivePersonalInformation));
}
