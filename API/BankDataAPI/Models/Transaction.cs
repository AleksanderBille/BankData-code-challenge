namespace BankDataAPI.Models;

public class Transaction
{
    public long Id { get; set; }
    public long FromAccountId { get; set; }
    public long ToAccountId { get; set; }
    public decimal Amount { get; set; }
    public DateTime Timestamp { get; set; }
}
