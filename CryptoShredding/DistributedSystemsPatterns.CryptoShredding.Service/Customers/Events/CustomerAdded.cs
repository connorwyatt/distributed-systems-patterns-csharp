using ConnorWyatt.EventSourcing;

namespace DistributedSystemsPatterns.CryptoShredding.Service.Customers.Events;

[Event("DistributedSystemsPatterns.CryptoShredding.CustomerAdded.V1")]
public class CustomerAdded : IEvent
{
  public string CustomerId { get; }

  public string Name { get; }

  public string SensitivePersonalInformation { get; }

  public CustomerAdded(string customerId, string name, string sensitivePersonalInformation)
  {
    CustomerId = customerId;
    Name = name;
    SensitivePersonalInformation = sensitivePersonalInformation;
  }
}
