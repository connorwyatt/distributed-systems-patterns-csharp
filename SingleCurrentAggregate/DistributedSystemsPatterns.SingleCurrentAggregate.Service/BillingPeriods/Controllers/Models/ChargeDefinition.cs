using NodaTime;

namespace DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.Controllers.Models;

public record ChargeDefinition(string UserId, double Amount, Instant Timestamp);
