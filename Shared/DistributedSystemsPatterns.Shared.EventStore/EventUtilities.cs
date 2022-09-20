using System.Reflection;

namespace DistributedSystemsPatterns.Shared.EventStore;

internal static class EventUtilities
{
  public static string GetType<T>() where T : class => GetType(typeof(T));

  public static string GetType(Type type)
  {
    var eventAttribute = type.GetCustomAttribute<EventAttribute>() ??
      throw new InvalidOperationException($"Missing \"EventAttribute\" on {type.Name}.");

    return eventAttribute.EventType;
  }
}
