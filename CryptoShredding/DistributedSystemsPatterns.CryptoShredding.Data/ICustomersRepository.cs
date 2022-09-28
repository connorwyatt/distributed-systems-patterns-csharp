using DistributedSystemsPatterns.CryptoShredding.Data.Models;

namespace DistributedSystemsPatterns.CryptoShredding.Data;

public interface ICustomersRepository
{
  Task<Customer?> GetCustomer(string customerId);

  Task<IList<Customer>> GetCustomers();

  Task InsertCustomer(Customer customer);
}
