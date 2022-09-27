using ConnorWyatt.EventSourcing.Aggregates;
using DistributedSystemsPatterns.SingleCurrentAggregate.Data;
using DistributedSystemsPatterns.SingleCurrentAggregate.Data.Models;
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
  private readonly IBillingPeriodsRepository _billingPeriodsRepository;
  private readonly IChargesRepository _chargesRepository;

  public BillingPeriodHandlers(
    AggregateRepository aggregateRepository,
    IBillingPeriodsRepository billingPeriodsRepository,
    IChargesRepository chargesRepository)
  {
    _aggregateRepository = aggregateRepository;
    _billingPeriodsRepository = billingPeriodsRepository;
    _chargesRepository = chargesRepository;
  }

  public async Task<Unit> Handle(OpenBillingPeriod request, CancellationToken cancellationToken)
  {
    var aggregate = await _aggregateRepository.LoadAggregate<BillingPeriod>(request.BillingPeriodId);

    aggregate.OpenBillingPeriod(request.UserId);

    await _aggregateRepository.SaveAggregate(aggregate);

    return Unit.Value;
  }

  public async Task<Unit> Handle(CloseBillingPeriod request, CancellationToken cancellationToken)
  {
    var aggregate = await _aggregateRepository.LoadAggregate<BillingPeriod>(request.BillingPeriodId);

    aggregate.CloseBillingPeriod();

    await _aggregateRepository.SaveAggregate(aggregate);

    return Unit.Value;
  }

  public async Task<Unit> Handle(AddCharge request, CancellationToken cancellationToken)
  {
    var billingPeriodId = await GetBillingPeriodId(request.UserId);

    var aggregate = await _aggregateRepository.LoadAggregate<BillingPeriod>(billingPeriodId);

    aggregate.AddCharge(request.ChargeId, request.Amount);

    await _aggregateRepository.SaveAggregate(aggregate);

    return Unit.Value;
  }

  public async Task<Unit> Handle(RemoveCharge request, CancellationToken cancellationToken)
  {
    var charge = await _chargesRepository.GetCharge(request.ChargeId);

    if (charge is null)
    {
      throw new InvalidOperationException("Charge could not be found.");
    }

    var aggregate = await _aggregateRepository.LoadAggregate<BillingPeriod>(charge.BillingPeriodId);

    aggregate.RemoveCharge(request.ChargeId);

    await _aggregateRepository.SaveAggregate(aggregate);

    return Unit.Value;
  }

  private async Task<string> GetBillingPeriodId(string userId)
  {
    var billingPeriods =
      await _billingPeriodsRepository.GetBillingPeriods(
        userId,
        BillingPeriodStatus.Open);

    return billingPeriods.Count switch
    {
      > 1 => throw new InvalidOperationException("Multiple open billing periods."),
      < 1 => throw new InvalidOperationException("No open billing periods."),
      _ => billingPeriods.Single().BillingPeriodId,
    };
  }
}
