using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public partial class TradingController : SingletonMonoBehaviour<TradingController>
{
    private readonly Dictionary<NetworkPlayer, bool> readyToTradeStates = new Dictionary<NetworkPlayer, bool>();
    private readonly List<TradePair> trades = new List<TradePair>();
    public List<ItemPickup> Items = new List<ItemPickup>();

    private void FixedUpdate()
    {
        if (!Network.isServer)
        {
            return;
        }

        // check if any trade is done
        for(var index = 0; index < this.trades.Count; index++)
        {
            var trade = this.trades[index];
            if(Time.time - trade.StartTime > trade.Duration)
            {
                this.NetworkMessageController.StopTradingGraphics(trade.InitiaterPlayer.NetworkPlayer);
                this.NetworkMessageController.StopTradingGraphics(trade.OtherPlayer.NetworkPlayer);
                this.NetworkMessageController.AddToPlayerScoreOnServer(trade.InitiaterPlayer.NetworkPlayer, 1);
                this.NetworkMessageController.AddToPlayerScoreOnServer(trade.OtherPlayer.NetworkPlayer, 1);
                trade.InitiaterPed.IsTrading = false;
                trade.OtherPed.IsTrading = false;
                this.trades.RemoveAt(index);
                index--;
            }
        }

        foreach (Ped ped in PedController.Peds)
        {
            if (!ped.IsTrading)
            {
                List<TradePair> tradesToRemove = new List<TradePair>();
                float distance = 1.2f;
                foreach (TradePair trade in trades)
                {
                    if (ped.Transform == trade.Initiater.transform || ped.Transform == trade.Other.transform)
                        continue;
                    if (Vector3.Distance(ped.Transform.position, trade.Initiater.transform.position) < distance)
                    {
                        Ped otherPed = PedController.Peds.SingleOrDefault(d => d.Transform == trade.Initiater.transform);
                        if (otherPed == null)
                            continue;
                        if (CheckDistruptionAvailableDistance(ped, otherPed))
                        {
                            tradesToRemove.Add(trade);
                            break;
                        }
                    }
                    else if (Vector3.Distance(ped.Transform.position, trade.Other.transform.position) < distance)
                    {
                        Ped otherPed = PedController.Peds.SingleOrDefault(d => d.Transform == trade.Other.transform);
                        if (otherPed == null)
                            continue;
                        if (CheckDistruptionAvailableDistance(ped, otherPed))
                        {
                            tradesToRemove.Add(trade);
                            break;
                        }
                    }
                }
                foreach(TradePair t in tradesToRemove)
                    trades.Remove(t);

                

                foreach (ItemPickup item in Items)
                {
                    if (Vector3.Distance(ped.Transform.position, item.transform.position) < 0.5f)
                    {
                        ped.HasItem = true;

                    }
                }
            }
        }

        // check if a trade can be started
        var copy = this.readyToTradeStates.ToArray();
        foreach (var outer in copy)
        {
            if (!outer.Value)
            {
                continue;
            }

            foreach (var inner in copy)
            {
                if (!inner.Value)
                {
                    continue;
                }
                if (outer.Key == inner.Key)
                {
                    continue;
                }

                var outerPlayer = this.PlayerController.Players.First(p => p.NetworkPlayer == outer.Key);
                var innerPlayer = this.PlayerController.Players.First(p => p.NetworkPlayer == inner.Key);

                var outerPed = this.PedController.Peds.First(p => p.Id == outerPlayer.PedId);
                var innerPed = this.PedController.Peds.First(p => p.Id == innerPlayer.PedId);

                if (outerPed.IsTrading || innerPed.IsTrading)
                {
                    continue;
                }

                var outerTransform = outerPed.Transform;
                var innerTransform = innerPed.Transform;

                var distance = Vector3.Distance(outerTransform.position, innerTransform.position);
                if (distance < 1.0f)
                {
                    this.StartTrade(outerPlayer, innerPlayer, outerPed, innerPed, outerTransform, innerTransform);
                }
            }
        }
    }

    private void StartTrade(Player outerPlayer, Player innerPlayer, Ped outerPed, Ped innerPed, Transform outerTransform, Transform innerTransform)
    {
        var trade = new TradePair()
        {
            Initiater = outerTransform.gameObject,
            Other = innerTransform.gameObject,
            InitiaterPed = outerPed,
            OtherPed = innerPed,
            InitiaterPlayer = outerPlayer,
            OtherPlayer = innerPlayer,
            StartTime = Time.time,
            Duration = 5.0f,
        };

        this.trades.Add(trade);

        outerPed.IsTrading = true;
        innerPed.IsTrading = true;

        // deactivate ready to trade to enure that when the trade stops
        // it is not automatically restarted
        this.readyToTradeStates[outerPlayer.NetworkPlayer] = false;
        this.readyToTradeStates[innerPlayer.NetworkPlayer] = false;

        this.NetworkMessageController.SetReadyToTradeFromServer(false, outerPlayer.NetworkPlayer);
        this.NetworkMessageController.SetReadyToTradeFromServer(false, innerPlayer.NetworkPlayer);

        this.PedController.UpdatePedFromClient(outerPlayer.NetworkPlayer, Vector3.zero);
        this.PedController.UpdatePedFromClient(innerPlayer.NetworkPlayer, Vector3.zero);

        this.NetworkMessageController.StartTradeGrahicsOnClients(trade.Duration, outerPlayer.NetworkPlayer);
        this.NetworkMessageController.StartTradeGrahicsOnClients(trade.Duration, innerPlayer.NetworkPlayer);
    }

    public void SetReadyToTrade(NetworkPlayer networkPlayer, bool isReadyToTrade)
    {
        Debug.Log(string.Concat("SetReadyToTrade[NetworkPlayer=", networkPlayer, ", IsReadyToTrade=", isReadyToTrade, "]"));
        if (this.readyToTradeStates.ContainsKey(networkPlayer))
        {
            this.readyToTradeStates[networkPlayer] = isReadyToTrade;
        }
        else
        {
            this.readyToTradeStates.Add(networkPlayer, isReadyToTrade);
        }
    }

    public bool IsTrading(NetworkPlayer networkPlayer)
    {
        return this.trades.Any(t => t.OtherPlayer.NetworkPlayer.Equals(networkPlayer) || t.InitiaterPlayer.NetworkPlayer.Equals(networkPlayer));
    }
}
