namespace DistributedSystemsPatterns.CryptoShredding.Service.Customers.Controllers.Models;

public record Customer(string CustomerId, string Name, string SensitivePersonalInformation);
