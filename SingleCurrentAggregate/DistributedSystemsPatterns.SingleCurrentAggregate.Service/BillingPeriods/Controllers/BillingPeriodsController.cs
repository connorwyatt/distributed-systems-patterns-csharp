using DistributedSystemsPatterns.SingleCurrentAggregate.Data;
using DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.Commands;
using DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.Controllers.Mapping;
using DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.Controllers.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.Controllers;

[ApiController]
[Route("billing-periods")]
public class BillingPeriodsController : ControllerBase
{
  private readonly IBillingPeriodsRepository _billingPeriodsRepository;
  private readonly IMediator _mediator;

  public BillingPeriodsController(IBillingPeriodsRepository billingPeriodsRepository, IMediator mediator)
  {
    _billingPeriodsRepository = billingPeriodsRepository;
    _mediator = mediator;
  }

  [HttpGet]
  [Route("")]
  public async Task<IActionResult> GetBillingPeriods([FromQuery] BillingPeriodStatus? status = null)
  {
    var billingPeriods =
      await _billingPeriodsRepository.GetBillingPeriods(
        status.HasValue ? BillingPeriodStatusMapper.FromApiModel(status.Value) : null);

    return Ok(billingPeriods.Select(BillingPeriodMapper.ToApiModel));
  }

  [HttpPost]
  [Route("{billingPeriodId}/actions/close")]
  public async Task<IActionResult> GetBillingPeriods([FromRoute] string billingPeriodId)
  {
    var billingPeriod = await _billingPeriodsRepository.GetBillingPeriod(billingPeriodId);

    if (billingPeriod is null)
    {
      return NotFound();
    }

    await _mediator.Send(new CloseBillingPeriod(billingPeriodId));

    return Accepted();
  }
}
