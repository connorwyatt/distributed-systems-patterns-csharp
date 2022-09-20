using DistributedSystemsPatterns.Shared.EventStore.Aggregates;
using DistributedSystemsPatterns.UniqueConstraints.Service.Users.Commands;
using MediatR;

namespace DistributedSystemsPatterns.UniqueConstraints.Service.Users.Domain;

public class UserHandlers : IRequestHandler<AddUser>, IRequestHandler<DeactivateUser>
{
  private readonly AggregateRepository _aggregateRepository;

  public UserHandlers(AggregateRepository aggregateRepository) => _aggregateRepository = aggregateRepository;

  public async Task<Unit> Handle(AddUser command, CancellationToken cancellationToken)
  {
    var aggregate = await _aggregateRepository.LoadAggregate<User>(command.UserId);

    aggregate.AddUser(command.Name, command.EmailAddress);

    await _aggregateRepository.SaveAggregate(aggregate);

    return Unit.Value;
  }

  public async Task<Unit> Handle(DeactivateUser command, CancellationToken cancellationToken)
  {
    var aggregate = await _aggregateRepository.LoadAggregate<User>(command.UserId);

    aggregate.DeactivateUser(command.Reason);

    await _aggregateRepository.SaveAggregate(aggregate);

    return Unit.Value;
  }
}
