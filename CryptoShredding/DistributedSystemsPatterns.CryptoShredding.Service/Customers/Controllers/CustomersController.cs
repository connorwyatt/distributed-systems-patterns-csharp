using DistributedSystemsPatterns.CryptoShredding.Data;
using DistributedSystemsPatterns.CryptoShredding.Service.Crypto;
using DistributedSystemsPatterns.CryptoShredding.Service.Customers.Commands;
using DistributedSystemsPatterns.CryptoShredding.Service.Customers.Controllers.Mapping;
using DistributedSystemsPatterns.CryptoShredding.Service.Customers.Controllers.Models;
using DistributedSystemsPatterns.Shared.Ids;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DistributedSystemsPatterns.CryptoShredding.Service.Customers.Controllers;

[ApiController]
[Route("customers")]
public class CustomersController : ControllerBase
{
  private readonly IMediator _mediator;
  private readonly ICustomersRepository _customersRepository;
  private readonly CryptoService _cryptoService;

  public CustomersController(IMediator mediator, ICustomersRepository customersRepository, CryptoService cryptoService)
  {
    _mediator = mediator;
    _customersRepository = customersRepository;
    _cryptoService = cryptoService;
  }

  [HttpGet]
  [Route("")]
  public async Task<IActionResult> GetCustomers()
  {
    var customers = await _customersRepository.GetCustomers();

    var mappingTasks = customers.Select(async customer => await CustomerMapper.ToApiModel(customer, _cryptoService))
      .ToArray();

    await Task.WhenAll(mappingTasks);

    return Ok(mappingTasks.Select(t => t.Result));
  }

  [HttpGet]
  [Route("{customerId}")]
  public async Task<IActionResult> GetCustomer([FromRoute] string customerId)
  {
    var customer = await _customersRepository.GetCustomer(customerId);

    if (customer is null)
    {
      return NotFound();
    }

    return Ok(await CustomerMapper.ToApiModel(customer, _cryptoService));
  }

  [HttpPost]
  [Route("")]
  public async Task<IActionResult> AddCustomer([FromBody] CustomerDefinition definition)
  {
    var customerId = HashId.NewHashId();

    await _mediator.Send(new AddCustomer(customerId, definition.Name, definition.SensitivePersonalInformation));

    return Accepted(new CustomerReference(customerId));
  }

  [HttpPost]
  [Route("{customerId}/actions/redact")]
  public async Task<IActionResult> RedactCustomerSensitivePersonalInformation([FromRoute] string customerId)
  {
    var customer = await _customersRepository.GetCustomer(customerId);

    if (customer is null)
    {
      return NotFound();
    }

    await _mediator.Send(new RedactCustomerSensitivePersonalInformation(customerId));

    return Accepted();
  }
}
