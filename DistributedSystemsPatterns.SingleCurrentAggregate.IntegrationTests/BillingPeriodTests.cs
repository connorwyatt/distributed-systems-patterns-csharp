using System.Net.Http.Json;
using ConnorWyatt.EventSourcing;
using DistributedSystemsPatterns.SingleCurrentAggregate.Service;
using DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.Controllers.Models;
using DistributedSystemsPatterns.SingleCurrentAggregate.Service.Users.Models;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc.Testing;
using NodaTime;
using Polly;
using Xunit;

namespace DistributedSystemsPatterns.SingleCurrentAggregate.IntegrationTests;

public class BillingPeriodTests : IClassFixture<WebApplicationFactory<Startup>>
{
  private readonly HttpClient _client;

  public BillingPeriodTests(WebApplicationFactory<Startup> factory) => _client = factory.CreateClient();

  [Fact]
  public async Task When_Adding_A_User__Then_A_Billing_Period_Should_Be_Opened()
  {
    var testStartTime = SystemClock.Instance.GetCurrentInstant();

    var userId = await AddUser();

    var billingPeriod = await GetOrWaitForSingleOpenBillingPeriod(userId);

    var testEndTime = SystemClock.Instance.GetCurrentInstant();

    using (new AssertionScope())
    {
      billingPeriod.BillingPeriodId.Should().NotBeNull();
      billingPeriod.UserId.Should().Be(userId);
      billingPeriod.Status.Should().Be(BillingPeriodStatus.Open);
      billingPeriod.TotalAmount.Should().Be(0);
      billingPeriod.UpdatedAt.Should().BeInRange(testStartTime, testEndTime);
      billingPeriod.OpenedAt.Should().Be(billingPeriod.UpdatedAt);
      billingPeriod.ClosedAt.Should().BeNull();
    }
  }

  [Fact]
  public async Task Given_A_Billing_Period__When_Adding_A_Charge__Then_It_Should_Be_Added_To_The_Billing_Period()
  {
    var userId = await AddUser();

    await GetOrWaitForSingleOpenBillingPeriod(userId);

    var testStartTime = SystemClock.Instance.GetCurrentInstant();

    var chargeId = await AddCharge(userId, 50);

    var charge = await GetAndWaitForCharge(chargeId);

    var billingPeriod = await RetryUntil(
      () => GetOrWaitForSingleOpenBillingPeriod(userId),
      bp => bp.UpdatedAt >= testStartTime);

    var testEndTime = SystemClock.Instance.GetCurrentInstant();

    using (new AssertionScope())
    {
      billingPeriod.TotalAmount.Should().Be(50);
      billingPeriod.UpdatedAt.Should().BeInRange(testStartTime, testEndTime);
      charge.ChargeId.Should().Be(chargeId);
      charge.BillingPeriodId.Should().Be(billingPeriod.BillingPeriodId);
      charge.UserId.Should().Be(userId);
      charge.Status.Should().Be(ChargeStatus.Added);
      charge.Amount.Should().Be(50);
      charge.AddedAt.Should().BeInRange(testStartTime, testEndTime);
      charge.UpdatedAt.Should().Be(charge.AddedAt);
    }
  }

