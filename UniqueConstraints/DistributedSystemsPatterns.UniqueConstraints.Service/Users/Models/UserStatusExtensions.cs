namespace DistributedSystemsPatterns.UniqueConstraints.Service.Users.Models;

public static class UserStatusExtensions
{
  public static UserStatus ToApiModel(this Mongo.UserStatus status) =>
    status switch
    {
      Mongo.UserStatus.Active => UserStatus.Active,
      Mongo.UserStatus.Deactivated => UserStatus.Deactivated,
      _ => throw new ArgumentOutOfRangeException(nameof(status), status, null),
    };
}
