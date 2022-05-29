namespace AlphaWallet;

public class WalletMnemonicRequest
{
    public WalletMnemonicRequest(string password, string? mnemonic) {
        Password = password;
        Mnemonic = mnemonic;
    }

    public string Password { get; set; }
    public string? Mnemonic { get; set; }
}
