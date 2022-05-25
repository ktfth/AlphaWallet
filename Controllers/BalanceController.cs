using Microsoft.AspNetCore.Mvc;
using NBitcoin;
using QBitNinja.Client;

namespace AlphaWallet.Controllers;

[ApiController]
[Route("[controller]")]
public class BalanceController : ControllerBase
{
    private readonly ILogger<BalanceController> _logger;

    public BalanceController(ILogger<BalanceController> logger)
    {
        _logger = logger;
    }

    [HttpPost(Name = "PostBalance")]
    public WalletBalance Post([FromBody] WalletBalanceRequest body)
    {
        var bitcoinPrivateKey = new BitcoinSecret(body.PrivateKey, Network.TestNet);
        var network = bitcoinPrivateKey.Network;
        var address = bitcoinPrivateKey.GetAddress(ScriptPubKeyType.Legacy);

        var client = new QBitNinjaClient(network);
        var balance = client.GetBalance(address).Result;
        var operations = balance.Operations.ToArray();
        var result = new WalletBalance(0.0m);

        for (var i = operations.Length - 1; i >= 0; i--)
        {
            result.Amount = result.Amount + operations[i].Amount.ToDecimal(MoneyUnit.BTC);
        }

        return result;
    }
}