  [Fact]
  public async Task
    Given_An_Open_Billing_Period__When_Closing_The_Billing_Period__Then_It_A_New_Billing_Period_Is_Opened()
  {
    var userId = await AddUser();

    var firstBillingPeriod = await GetOrWaitForSingleOpenBillingPeriod(userId);

    await GetBillingPeriod(firstBillingPeriod.BillingPeriodId);

    var testStartTime = SystemClock.Instance.GetCurrentInstant();

    await CloseBillingPeriod(firstBillingPeriod.BillingPeriodId);

    firstBillingPeriod = await RetryUntil(
      () => GetBillingPeriod(firstBillingPeriod.BillingPeriodId),
      bp => bp.Status == BillingPeriodStatus.Closed);

    var secondBillingPeriod = await RetryUntil(
      () => GetOrWaitForSingleOpenBillingPeriod(userId),
      bp => bp.BillingPeriodId == firstBillingPeriod.BillingPeriodId);

    var testEndTime = SystemClock.Instance.GetCurrentInstant();

    firstBillingPeriod.ClosedAt.Should().NotBeNull();
    var firstBillingPeriodClosedAt = firstBillingPeriod.ClosedAt!.Value;

    using (new AssertionScope())
    {
      firstBillingPeriod.BillingPeriodId.Should().NotBeNull();
      firstBillingPeriod.UserId.Should().Be(userId);
      firstBillingPeriod.Status.Should().Be(BillingPeriodStatus.Closed);
      firstBillingPeriod.TotalAmount.Should().Be(0);
      firstBillingPeriodClosedAt.Should().BeInRange(testStartTime, testEndTime);
      firstBillingPeriod.UpdatedAt.Should().Be(firstBillingPeriodClosedAt);

      secondBillingPeriod.BillingPeriodId.Should().NotBeNull();
      secondBillingPeriod.UserId.Should().Be(userId);
      secondBillingPeriod.Status.Should().Be(BillingPeriodStatus.Open);
      secondBillingPeriod.TotalAmount.Should().Be(0);
      secondBillingPeriod.UpdatedAt.Should().BeInRange(testStartTime, testEndTime);
      secondBillingPeriod.OpenedAt.Should().Be(secondBillingPeriod.UpdatedAt);
      secondBillingPeriod.ClosedAt.Should().BeNull();
    }
  }

  [Fact]
  public async Task
    Given_An_Open_Billing_Period_And_A_Closed_Billing_Period__When_Adding_A_Charge__Then_It_Should_Be_Added_To_The_Open_Billing_Period()
  {
    var userId = await AddUser();

    var firstBillingPeriod = await GetOrWaitForSingleOpenBillingPeriod(userId);

    await CloseBillingPeriod(firstBillingPeriod.BillingPeriodId);

    var secondBillingPeriod = await RetryUntil(
      () => GetOrWaitForSingleOpenBillingPeriod(userId),
      bp => bp.BillingPeriodId != firstBillingPeriod.BillingPeriodId);

    var testStartTime = SystemClock.Instance.GetCurrentInstant();

    var chargeId = await AddCharge(userId, 50);

    var charge = await GetAndWaitForCharge(chargeId);

    secondBillingPeriod = await RetryUntil(
      () => GetBillingPeriod(secondBillingPeriod.BillingPeriodId),
      bp => bp.UpdatedAt >= testStartTime);

    var testEndTime = SystemClock.Instance.GetCurrentInstant();

    using (new AssertionScope())
    {
      secondBillingPeriod.TotalAmount.Should().Be(50);
      secondBillingPeriod.UpdatedAt.Should().BeInRange(testStartTime, testEndTime);
      charge.ChargeId.Should().Be(chargeId);
      charge.BillingPeriodId.Should().Be(secondBillingPeriod.BillingPeriodId);
      charge.UserId.Should().Be(userId);
      charge.Status.Should().Be(ChargeStatus.Added);
      charge.Amount.Should().Be(50);
      charge.AddedAt.Should().BeInRange(testStartTime, testEndTime);
      charge.UpdatedAt.Should().Be(charge.AddedAt);
    }
  }

  [Fact]
  public async Task Given_A_Charge__When_Removing_The_Charge__Then_It_Should_Be_Removed_From_The_Billing_Period()
  {
    var userId = await AddUser();

    await GetOrWaitForSingleOpenBillingPeriod(userId);

    var chargeId = await AddCharge(userId, 50);

    await GetAndWaitForCharge(chargeId);

    var testStartTime = SystemClock.Instance.GetCurrentInstant();

    await RemoveCharge(chargeId);

    var charge = await RetryUntil(() => GetAndWaitForCharge(chargeId), charge => charge.UpdatedAt >= testStartTime);

    var billingPeriod = await RetryUntil(
      () => GetOrWaitForSingleOpenBillingPeriod(userId),
      bp => bp.UpdatedAt >= testStartTime);

    var testEndTime = SystemClock.Instance.GetCurrentInstant();

    using (new AssertionScope())
    {
      billingPeriod.TotalAmount.Should().Be(0);
      billingPeriod.UpdatedAt.Should().BeInRange(testStartTime, testEndTime);
      charge.ChargeId.Should().Be(chargeId);
      charge.BillingPeriodId.Should().Be(billingPeriod.BillingPeriodId);
      charge.UserId.Should().Be(userId);
      charge.Status.Should().Be(ChargeStatus.Removed);
      charge.Amount.Should().Be(50);
      charge.UpdatedAt.Should().BeInRange(testStartTime, testEndTime);
    }
  }

