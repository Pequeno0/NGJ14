using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : SingletonMonoBehaviour<PlayerController>
{
    private readonly List<Player> players = new List<Player>();

    public IEnumerable<Player> Players
    {
        get
        {
            return this.players;
        }
    }

    private void OnPlayerConnected(NetworkPlayer networkPlayer)
    {
        var player = new Player();
        this.players.Add(player);
    }

    private void OnPlayerDisconnected(NetworkPlayer networkPlayer)
    {
        this.players.RemoveAll(p => p.NetworkPlayer == networkPlayer);
    }

    private void OnServerInitialized()
    {
        this.OnPlayerConnected(Network.player);
    }

    private void OnDisconnectedFromServer(NetworkDisconnection disconnection)
    {
        if(Network.isServer)
        {
            this.OnPlayerDisconnected(Network.player);
        }
    }

    [RPC]
    public void SetPlayerName(NetworkPlayer networkPlayer, string playerName)
    {
        var player = this.players.FirstOrDefault(p => p.NetworkPlayer == networkPlayer);
        player.Name = playerName;
    }
}
