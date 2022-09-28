using ConnorWyatt.EventSourcing.Aggregates;
using DistributedSystemsPatterns.CryptoShredding.Service.Crypto;
using DistributedSystemsPatterns.CryptoShredding.Service.Customers.Commands;
using MediatR;

namespace DistributedSystemsPatterns.CryptoShredding.Service.Customers.Domain;

public class CustomerHandlers
  : IRequestHandler<AddCustomer>, IRequestHandler<RedactCustomerSensitivePersonalInformation>
{
  private readonly AggregateRepository _aggregateRepository;
  private readonly CryptoService _cryptoService;

  public CustomerHandlers(AggregateRepository aggregateRepository, CryptoService cryptoService)
  {
    _aggregateRepository = aggregateRepository;
    _cryptoService = cryptoService;
  }

  public async Task<Unit> Handle(AddCustomer request, CancellationToken cancellationToken)
  {
    var hashedSensitivePersonalInformation = await _cryptoService.Encrypt(request.SensitivePersonalInformation);

    var aggregate = await _aggregateRepository.LoadAggregate<Customer>(request.CustomerId);

    aggregate.AddCustomer(request.Name, hashedSensitivePersonalInformation);

    await _aggregateRepository.SaveAggregate(aggregate);

    return Unit.Value;
  }

  public async Task<Unit> Handle(
    RedactCustomerSensitivePersonalInformation request,
    CancellationToken cancellationToken)
  {
    var aggregate = await _aggregateRepository.LoadAggregate<Customer>(request.CustomerId);

    foreach (var sensitivePersonalInformation in aggregate.SensitivePersonalInformation)
    {
      await _cryptoService.Redact(sensitivePersonalInformation);
    }

    return Unit.Value;
  }
}
