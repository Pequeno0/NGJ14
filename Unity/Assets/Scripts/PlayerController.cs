﻿using UnityEngine;
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
        if (Network.isServer)
        {
            var player = this.players.First(p => p.NetworkPlayer.Equals(networkPlayer));
            var ped = this.PedController.Peds.FirstOrDefault(p => p.Id == player.PedId);
            if (ped == null)
                return;
            this.NetworkMessageController.RemovePlayerFromServer(ped.Id, networkPlayer);
        }
    }

    public void RemovePlayer(NetworkPlayer networkPlayer)
    {
        this.players.RemoveAll(p => p.NetworkPlayer.Equals(networkPlayer));
    }

    private void OnServerInitialized()
    {
        this.OnPlayerConnected(Network.player);
        this.NetworkMessageController.SetPlayerInfo(PlayerPrefsVars.PlayerName, Network.player);
        //this.NetworkMessageController.Reliable.RPC("OnSetPlayerInfo", RPCMode.OthersBuffered, name);
        //this.SetPlayerName(Network.player, PlayerPrefsVars.PlayerName);
    }

    private void OnDisconnectedFromServer(NetworkDisconnection disconnection)
    {
        if(Network.isServer)
        {
            this.OnPlayerDisconnected(Network.player);
        }

        Application.LoadLevel(0);
    }

    private Player AddPlayer(NetworkPlayer networkPlayer)
    {
        Debug.Log("Added new Player: " + networkPlayer.ToString() + ". ip: " + networkPlayer.ipAddress);
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
        player.PedId = int.Parse(networkPlayer.ToString());
    }

    Vector3 lastDirection = Vector3.zero;

    public void FixedUpdate()
    {
        
        if (this.GameStateController.CurrentGameState == GameState.Playing)
        {
            //float middleX = Screen.width / 2;
            //float middleY = Screen.height / 2;
            Vector3 direction = Vector3.zero;
            if (Application.platform == RuntimePlatform.Android)
            {
                Touch[] touches = Input.touches;

//                foreach (Touch t in touches)
//                {
//                    if (t.position.x < middleX - (middleX / 2))
//                        direction += new Vector3(-1, 0);
//                    if (t.position.x > middleX + middleX / 2)
//                        direction += new Vector3(1, 0);
//                    if (t.position.y < middleY - (middleY / 2))
//                        direction += new Vector3(0, -1);
//                    if (t.position.y > middleY + middleY / 2)
//                        direction += new Vector3(0, 1);
//                }

//				Rect leftRect = Menu.Instance.leftArrowRect;
//				leftRect.y = Screen.height - leftRect.y;

				Rect upLeft = new Rect(Menu.Instance.leftArrowRect.xMin, Menu.Instance.upArrowRect.yMin, Menu.Instance.leftArrowRect.width, Menu.Instance.upArrowRect.height);
				Rect upRight = new Rect(Menu.Instance.rightArrowRect.xMin, Menu.Instance.upArrowRect.yMin, Menu.Instance.leftArrowRect.width, Menu.Instance.upArrowRect.height);
				Rect downLeft = new Rect(Menu.Instance.leftArrowRect.xMin, Menu.Instance.downArrowRect.yMin, Menu.Instance.leftArrowRect.width, Menu.Instance.upArrowRect.height);
				Rect downRight = new Rect(Menu.Instance.rightArrowRect.xMin, Menu.Instance.downArrowRect.yMin, Menu.Instance.leftArrowRect.width, Menu.Instance.upArrowRect.height);

				foreach (Touch t in touches)
				{
					if (Menu.Instance.leftArrowRect.Contains(new Vector2(t.position.x, Screen.height - t.position.y)))
						direction += new Vector3(-1, 0);
					else if (upLeft.Contains(new Vector2(t.position.x, Screen.height - t.position.y)))
						direction += new Vector3(-1, 1);
					else if (Menu.Instance.rightArrowRect.Contains(new Vector2(t.position.x, Screen.height - t.position.y)))
						direction += new Vector3(1, 0);
					else if (upRight.Contains(new Vector2(t.position.x, Screen.height - t.position.y)))
						direction += new Vector3(1, 1);
					else if (Menu.Instance.downArrowRect.Contains(new Vector2(t.position.x, Screen.height - t.position.y)))
						direction += new Vector3(0, -1);
					else if (downLeft.Contains(new Vector2(t.position.x, Screen.height - t.position.y)))
						direction += new Vector3(-1, -1);
					else if (Menu.Instance.upArrowRect.Contains(new Vector2(t.position.x, Screen.height - t.position.y)))
						direction += new Vector3(0, 1);
					else if (downRight.Contains(new Vector2(t.position.x, Screen.height - t.position.y)))
						direction += new Vector3(1, -1);
				}
				
				
				
				
				
				
			}
			else
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
                if (Network.isClient)
                {
                    this.NetworkMessageController.Reliable.RPC("UpdatePlayerDirection", RPCMode.Server, direction.normalized);
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
