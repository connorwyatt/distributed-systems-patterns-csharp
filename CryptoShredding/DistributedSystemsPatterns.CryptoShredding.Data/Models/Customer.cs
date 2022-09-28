namespace DistributedSystemsPatterns.CryptoShredding.Data.Models;

public record Customer(
  string CustomerId,
  string Name,
  string SensitivePersonalInformation,
  ulong Version);
