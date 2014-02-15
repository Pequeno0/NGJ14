using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public partial class TradingController : SingletonMonoBehaviour<TradingController>
{
    private readonly Dictionary<NetworkPlayer, bool> readyToTradeStates = new Dictionary<NetworkPlayer, bool>();
    private readonly List<TradePair> trades = new List<TradePair>();

    private void Update()
    {
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
                this.trades.RemoveAt(index);
                index--;
            }
        }

        // check if a trade can be started
        foreach (var outer in this.readyToTradeStates)
        {
            if (!outer.Value)
            {
                continue;
            }

            foreach (var inner in this.readyToTradeStates)
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

                var outerTransform = outerPed.Transform;
                var innerTransform = innerPed.Transform;

                var distance = Vector3.Distance(outerTransform.position, innerTransform.position);
                if (distance < 1.2f)
                {
                    // deactivate ready to trade to enure that when the trade stops
                    // it is not automatically restarted
                    this.readyToTradeStates[outer.Key] = false;
                    this.readyToTradeStates[inner.Key] = false;

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
