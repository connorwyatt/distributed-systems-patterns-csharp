namespace DistributedSystemsPatterns.UniqueConstraints.Service.Users.Models;

public static class UserExtensions
{
  public static User ToApiModel(this Mongo.User user) =>
    new(user.UserId, user.Status.ToApiModel(), user.Name, user.EmailAddress, user.JoinedAt);
}
