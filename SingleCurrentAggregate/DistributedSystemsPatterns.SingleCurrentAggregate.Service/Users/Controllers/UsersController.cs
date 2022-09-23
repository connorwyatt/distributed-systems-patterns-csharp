using DistributedSystemsPatterns.Shared.Ids;
using DistributedSystemsPatterns.SingleCurrentAggregate.Service.Users.Events;
using DistributedSystemsPatterns.SingleCurrentAggregate.Service.Users.Models;
using EventStore.Client;
using Microsoft.AspNetCore.Mvc;
using EventStoreClient = ConnorWyatt.EventSourcing.EventStoreClient;

namespace DistributedSystemsPatterns.SingleCurrentAggregate.Service.Users.Controllers;

[ApiController]
public class UsersController : ControllerBase
{
  private readonly EventStoreClient _eventStoreClient;

  public UsersController(EventStoreClient eventStoreClient) => _eventStoreClient = eventStoreClient;

  [HttpPost]
  [Route("users")]
  public async Task<IActionResult> SimulateAddingUser()
  {
    var userId = HashId.NewHashId();

    await _eventStoreClient.AppendToStreamAsync(
      $"singlecurrentaggregate.users-{userId}",
      StreamRevision.None,
      new[] { new UserAdded(userId), });

    return Accepted(new UserReference(userId));
  }
}
