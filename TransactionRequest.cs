namespace AlphaWallet;

public class TransactionRequest
{
    public TransactionRequest(string privateKey, string transactionHash, string address, string message, decimal amount) {
        PrivateKey = privateKey;
        TransactionHash = transactionHash;
        Address = address;
        Message = message;
        Amount =  amount;
    }
    public string PrivateKey { get; set; }
    public string TransactionHash { get; set; }
    public string Address { get; set; }
    public string Message { get; set; }
    public decimal Amount { get; set; }
}
