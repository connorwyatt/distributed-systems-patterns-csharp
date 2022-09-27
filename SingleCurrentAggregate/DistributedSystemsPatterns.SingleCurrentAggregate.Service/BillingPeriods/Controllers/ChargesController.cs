using DistributedSystemsPatterns.Shared.Ids;
using DistributedSystemsPatterns.SingleCurrentAggregate.Data;
using DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.Commands;
using DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.Controllers.Mapping;
using DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.Controllers.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.Controllers;

[ApiController]
[Route("charges")]
public class ChargesController : ControllerBase
{
  private readonly IMediator _mediator;
  private readonly IChargesRepository _chargesRepository;

  public ChargesController(IMediator mediator, IChargesRepository chargesRepository)
  {
    _mediator = mediator;
    _chargesRepository = chargesRepository;
  }

  [HttpGet]
  [Route("")]
  public async Task<IActionResult> GetCharges()
  {
    var charges = await _chargesRepository.GetCharges();

    return Ok(charges.Select(ChargeMapper.ToApiModel));
  }

  [HttpGet]
  [Route("{chargeId}")]
  public async Task<IActionResult> GetCharge([FromRoute] string chargeId)
  {
    var charge = await _chargesRepository.GetCharge(chargeId);

    if (charge is null)
    {
      return NotFound();
    }

    return Ok(ChargeMapper.ToApiModel(charge));
  }

  [HttpPost]
  [Route("")]
  public async Task<IActionResult> AddCharge([FromBody] ChargeDefinition definition)
  {
    var chargeId = HashId.NewHashId();

    await _mediator.Send(new AddCharge(chargeId, definition.UserId, definition.Amount, definition.Timestamp));

    return Accepted(new ChargeReference(chargeId));
  }

  [HttpPost]
  [Route("{chargeId}/actions/remove")]
  public async Task<IActionResult> RemoveCharge([FromRoute] string chargeId)
  {
    var charge = await _chargesRepository.GetCharge(chargeId);

    if (charge is null)
    {
      return NotFound();
    }

    await _mediator.Send(new RemoveCharge(chargeId));

    return Accepted();
  }
}
