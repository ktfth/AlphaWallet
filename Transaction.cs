namespace AlphaWallet;

public class Transaction
{
    public string? ErrorCode { get; set; }
    public string? ErrorReason { get; set; }
    public string? ExceptionMessage { get; set; }
    public string? Id { get; set; }
    public string? Hash { get; set; }
    public string? Address { get; set; }
    public int? BlockConfirmations { get; set; }
}
