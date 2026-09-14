namespace BankDataAPI.Models;

public class Account
{
    public long Id { get; set; }
    public decimal Balance { get; set; }
    public Guid UserId { get; set; }
}
