// Services/AccountService.cs
using BankDataAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BankDataAPI.Services;

public class AccountService : IAccountService
{
    private readonly AccountContext _context;

    public AccountService(AccountContext context)
    {
        _context = context;
    }

    public async Task<Account> TransferAsync(long fromAccountId, long toAccountId, decimal amount, Guid userid)
    {
        if (fromAccountId == toAccountId) {
            throw new InvalidOperationException("Cannot transfer to the same account.");
        }

        if (amount <= 0) {
            throw new ArgumentException("Transfer amount must be positive.");
        }

        var fromAccount = await _context.Accounts.FindAsync(fromAccountId)
            ?? throw new KeyNotFoundException($"Account {fromAccountId} not found.");

        if (fromAccount.UserId != userid) {
            throw new InvalidOperationException("Account not owned by user.");
        }
        
        var toAccount = await _context.Accounts.FindAsync(toAccountId)
            ?? throw new KeyNotFoundException($"Account {toAccountId} not found.");


        if(fromAccount.Balance < amount) {
            throw new InvalidOperationException("Insufficient funds.");
        }

        fromAccount.Balance -= amount;
        toAccount.Balance += amount;

        var transaction = new Transaction
        {
            FromAccountId = fromAccountId,
            ToAccountId = toAccountId,
            Amount = amount,
            Timestamp = DateTime.UtcNow
        };

        _context.Transactions.Add(transaction);

        await _context.SaveChangesAsync();

        return fromAccount;
    }
}