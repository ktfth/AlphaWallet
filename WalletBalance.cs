namespace AlphaWallet;

public class WalletBalance
{
    public WalletBalance(decimal amount) 
    {
        Amount = amount;
    }
    public decimal Amount { get; set; }
}
