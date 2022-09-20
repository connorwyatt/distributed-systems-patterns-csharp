namespace DistributedSystemsPatterns.Shared.EventStore;

[AttributeUsage(AttributeTargets.Class)]
public class EventAttribute : Attribute
{
  public string EventType { get; }

  public EventAttribute(string eventType) => EventType = eventType;
}
