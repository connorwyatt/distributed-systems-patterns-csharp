using MediatR;

namespace DistributedSystemsPatterns.CryptoShredding.Service.Customers.Commands;

public record AddCustomer(string CustomerId, string Name, string SensitivePersonalInformation) : IRequest;
