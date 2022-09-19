using MediatR;

namespace DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.Commands;

public record CloseBillingPeriod(string BillingPeriodId) : IRequest;
