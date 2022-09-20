using HashidsNet;

namespace DistributedSystemsPatterns.Shared.Ids;

public static class HashId
{
  private static readonly Hashids Hashids = new("DistributedSystemsPatterns");

  public static string NewHashId() => Hashids.EncodeHex(Guid.NewGuid().ToString("N"));
}
