using ConnorWyatt.EventSourcing;
using ConnorWyatt.EventSourcing.Subscriptions;
using DistributedSystemsPatterns.CryptoShredding.Data;
using DistributedSystemsPatterns.CryptoShredding.Data.Models;
using DistributedSystemsPatterns.CryptoShredding.Service.Customers.Events;

namespace DistributedSystemsPatterns.CryptoShredding.Service.Customers.Projections;

[SubscriberName("CustomersProjection")]
[Subscription("$ce-cryptoshredding.customers")]
public class CustomersProjection : SubscriberBase
{
  private readonly ICustomersRepository _customersRepository;

  public CustomersProjection(ICustomersRepository customersRepository)
  {
    _customersRepository = customersRepository;

    When<CustomerAdded>(Handle);
  }

  private async Task Handle(CustomerAdded @event, EventMetadata metadata)
  {
    var customer = await _customersRepository.GetCustomer(@event.CustomerId);

    if (customer is not null)
    {
      return;
    }

    await _customersRepository.InsertCustomer(
      new Customer(@event.CustomerId, @event.Name, @event.SensitivePersonalInformation, metadata.StreamPosition));
  }
}
