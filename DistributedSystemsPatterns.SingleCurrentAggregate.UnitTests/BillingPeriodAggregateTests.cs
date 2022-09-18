using DistributedSystemsPatterns.Shared.EventStore;
using DistributedSystemsPatterns.Shared.Ids;
using DistributedSystemsPatterns.Shared.TestUtils;
using DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.Domain;
using DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.Domain.Exceptions;
using DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.Events;

namespace DistributedSystemsPatterns.SingleCurrentAggregate.UnitTests;

public class BillingPeriodTests : AggregateTests<BillingPeriod>
{
  [Fact]
  public void When_Starting_A_BillingPeriod__Then_It_Should_Be_Started()
  {
    var userId = HashId.NewHashId();

    When(aggregate => { aggregate.StartBillingPeriod(userId); });

    Then(aggregateId => new IEvent[] { new BillingPeriodStarted(aggregateId, userId), });
  }

  [Fact]
  public void Given_A_Started_BillingPeriod__When_Starting_It_Again__Then_It_Should_Throw()
  {
    var userId = HashId.NewHashId();

    Given(aggregateId => new[] { Envelope(new BillingPeriodStarted(aggregateId, userId)), });

    When(aggregate => { aggregate.StartBillingPeriod(userId); });

    ThenThrows<BillingPeriodAlreadyStartedException>();
  }

  [Fact]
  public void Given_A_Started_BillingPeriod__When_Ending_It__Then_It_Be_Ended()
  {
    var userId = HashId.NewHashId();

    Given(aggregateId => new[] { Envelope(new BillingPeriodStarted(aggregateId, userId)), });

    When(aggregate => { aggregate.EndBillingPeriod(); });

    Then(aggregateId => new IEvent[] { new BillingPeriodEnded(aggregateId), });
  }

  [Fact]
  public void Given_A_Started_BillingPeriod__When_Adding_Charges__Then_They_Are_Added()
  {
    var userId = HashId.NewHashId();

    var chargeIds = Enumerable.Range(0, 2).Select(_ => HashId.NewHashId()).ToArray();

    Given(aggregateId => new[] { Envelope(new BillingPeriodStarted(aggregateId, userId)), });

    When(
      aggregate =>
      {
        foreach (var chargeId in chargeIds)
        {
          aggregate.AddCharge(chargeId, 5);
        }
      });

    Then(
      aggregateId => new IEvent[]
      {
        new ChargeAdded(aggregateId, chargeIds[0], 5, 5), new ChargeAdded(aggregateId, chargeIds[1], 5, 10),
      });
  }

  [Fact]
  public void Given_A_BillingPeriod_With_Charges__When_Removing_Charges__Then_They_Are_Removed()
  {
    var userId = HashId.NewHashId();

    var chargeIds = Enumerable.Range(0, 2).Select(_ => HashId.NewHashId()).ToArray();

    Given(
      aggregateId => new[]
      {
        Envelope(new BillingPeriodStarted(aggregateId, userId)),
        Envelope(new ChargeAdded(aggregateId, chargeIds[0], 5, 5)),
        Envelope(new ChargeAdded(aggregateId, chargeIds[1], 5, 10)),
      });

    When(
      aggregate =>
      {
        foreach (var chargeId in chargeIds)
        {
          aggregate.RemoveCharge(chargeId);
        }
      });

    Then(
      aggregateId => new IEvent[]
      {
        new ChargeRemoved(aggregateId, chargeIds[0], 5), new ChargeRemoved(aggregateId, chargeIds[1], 0),
      });
  }

  [Fact]
  public void Given_An_Ended_BillingPeriod__When_Adding_A_Charge__Then_It_Should_Throw()
  {
    var userId = HashId.NewHashId();

    Given(
      aggregateId => new[]
      {
        Envelope(new BillingPeriodStarted(aggregateId, userId)), Envelope(new BillingPeriodEnded(aggregateId)),
      });

    When(aggregate => { aggregate.AddCharge(HashId.NewHashId(), 5); });

    ThenThrows<BillingPeriodNotOpenException>();
  }

  [Fact]
  public void Given_An_Ended_BillingPeriod__When_Removing_A_Charge__Then_It_Should_Throw()
  {
    var userId = HashId.NewHashId();

    var chargeId = HashId.NewHashId();

    Given(
      aggregateId => new[]
      {
        Envelope(new BillingPeriodStarted(aggregateId, userId)),
        Envelope(new ChargeAdded(aggregateId, chargeId, 5, 5)),
        Envelope(new BillingPeriodEnded(aggregateId)),
      });

    When(aggregate => { aggregate.RemoveCharge(chargeId); });

    ThenThrows<BillingPeriodNotOpenException>();
  }
}
