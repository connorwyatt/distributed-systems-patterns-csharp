using System.Net.Http.Json;
using ConnorWyatt.EventSourcing;
using DistributedSystemsPatterns.CryptoShredding.Service;
using DistributedSystemsPatterns.CryptoShredding.Service.Customers.Controllers.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Polly;

namespace DistributedSystemsPatterns.CryptoShredding.IntegrationTests;

public class CryptoShreddingTests : IClassFixture<WebApplicationFactory<Startup>>
{
  private readonly HttpClient _client;

  public CryptoShreddingTests(WebApplicationFactory<Startup> factory) => _client = factory.CreateClient();

  [Fact]
  public async Task When_Adding_A_Customer__Then_The_Sensitive_Personal_Information_Is_Readable()
  {
    var customerId = await AddCustomer("Joe Bloggs", "Confidential information");

    var customer = await GetAndWaitForCustomer(customerId);

    customer.SensitivePersonalInformation.Should().Be("Confidential information");
  }

  [Fact]
  public async Task
    Given_A_Customer__When_Redacting_The_Sensitive_Personal_Information__Then_The_Sensitive_Personal_Data_Is_Redacted()
  {
    var customerId = await AddCustomer("Joe Bloggs", "Confidential information");

    await GetAndWaitForCustomer(customerId);

    await RedactCustomerSensitivePersonalInformation(customerId);

    var customer = await GetAndWaitForCustomer(customerId);

    customer.SensitivePersonalInformation.Should().Be("<REDACTED>");
  }

  private async Task<Customer> GetAndWaitForCustomer(string customerId)
  {
    var getResponseMessage = await RetryUntil(
      () => _client.GetAsync($"customers/{customerId}"),
      httpResponseMessage => httpResponseMessage.IsSuccessStatusCode);

    return await getResponseMessage.Content.ReadFromJsonAsync<Customer>(DefaultJsonSerializerOptions.Options) ??
      throw new InvalidOperationException("Could not deserialize to Customer.");
  }

  private async Task<string> AddCustomer(string name, string sensitivePersonalInformation)
  {
    var postResponseMessage = await _client.PostAsJsonAsync(
      "customers",
      new CustomerDefinition(name, sensitivePersonalInformation),
      DefaultJsonSerializerOptions.Options);

    postResponseMessage.IsSuccessStatusCode.Should().BeTrue();

    var reference =
      await postResponseMessage.Content.ReadFromJsonAsync<CustomerReference>(DefaultJsonSerializerOptions.Options) ??
      throw new InvalidOperationException("Could not deserialize to CustomerReference.");

    return reference.CustomerId;
  }

  private async Task RedactCustomerSensitivePersonalInformation(string customerId)
  {
    var postResponseMessage = await _client.PostAsJsonAsync(
      $"customers/{customerId}/actions/redact",
      new { },
      DefaultJsonSerializerOptions.Options);

    postResponseMessage.IsSuccessStatusCode.Should().BeTrue();
  }

  private static async Task<T> RetryUntil<T>(Func<Task<T>> action, Func<T, bool> retryUntilPredicate) =>
    await Policy.HandleResult<T>(result => !retryUntilPredicate.Invoke(result))
      .WaitAndRetryAsync(50, _ => TimeSpan.FromMilliseconds(100))
      .ExecuteAsync(action);
}
