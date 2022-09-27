using MediatR;
using NodaTime;

namespace DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.Commands;

public record AddCharge(string ChargeId, string UserId, double Amount, Instant Timestamp) : IRequest;
