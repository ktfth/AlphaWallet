using System.Text;
using Microsoft.AspNetCore.Mvc;
using NBitcoin;
using NBitcoin.Protocol;
using QBitNinja.Client;
using QBitNinja.Client.Models;

namespace AlphaWallet.Controllers;

[ApiController]
[Route("[controller]")]
public class TransactionController : ControllerBase
{
    private readonly ILogger<TransactionController> _logger;

    public TransactionController(ILogger<TransactionController> logger)
    {
        _logger = logger;
    }

    [HttpPost(Name = "PostTransaction")]
    public Transaction Post([FromBody] TransactionRequest body)
    {
        var bitcoinPrivateKey = new BitcoinSecret(body.PrivateKey, Network.Main);
        var network = bitcoinPrivateKey.Network;
        var address = bitcoinPrivateKey.GetAddress(ScriptPubKeyType.Legacy);

        var client = new QBitNinjaClient(network);
        var transactionId = uint256.Parse(body.TransactionHash);
        var transactionResponse = client.GetTransaction(transactionId).Result;

        var receivedCoins = transactionResponse.ReceivedCoins;
        OutPoint? outPointToSpend = null;
        foreach (var coin in receivedCoins)
        {
            if (coin.TxOut.ScriptPubKey == bitcoinPrivateKey.GetAddress(ScriptPubKeyType.Legacy).ScriptPubKey)
            {
                outPointToSpend = coin.Outpoint;
            }
        }
        if (outPointToSpend == null)
            throw new Exception("TxOut doesn't contain our ScriptPubKey");
        var transaction = NBitcoin.Transaction.Create(network);
        transaction.Inputs.Add(new TxIn()
        {
            PrevOut = outPointToSpend
        });
        var hallOfTheMakersAddress = BitcoinAddress.Create(body.Address, network);
        var hallOfTheMakersAmount = new Money(body.Amount, MoneyUnit.BTC);
        var minerFee = new Money(0.00007m, MoneyUnit.BTC);
        var txInAmount = (Money)receivedCoins[(int)outPointToSpend.N].Amount;
        var changeAmount = txInAmount - hallOfTheMakersAmount - minerFee;
        transaction.Outputs.Add(hallOfTheMakersAmount, hallOfTheMakersAddress.ScriptPubKey);
        transaction.Outputs.Add(changeAmount, address.ScriptPubKey);
        var message = body.Message;
        var bytes = Encoding.UTF8.GetBytes(message);
        transaction.Outputs.Add(Money.Zero, TxNullDataTemplate.Instance.GenerateScriptPubKey(bytes));
        transaction.Inputs[0].ScriptSig = address.ScriptPubKey;
        transaction.Sign(bitcoinPrivateKey, receivedCoins.ToArray());

        BroadcastResponse broadcastResponse = client.Broadcast(transaction).Result;

        if (!broadcastResponse.Success)
        {
            return new Transaction() 
            {
                ErrorCode = broadcastResponse.Error.ErrorCode.ToString(),
                ErrorReason = broadcastResponse.Error.Reason.ToString(),
            };
        } else
        {
            try
            {
                using (var node = Node.Connect(network))
                {
                    node.VersionHandshake();
                    node.SendMessage(new InvPayload(InventoryType.MSG_TX, transaction.GetHash()));
                    node.SendMessage(new TxPayload(transaction));
                    Thread.Sleep(500);
                    return new Transaction()
                    {
                        Id = transactionResponse.TransactionId.ToString(),
                        Address = address.ToString(),
                        Hash = transaction.GetHash().ToString(),
                        BlockConfirmations = transactionResponse.Block.Confirmations,
                    };
                }
            } catch (Exception ex)
            {
                return new Transaction()
                {
                    ExceptionMessage = ex.Message,
                };
            }
        }
    }
}
