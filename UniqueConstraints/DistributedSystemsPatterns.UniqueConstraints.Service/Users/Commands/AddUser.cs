using MediatR;

namespace DistributedSystemsPatterns.UniqueConstraints.Service.Users.Commands;

public record AddUser(string UserId, string Name, string EmailAddress) : IRequest;
