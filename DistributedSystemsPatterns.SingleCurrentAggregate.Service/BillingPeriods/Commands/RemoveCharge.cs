using MediatR;

namespace DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.Commands;

public record RemoveCharge(string ChargeId) : IRequest;
