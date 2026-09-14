// AccountServiceTests.cs
using Microsoft.EntityFrameworkCore;
using BankDataAPI.Models;
using BankDataAPI.Services;
using Xunit;

public class AccountServiceTests
{
    private static AccountContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AccountContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // unique DB per test
            .Options;

        return new AccountContext(options);
    }

    [Fact]
    public async Task TransferAsync_MovesBalanceBetweenAccounts()
    {
        await using var context = CreateContext();
        var userId = Guid.NewGuid();

        var from = new Account { Balance = 100m, UserId = userId };
        var to = new Account { Balance = 50m, UserId = Guid.NewGuid() };
        context.Accounts.AddRange(from, to);
        await context.SaveChangesAsync();

        var service = new AccountService(context);
        await service.TransferAsync(from.Id, to.Id, 30m, userId);

        Assert.Equal(70m, from.Balance);
        Assert.Equal(80m, to.Balance);
    }

    [Fact]
    public async Task TransferAsync_ThrowsWhenNotOwner()
    {
        await using var context = CreateContext();
        var owner = Guid.NewGuid();
        var attacker = Guid.NewGuid();

        var from = new Account { Balance = 100m, UserId = owner };
        var to = new Account { Balance = 0m, UserId = Guid.NewGuid() };
        context.Accounts.AddRange(from, to);
        await context.SaveChangesAsync();

        var service = new AccountService(context);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.TransferAsync(from.Id, to.Id, 10m, attacker));
    }

    [Fact]
    public async Task TransferAsync_ThrowsOnInsufficientFunds()
    {
        await using var context = CreateContext();
        var userId = Guid.NewGuid();

        var from = new Account { Balance = 10m, UserId = userId };
        var to = new Account { Balance = 0m, UserId = Guid.NewGuid() };
        context.Accounts.AddRange(from, to);
        await context.SaveChangesAsync();

        var service = new AccountService(context);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.TransferAsync(from.Id, to.Id, 50m, userId));
    }
}