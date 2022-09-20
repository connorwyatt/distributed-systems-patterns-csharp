using System.Reflection;

namespace DistributedSystemsPatterns.Shared.EventStore.Aggregates;

internal static class AggregateUtilities
{
  public static string GetStreamName<T>(string id) where T : Aggregate => GetStreamName(typeof(T), id);

  public static string GetStreamName(Type type, string id) => $"{GetCategory(type)}-{id}";

  public static string GetCategory<T>() where T : Aggregate => GetCategory(typeof(T));

  public static string GetCategory(Type type)
  {
    var categoryAttribute = type.GetCustomAttribute<CategoryAttribute>() ??
      throw new InvalidOperationException($"Missing \"CategoryAttribute\" on {type.Name}.");

    return categoryAttribute.Category;
  }
}
