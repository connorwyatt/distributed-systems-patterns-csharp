using System.Net.Http.Json;
using DistributedSystemsPatterns.Shared.Ids;
using DistributedSystemsPatterns.Shared.Serialization;
using DistributedSystemsPatterns.UniqueConstraints.Service;
using DistributedSystemsPatterns.UniqueConstraints.Service.Users.Models;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc.Testing;
using NodaTime;
using Polly;
using Polly.Retry;

namespace DistributedSystemsPatterns.UniqueConstraints.IntegrationTests;

public class UsersTests : IClassFixture<WebApplicationFactory<Startup>>
{
  private static readonly AsyncRetryPolicy<HttpResponseMessage> RetryPolicy =
    Policy.HandleResult<HttpResponseMessage>(httpResponseMessage => !httpResponseMessage.IsSuccessStatusCode)
      .WaitAndRetryAsync(50, _ => TimeSpan.FromMilliseconds(100));

  private readonly HttpClient _client;

  public UsersTests(WebApplicationFactory<Startup> factory) => _client = factory.CreateClient();

  [Fact]
  public async Task When_Adding_A_User__Then_It_Should_Be_Active()
  {
    var testStartTime = SystemClock.Instance.GetCurrentInstant();

    const string name = "Joe Bloggs";
    var emailAddress = $"joe+{HashId.NewHashId()}@example.com";

    var userDefinition = new UserDefinition(name, emailAddress);

    var userId = await AddUser(userDefinition);

    var user = await GetUser(userId);

    var testEndTime = SystemClock.Instance.GetCurrentInstant();

    using (new AssertionScope())
    {
      user.UserId.Should().Be(userId);
      user.Status.Should().Be(UserStatus.Active);
      user.Name.Should().Be(name);
      user.EmailAddress.Should().Be(emailAddress);
      user.JoinedAt.Should().BeInRange(testStartTime, testEndTime);
    }
  }

  [Fact]
  public async Task Given_A_User__When_Adding_A_User_With_The_Same_EmailAddress__Then_It_Should_Be_Deactivated()
  {
    var userDefinition = new UserDefinition("Joe Bloggs", $"joe+{HashId.NewHashId()}@example.com");

    var firstUserId = await AddUser(userDefinition);
    var secondUserId = await AddUser(userDefinition);

    var deactivatedUserRetryPolicy = Policy.HandleResult<User>(user => user.Status == UserStatus.Deactivated)
      .WaitAndRetryAsync(50, _ => TimeSpan.FromMilliseconds(100));

    var firstUser = await GetUser(firstUserId);
    var secondUser = await deactivatedUserRetryPolicy.ExecuteAsync(() => GetUser(secondUserId));

    using (new AssertionScope())
    {
      firstUser.Status.Should().Be(UserStatus.Active);
      secondUser.Status.Should().Be(UserStatus.Deactivated);
    }
  }

  private async Task<User> GetUser(string userId)
  {
    var getResponseMessage =
      await RetryPolicy.ExecuteAsync(() => _client.GetAsync($"users/{userId}"));

    return await getResponseMessage.EnsureSuccessStatusCode()
        .Content.ReadFromJsonAsync<User>(DefaultJsonSerializerOptions.Options) ??
      throw new InvalidOperationException("Could not deserialize to User.");
  }

  private async Task<string> AddUser(UserDefinition userDefinition)
  {
    var postResponseMessage = await _client.PostAsJsonAsync(
      "users",
      userDefinition,
      DefaultJsonSerializerOptions.Options);

    postResponseMessage.IsSuccessStatusCode.Should().BeTrue();

    var reference =
      await postResponseMessage.Content.ReadFromJsonAsync<UserReference>(DefaultJsonSerializerOptions.Options) ??
      throw new InvalidOperationException("Could not deserialize to Reference.");

    return reference.UserId;
  }
}
