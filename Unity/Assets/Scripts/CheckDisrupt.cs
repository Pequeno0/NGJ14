using UnityEngine;
using System.Collections;
using System.Linq;

public partial class TradingController : SingletonMonoBehaviour<TradingController>
{
    /*
    public void CheckDistruptionAvailable(Collider thisCollider, Collider collider)
    {
        if (Network.isServer)
        {
            Debug.Log("Server1");
            var hitPed = this.PedController.Peds.FirstOrDefault(p => p.Transform == collider.transform || p.Transform == collider.transform.parent);

            if (hitPed == null)
            {
                Debug.Log("hitPed was null");
                return;
            }

            var hitPlayer = this.PlayerController.Players.First(p => p.PedId == hitPed.Id);

            var thisPed = this.PedController.Peds.First(p => p.Transform == thisCollider.transform || p.Transform == thisCollider.transform.parent);

            var thisPlayer = this.PlayerController.Players.First(p => p.PedId == thisPed.Id);

            Debug.Log("Finding trade");
            TradePair trade = trades.FirstOrDefault(d => d.InitiaterPlayer == hitPlayer || d.OtherPlayer == hitPlayer
                                            && (d.InitiaterPlayer.PedId != thisPed.Id || d.OtherPlayer.PedId != thisPed.Id));

            if (trade != null)
            {
                Debug.Log("Trying to disrupt");
                Vector3 pos = thisCollider.transform.position;
                RaycastHit hit;
                if (Physics.Raycast(new Ray(pos, collider.transform.position - pos), out hit, 5f))
                {
                    Debug.Log("Raycast hit something");
                    if (hit.collider == trade.Initiater.collider || hit.collider == trade.Other.collider)
                    {
                        Debug.Log("Disrupting!");
                        NetworkMessageController.AddToPlayerScoreOnServer(trade.OtherPlayer.NetworkPlayer, -1);
                        NetworkMessageController.AddToPlayerScoreOnServer(trade.InitiaterPlayer.NetworkPlayer, -1);
                        NetworkMessageController.AddToPlayerScoreOnServer(thisPlayer.NetworkPlayer, 1);
                        NetworkMessageController.StopTradingFromServer(trade);
                        trades.Remove(trade);
                    }
                }
                else
                    Debug.Log("Something in the way");
            }
            else
                Debug.Log("trade was null");
        }
    }
    */
    public bool CheckDistruptionAvailableDistance(Ped thisPed, Ped hitPed)
    {
        if (thisPed.Id == hitPed.Id)
            return false;

        var hitPlayer = this.PlayerController.Players.First(p => p.PedId == hitPed.Id);

        var thisPlayer = this.PlayerController.Players.First(p => p.PedId == thisPed.Id);

        TradePair trade = trades.FirstOrDefault(d => d.InitiaterPlayer == hitPlayer || d.OtherPlayer == hitPlayer
                                        && (d.InitiaterPlayer.PedId != thisPed.Id && d.OtherPlayer.PedId != thisPed.Id));


        if (trade != null)
        {
            Vector3 pos = thisPed.Transform.position;
            RaycastHit hit;
            if (Physics.Raycast(new Ray(pos, hitPed.Transform.position - pos), out hit, 5f))
            {
                if (hit.collider == trade.Initiater.collider || hit.collider == trade.Other.collider)
                {
                    NetworkMessageController.AddToPlayerScoreOnServer(trade.OtherPlayer.NetworkPlayer, -1);
                    NetworkMessageController.AddToPlayerScoreOnServer(trade.InitiaterPlayer.NetworkPlayer, -1);
                    NetworkMessageController.AddToPlayerScoreOnServer(thisPlayer.NetworkPlayer, 1);
                    NetworkMessageController.StopTradingFromServer(trade);
                    trades.Remove(trade);
                    return true;
                }
            }
        }
        return false;
    }
}
