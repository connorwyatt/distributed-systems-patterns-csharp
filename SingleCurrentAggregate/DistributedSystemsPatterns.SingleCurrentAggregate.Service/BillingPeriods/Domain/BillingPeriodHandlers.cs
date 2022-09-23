using ConnorWyatt.EventSourcing.Aggregates;
using DistributedSystemsPatterns.SingleCurrentAggregate.Data;
using DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.Commands;
using MediatR;

namespace DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.Domain;

public class BillingPeriodHandlers
  : IRequestHandler<OpenBillingPeriod>,
    IRequestHandler<CloseBillingPeriod>,
    IRequestHandler<AddCharge>,
    IRequestHandler<RemoveCharge>
{
  private readonly AggregateRepository _aggregateRepository;
  private readonly IChargesRepository _chargesRepository;
  private readonly IUserCurrentBillingPeriodsRepository _userCurrentBillingPeriodsRepository;

  public BillingPeriodHandlers(
    AggregateRepository aggregateRepository,
    IChargesRepository chargesRepository,
    IUserCurrentBillingPeriodsRepository userCurrentBillingPeriodsRepository)
  {
    _aggregateRepository = aggregateRepository;
    _chargesRepository = chargesRepository;
    _userCurrentBillingPeriodsRepository = userCurrentBillingPeriodsRepository;
  }

  public async Task<Unit> Handle(OpenBillingPeriod request, CancellationToken cancellationToken)
  {
    var billingPeriod = await _aggregateRepository.LoadAggregate<BillingPeriod>(request.BillingPeriodId);

    billingPeriod.OpenBillingPeriod(request.UserId);

    await _aggregateRepository.SaveAggregate(billingPeriod);

    return Unit.Value;
  }

  public async Task<Unit> Handle(CloseBillingPeriod request, CancellationToken cancellationToken)
  {
    var billingPeriod = await _aggregateRepository.LoadAggregate<BillingPeriod>(request.BillingPeriodId);

    billingPeriod.CloseBillingPeriod();

    await _aggregateRepository.SaveAggregate(billingPeriod);

    return Unit.Value;
  }

  public async Task<Unit> Handle(AddCharge request, CancellationToken cancellationToken)
  {
    var currentBillingPeriodId = await _userCurrentBillingPeriodsRepository.GetUserCurrentBillingPeriod(request.UserId);

    if (currentBillingPeriodId is null)
    {
      throw new InvalidOperationException("No current billing period.");
    }

    var billingPeriod = await _aggregateRepository.LoadAggregate<BillingPeriod>(currentBillingPeriodId);

    billingPeriod.AddCharge(request.ChargeId, request.Amount);

    await _aggregateRepository.SaveAggregate(billingPeriod);

    return Unit.Value;
  }

  public async Task<Unit> Handle(RemoveCharge request, CancellationToken cancellationToken)
  {
    var charge = await _chargesRepository.GetCharge(request.ChargeId);

    if (charge is null)
    {
      throw new InvalidOperationException("Charge could not be found.");
    }

    var billingPeriod = await _aggregateRepository.LoadAggregate<BillingPeriod>(charge.BillingPeriodId);

    billingPeriod.RemoveCharge(request.ChargeId);

    await _aggregateRepository.SaveAggregate(billingPeriod);

    return Unit.Value;
  }
}
