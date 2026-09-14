using BankDataAPI.Models;

namespace BankDataAPI.Services;

public interface IAccountService
{
    Task<Account> TransferAsync(long fromAccountId, long toAccountId, decimal amount, Guid userid);
}