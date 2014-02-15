using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public partial class TradingController : SingletonMonoBehaviour<TradingController>
{
    private readonly List<TradePair> trades = new List<TradePair>();

    private void Update()
    {
        foreach(var trade in this.trades)
        {
            if(Time.time - trade.StartTime > trade.Duration)
            {
                this.NetworkMessageController.StopTradingGraphics(trade.InitiaterPlayer.NetworkPlayer);
                this.NetworkMessageController.StopTradingGraphics(trade.OtherPlayer.NetworkPlayer);
            }
        }
    }

    public void Trade(GameObject initiater, GameObject other)
    {
        Debug.Log(string.Concat(initiater.name, " ", other.name));

        if (initiater == other)
        {
            return;
        }

        if (!initiater.name.StartsWith("Ped") && !other.name.StartsWith("Ped"))
        {
            return;
        }

        if (this.trades.Any(t => (t.Initiater == initiater && t.Other == other) || (t.Other == initiater && t.Initiater == other)))
        {
            return;
        }

        var initiaterPed = this.PedController.Peds.First(p => p.Transform == initiater.transform || p.Transform == initiater.transform.parent);
        var otherPed = this.PedController.Peds.First(p => p.Transform == other.transform || p.Transform == other.transform.parent);

        var initiaterPlayer = this.PlayerController.Players.First(p => p.PedId == initiaterPed.Id);
        var otherPlayer = this.PlayerController.Players.First(p => p.PedId == otherPed.Id);

        var trade = new TradePair()
        {
            Initiater = initiater,
            Other = other,
            InitiaterPed = initiaterPed,
            OtherPed = otherPed,
            InitiaterPlayer = initiaterPlayer,
            OtherPlayer = otherPlayer,
            StartTime = Time.time,
            Duration = 5.0f,
        };

        this.trades.Add(trade);

        this.NetworkMessageController.StartTradeGrahicsOnClients(trade.Duration, initiaterPlayer.NetworkPlayer);
        this.NetworkMessageController.StartTradeGrahicsOnClients(trade.Duration, otherPlayer.NetworkPlayer);
    }
}
