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
        var network = Network.Main;

        var privateKey = new Key();
        var bitcoinPrivateKey = privateKey.GetWif(network);
        var address = bitcoinPrivateKey.GetAddress(ScriptPubKeyType.Legacy);
        
        return new Wallet() {
            PrivateKey = bitcoinPrivateKey.ToString(),
            Address = address.ToString(),
        };
    }

    [HttpPost(Name = "PostWalletMnemonic")]
    public WalletMnemonic Post([FromBody] WalletMnemonicRequest body)
    {
        var network = Network.Main;
        Mnemonic mnemo = new Mnemonic(Wordlist.English, WordCount.Twelve);
        ExtKey hdRoot = mnemo.DeriveExtKey(body.Password);
        var address = hdRoot.PrivateKey.PubKey.GetAddress(ScriptPubKeyType.Legacy, network);

        return new WalletMnemonic()
        {
            PrivateKey = hdRoot.PrivateKey.GetWif(network).ToString(),
            Address = address.ToString(),
            Mnemonic = mnemo.ToString()
        };
    }
}