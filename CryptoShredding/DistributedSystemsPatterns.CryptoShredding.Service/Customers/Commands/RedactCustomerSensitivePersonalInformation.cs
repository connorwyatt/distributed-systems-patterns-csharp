using MediatR;

namespace DistributedSystemsPatterns.CryptoShredding.Service.Customers.Commands;

public record RedactCustomerSensitivePersonalInformation(string CustomerId) : IRequest;
