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

    public int PlayerCount
    {
        get
        {
            return this.players.Count;
        }
    }

    private void OnConnectedToServer()
    {
        this.AddPlayer(Network.player);
    }

    private void OnPlayerConnected(NetworkPlayer networkPlayer)
    {
        this.AddPlayer(networkPlayer);
    }

    private void OnPlayerDisconnected(NetworkPlayer networkPlayer)
    {
        this.players.RemoveAll(p => p.NetworkPlayer.Equals(networkPlayer));
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

    private Player AddPlayer(NetworkPlayer networkPlayer)
    {
        var player = this.players.FirstOrDefault(p => p.NetworkPlayer.Equals(networkPlayer));
        if (player == null)
        {
            player = new Player()
            {
                NetworkPlayer = networkPlayer,
            };
            this.players.Add(player);
        }
        return player;
    }

    public void SetPlayerName(NetworkPlayer networkPlayer, string playerName)
    {
        var player = this.players.FirstOrDefault(p => p.NetworkPlayer.Equals(networkPlayer));
        if (player == null)
        {
            player = this.AddPlayer(networkPlayer);
        }
        player.Name = playerName;
    }

    Vector3 lastDirection = Vector3.zero;

    public void Update()
    {
        if (this.GameStateController.CurrentGameState == GameState.Playing)
        {
            float middleX = Screen.width / 2;
            float middleY = Screen.height / 2;
            Vector3 direction = Vector3.zero;
            if (Application.platform == RuntimePlatform.Android)
            {
                Touch[] touches = Input.touches;

                foreach (Touch t in touches)
                {
                    if (t.position.x < middleX - (middleX / 2))
                        direction += new Vector3(-1, 0);
                    if (t.position.x > middleX + middleX / 2)
                        direction += new Vector3(1, 0);
                    if (t.position.y < middleY - (middleY / 2))
                        direction += new Vector3(0, -1);
                    if (t.position.y > middleY + middleY / 2)
                        direction += new Vector3(0, 1);
                }

               
            }
            else if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    direction += new Vector3(-1, 0);
                }
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    direction += new Vector3(1, 0);
                }
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    direction += new Vector3(0, 1);
                }
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    direction += new Vector3(0, -1);
                }
            }

            //Player player = Players.First(d => d.NetworkPlayer.Equals(Network.player));

            if (direction != lastDirection)
            {
                print(direction);
                if (Network.isClient)
                {
                    this.NetworkMessageController.Reliable.RPC("UpdatePlayerDirection", RPCMode.Server, direction);
                }
                else
                    this.NetworkMessageController.UpdatePlayerDirection(direction, new NetworkMessageInfo());
                //this.NetworkMessageController.UpdatePlayerDirection(player.PedId, direction);
            }
            lastDirection = direction;
            
            //this.PedController.UpdatePed(player.PedId, this.transform.position);
        }
    }
}
