using System.Net;
using System.Net.Http.Json;
using BankDataAPI.DTO;
using Xunit;
using BankDataAPI.Models;

public class AccountControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AccountControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Post_CreatesAccount_ForAuthenticatedUser()
    {
        TestAuthHandler.UserId = Guid.NewGuid();

        var response = await _client.PostAsJsonAsync("/account", new CreateAccountDTO { Balance = 100 });

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GetAll_ReturnsOnlyOwnAccounts()
    {
        var userId = Guid.NewGuid();
        TestAuthHandler.UserId = userId;

        await _client.PostAsJsonAsync("/account", new CreateAccountDTO { Balance = 50 });

        var response = await _client.GetAsync("/account");
        var body = await response.Content.ReadAsStringAsync();
        Assert.True(response.IsSuccessStatusCode, $"Expected success but got {response.StatusCode}: {body}");

        response.EnsureSuccessStatusCode();

        var accounts = await response.Content.ReadFromJsonAsync<List<Account>>();
        Assert.All(accounts!, a => Assert.Equal(userId, a.UserId));
    }
}