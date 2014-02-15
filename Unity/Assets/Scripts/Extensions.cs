using UnityEngine;
using System.Collections;

public static class Extensions
{
    public static NetworkPlayer GetActualSender(this NetworkMessageInfo messageInfo)
    {
        var networkPlayer = messageInfo.sender;
        if (networkPlayer.guid.Length < 2)
        {
            networkPlayer = Network.player;
        }
        return networkPlayer;
    }
}
