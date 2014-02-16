using UnityEngine;
using System.Collections;
using System.Linq;
public partial class NetworkMessageController : BaseMonoBehaviour
{
    public void AddToPlayerScoreOnServer(NetworkPlayer player, int add)
    {
        if (Network.isServer)
        {
            var p = PlayerController.Players.FirstOrDefault(m => m.NetworkPlayer == player);
            p.Score += add;

            this.Reliable.RPC("UpdateScoreOnClient", RPCMode.All, player, p.Score);
        }
    }

    [RPC]
    public void UpdateScoreOnClient(NetworkPlayer player, int newScore)
    {
        using (new TimeMeasure("UpdateScoreOnClient"))
        {
            var p = PlayerController.Players.FirstOrDefault(m => m.NetworkPlayer == player);
            p.Score = newScore;
        }
    }

    public void SetPlayerInfo(string name, NetworkPlayer player)
    {
        this.Reliable.RPC("OnSetPlayerInfo", RPCMode.AllBuffered, name, player);
    }

    [RPC]
    private void OnSetPlayerInfo(string name, NetworkPlayer player)
    {
        using (new TimeMeasure("OnSetPlayerInfo"))
        {
            //var networkPlayer = messageInfo.GetActualSender();
            this.PlayerController.SetPlayerName(player, name);
        }
    }
}
