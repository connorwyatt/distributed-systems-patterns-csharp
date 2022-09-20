using MediatR;

namespace DistributedSystemsPatterns.UniqueConstraints.Service.Users.Commands;

public record DeactivateUser(string UserId, string Reason) : IRequest;
