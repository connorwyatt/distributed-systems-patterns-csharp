using System.Reflection;

namespace DistributedSystemsPatterns.Shared.EventStore.Subscriptions;

public static class SubscriberUtilities
{
  public static IEnumerable<SubscriptionAttribute> GetSubscriptionAttributes<T>() where T : class =>
    GetSubscriptionAttributes(typeof(T));

  public static IEnumerable<SubscriptionAttribute> GetSubscriptionAttributes(Type type) =>
    type.GetCustomAttributes<SubscriptionAttribute>();

  public static string GetSubscriberName<T>() => GetSubscriberName(typeof(T));

  public static string GetSubscriberName(Type type) =>
    (type.GetCustomAttribute<SubscriberNameAttribute>() ??
      throw new InvalidOperationException($"Missing \"SubscriptionAttribute\" on {type.Name}.")).Value;
}