  private async Task<Charge> GetAndWaitForCharge(string chargeId)
  {
    var getResponseMessage = await RetryUntil(
      () => _client.GetAsync($"charges/{chargeId}"),
      httpResponseMessage => httpResponseMessage.IsSuccessStatusCode);

    return await getResponseMessage.Content.ReadFromJsonAsync<Charge>(DefaultJsonSerializerOptions.Options) ??
      throw new InvalidOperationException("Could not deserialize to Charge.");
  }

  private async Task<string> AddUser()
  {
    var postResponseMessage = await _client.PostAsJsonAsync(
      "users",
      new { },
      DefaultJsonSerializerOptions.Options);

    postResponseMessage.IsSuccessStatusCode.Should().BeTrue();

    var reference =
      await postResponseMessage.Content.ReadFromJsonAsync<UserReference>(DefaultJsonSerializerOptions.Options) ??
      throw new InvalidOperationException("Could not deserialize to UserReference.");

    return reference.UserId;
  }

  private async Task<BillingPeriod> GetBillingPeriod(string billingPeriodId)
  {
    var getResponseMessage = await _client.GetAsync($"billing-periods/{billingPeriodId}");

    getResponseMessage.IsSuccessStatusCode.Should().BeTrue();

    var billingPeriod =
      await getResponseMessage.Content.ReadFromJsonAsync<BillingPeriod>(DefaultJsonSerializerOptions.Options) ??
      throw new InvalidOperationException("Could not deserialize to BillingPeriod.");

    return billingPeriod;
  }

  private async Task<IList<BillingPeriod>> GetBillingPeriods(string userId, BillingPeriodStatus? status = null)
  {
    var getResponseMessage = await _client.GetAsync($"billing-periods?userId={userId}&status={status}");

    getResponseMessage.IsSuccessStatusCode.Should().BeTrue();

    var billingPeriods =
      await getResponseMessage.Content.ReadFromJsonAsync<IList<BillingPeriod>>(DefaultJsonSerializerOptions.Options) ??
      throw new InvalidOperationException("Could not deserialize to IList<BillingPeriod>.");

    return billingPeriods;
  }

  private async Task<BillingPeriod> GetOrWaitForSingleOpenBillingPeriod(string userId)
  {
    var billingPeriods = await RetryUntil(
      () => GetBillingPeriods(userId, BillingPeriodStatus.Open),
      billingPeriods => billingPeriods.Any());

    return billingPeriods.Single();
  }

  private async Task CloseBillingPeriod(string billingPeriodId)
  {
    var postResponseMessage = await _client.PostAsJsonAsync(
      $"billing-periods/{billingPeriodId}/actions/close",
      new { },
      DefaultJsonSerializerOptions.Options);

    postResponseMessage.IsSuccessStatusCode.Should().BeTrue();
  }

  private async Task<string> AddCharge(string userId, double amount)
  {
    var postResponseMessage = await _client.PostAsJsonAsync(
      "charges",
      new ChargeDefinition(userId, amount, SystemClock.Instance.GetCurrentInstant()),
      DefaultJsonSerializerOptions.Options);

    postResponseMessage.IsSuccessStatusCode.Should().BeTrue();

    var reference =
      await postResponseMessage.Content.ReadFromJsonAsync<ChargeReference>(DefaultJsonSerializerOptions.Options) ??
      throw new InvalidOperationException("Could not deserialize to ChargeReference.");

    return reference.ChargeId;
  }

  private async Task RemoveCharge(string chargeId)
  {
    var postResponseMessage = await _client.PostAsJsonAsync(
      $"charges/{chargeId}/actions/remove",
      new { },
      DefaultJsonSerializerOptions.Options);

    postResponseMessage.IsSuccessStatusCode.Should().BeTrue();
  }

  private static async Task<T> RetryUntil<T>(Func<Task<T>> action, Func<T, bool> retryUntilPredicate) =>
    await Policy.HandleResult<T>(result => !retryUntilPredicate.Invoke(result))
      .WaitAndRetryAsync(50, _ => TimeSpan.FromMilliseconds(100))
      .ExecuteAsync(action);
}
