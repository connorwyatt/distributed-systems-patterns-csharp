using DistributedSystemsPatterns.CryptoShredding.Data.Models;

namespace DistributedSystemsPatterns.CryptoShredding.Data.Mongo.Mapping;

public static class CustomerMapper
{
  public static Customer ToDataModel(Models.Customer customer) =>
    new(customer.CustomerId, customer.Name, customer.SensitivePersonalInformation, customer.Version);

  public static Models.Customer FromDataModel(Customer customer) =>
    new(customer.CustomerId, customer.Name, customer.SensitivePersonalInformation, customer.Version);
}
