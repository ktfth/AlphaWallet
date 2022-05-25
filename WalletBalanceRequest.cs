namespace AlphaWallet;

public class WalletBalanceRequest
{
    public WalletBalanceRequest(string privateKey) {
        PrivateKey = privateKey;
    }
    public string PrivateKey { get; set; }
}
