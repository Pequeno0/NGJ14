using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class TradingController : SingletonMonoBehaviour<TradingController>
{
    private readonly List<TradePair> trades = new List<TradePair>();

    public void Trade(GameObject initiater, GameObject other)
    {
        if (this.trades.Any(t => (t.Initiater == initiater && t.Other == other) || (t.Other == initiater && t.Initiater == other)))
        {
            return;
        }

        var trade = new TradePair()
        {
            Initiater = initiater,
            Other = other,
            StartTime = Time.time,
            Length = 5.0f,
        };

        this.trades.Add(trade);
    }
}
