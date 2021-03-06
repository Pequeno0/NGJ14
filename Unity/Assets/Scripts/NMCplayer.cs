﻿using UnityEngine;
using System.Collections;

partial class NetworkMessageController : BaseMonoBehaviour
{
    [RPC]
    public void UpdatePlayerDirection(Vector3 direction, NetworkMessageInfo info)
    {
        using (new TimeMeasure("UpdatePlayerDirection"))
        {
            if (Network.isServer)
            {
                this.PedController.UpdatePedFromClient(info.sender, direction);
            }
        }
    }
    
    public void StopTradingFromServer(TradePair trade)
    {
        
        StopTradingGraphics(true, trade.InitiaterPlayer.NetworkPlayer);
        StopTradingGraphics(true, trade.OtherPlayer.NetworkPlayer);
    }
}

