using MediatR;

namespace DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.Commands;

public record OpenBillingPeriod(string BillingPeriodId, string UserId) : IRequest;
