namespace BankDataAPI.DTO;

public class TransferDTO 
{
    public long FromAccountId { get; set; }
    public long ToAccountId { get; set; }
    public decimal Amount { get; set; }
}