using Microsoft.AspNetCore.Mvc;
using NBitcoin;

namespace AlphaWallet.Controllers;

[ApiController]
[Route("[controller]")]
public class WalletController : ControllerBase
{
    private readonly ILogger<WalletController> _logger;

    public WalletController(ILogger<WalletController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWallet")]
    public Wallet Get()
    {
        var network = Network.TestNet;

        var privateKey = new Key();
        var bitcoinPrivateKey = privateKey.GetWif(network);
        var address = bitcoinPrivateKey.GetAddress(ScriptPubKeyType.Legacy);
        
        return new Wallet() {
            PrivateKey = bitcoinPrivateKey.ToString(),
            Address = address.ToString(),
        };
    }
}
