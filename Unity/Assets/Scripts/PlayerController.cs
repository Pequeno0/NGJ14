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

    private void OnPlayerConnected(NetworkPlayer networkPlayer)
    {
        this.AddPlayer(networkPlayer);
    }

    private void OnPlayerDisconnected(NetworkPlayer networkPlayer)
    {
        this.players.RemoveAll(p => p.NetworkPlayer.guid == networkPlayer.guid);
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
        var player = new Player();
        this.players.Add(player);
        return player;
    }

    public void SetPlayerName(NetworkPlayer networkPlayer, string playerName)
    {
        var player = this.players.FirstOrDefault(p => p.NetworkPlayer.guid == networkPlayer.guid);
        if (player == null)
        {
            player = this.AddPlayer(networkPlayer);
        }
        player.Name = playerName;
    }

    public float speed = 1f;
    float middleX = Screen.width / 2;
    float middleY = Screen.height / 2;
    public void Update()
    {
        if (this.GameStateController.CurrentGameState == GameState.Playing)
        {
            Vector3 direction = Vector3.zero;
            if (Application.platform == RuntimePlatform.Android)
            {
                Touch[] touches = Input.touches;

                foreach (Touch t in touches)
                {
                    if (t.position.x < middleX - (middleX / 2))
                        direction += new Vector3(1, 0);
                    if (t.position.x > middleX + middleX / 2)
                        direction += new Vector3(-1, 0);
                    if (t.position.y < middleY - (middleY / 2))
                        direction += new Vector3(0, 1);
                    if (t.position.y > middleX + middleY / 2)
                        direction += new Vector3(0, -1);
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

            
            //this.rigidbody.velocity += direction * speed * Time.deltaTime;
            this.transform.position += direction * speed * Time.deltaTime;
            Quaternion rotationToLookAt = Quaternion.LookRotation(direction);
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, rotationToLookAt, Time.deltaTime);
            Player player = Players.First(d => d.NetworkPlayer.guid == Network.player.guid);


            this.PedController.UpdatePed(player.PedId, this.transform.position, this.transform.eulerAngles);
        }
    }
}
