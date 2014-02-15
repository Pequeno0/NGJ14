using UnityEngine;
using System.Collections;

public static class Extensions
{
    public static NetworkPlayer GetActualSender(this NetworkMessageInfo messageInfo)
    {
        var networkPlayer = messageInfo.sender;
        //if (networkPlayer)
        //{
        //    networkPlayer = Network.player;
        //}
        return networkPlayer;
    }
}
