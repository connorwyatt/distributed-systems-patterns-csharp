using ConnorWyatt.EventSourcing.Aggregates;
using DistributedSystemsPatterns.CryptoShredding.Service.Customers.Domain.Exceptions;
using DistributedSystemsPatterns.CryptoShredding.Service.Customers.Events;

namespace DistributedSystemsPatterns.CryptoShredding.Service.Customers.Domain;

[Category("cryptoshredding.customers")]
public class Customer : Aggregate
{
  public IList<string> SensitivePersonalInformation = new List<string>();

  private bool _added;

  public Customer()
  {
    When<CustomerAdded>(Apply);
  }

  public void AddCustomer(string name, string sensitivePersonalInformation)
  {
    if (_added)
    {
      throw new CustomerAlreadyAddedException();
    }

    RaiseEvent(new CustomerAdded(Id, name, sensitivePersonalInformation));
  }

  private void Apply(CustomerAdded @event)
  {
    _added = true;
    SensitivePersonalInformation.Add(@event.SensitivePersonalInformation);
  }
}
