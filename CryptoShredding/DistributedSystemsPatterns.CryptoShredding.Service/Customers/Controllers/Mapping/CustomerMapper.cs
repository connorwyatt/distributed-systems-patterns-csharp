using DistributedSystemsPatterns.CryptoShredding.Service.Crypto;
using DistributedSystemsPatterns.CryptoShredding.Service.Customers.Controllers.Models;

namespace DistributedSystemsPatterns.CryptoShredding.Service.Customers.Controllers.Mapping;

public static class CustomerMapper
{
  public static async Task<Customer> ToApiModel(Data.Models.Customer customer, CryptoService cryptoService) =>
    new(
      customer.CustomerId,
      customer.Name,
      await cryptoService.Decrypt(customer.CustomerId, customer.SensitivePersonalInformation));
}
