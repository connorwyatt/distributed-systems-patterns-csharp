using DistributedSystemsPatterns.Shared.Ids;
using DistributedSystemsPatterns.UniqueConstraints.Mongo;
using DistributedSystemsPatterns.UniqueConstraints.Service.Users.Commands;
using DistributedSystemsPatterns.UniqueConstraints.Service.Users.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DistributedSystemsPatterns.UniqueConstraints.Service.Users.Controllers;

[ApiController]
[Route("users")]
public class UsersController : ControllerBase
{
  private readonly IMediator _mediator;
  private readonly MongoUsersRepository _usersRepository;

  public UsersController(IMediator mediator, MongoUsersRepository usersRepository)
  {
    _mediator = mediator;
    _usersRepository = usersRepository;
  }

  [HttpGet]
  [Route("")]
  public async Task<IActionResult> GetUsers()
  {
    var users = await _usersRepository.GetUsers();

    return Ok(users.Select(u => u.ToApiModel()));
  }

  [HttpGet]
  [Route("{userId}")]
  public async Task<IActionResult> GetUser([FromRoute] string userId)
  {
    var user = await _usersRepository.GetUser(userId);

    if (user == null)
    {
      return NotFound();
    }

    return Ok(user.ToApiModel());
  }

  [HttpPost]
  [Route("")]
  public async Task<IActionResult> AddUser([FromBody] UserDefinition userDefinition)
  {
    var id = HashId.NewHashId();

    await _mediator.Send(new AddUser(id, userDefinition.Name, userDefinition.EmailAddress));

    return Accepted(new UserReference(id));
  }
}
