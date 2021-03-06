﻿using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Menu : BaseMonoBehaviour
{
    private const string gameTypeName = "{00AC2942-5140-404A-B826-BDB25F8A7BE2}";
    private Vector2 serverListScrollPosition;
    private Vector2 playerListScrollPosition;
    private HostData[] servers = new HostData[0];
    private string gameName = string.Empty;
    private string errorMessage = string.Empty;
    public float GuiRatio = 1f;

	public Texture upArrow;
	[HideInInspector]
	public Rect upArrowRect;

	public Texture downArrow;
	[HideInInspector]
	public Rect downArrowRect;

	public Texture leftArrow;
	[HideInInspector]
	public Rect leftArrowRect;

	public Texture rightArrow;
	[HideInInspector]
	public Rect rightArrowRect;

	public static Menu Instance;

    public Texture2D logo;

	public Texture2D noItemLogo;
	public Texture2D itemNoTradeLogo;
	public Texture2D itemTradeLogo;

    public bool IsReadyToTrade
    {
        get;
        set;
    }

	void Awake()
	{
		Instance = this;
	}

    protected override void Start()
    {
        base.Start();
        this.RefreshServerList();
        GuiRatio = Screen.width / 800f;
        if (GuiRatio < 1f) 
            GuiRatio = 1f;
    }

    /// <summary>
    /// Scale the gui matrix, determined by the gui ratio
    /// </summary>
    public void SetGUIScale()
    {
        var scale = new Vector3(GuiRatio, GuiRatio, 1f);
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, scale);
    }

    private void OnGUI()
    {
        //if (Network.player != null)
        //{
        //    GUI.Label(new Rect(Screen.width * 0.5f + 20.0f, Screen.height * 0.5f, 30.0f, 30.0f), Network.player.ToString());
        //}
        
        if (this.GameStateController.CurrentGameState != GameState.Playing)
        {
			SetGUIScale();

            this.RenderLogo();

            var bounds = new Rect(0, 0, 300, Screen.height / GuiRatio);
            GUI.Window(1, bounds, this.RenderWindow, this.GameStateController.CurrentGameState.ToString());
        }
		else
		{
			this.RenderPlayingGUI();
		}

    }

    private void RenderWindow(int windowId)
    {
        switch (this.GameStateController.CurrentGameState)
        {
            case GameState.NetworkMenu:
                this.RenderNetworkGUI();
                break;
            case GameState.LobbyMenu:
                this.RenderLobbyGUI();
                break;
            case GameState.PrePlaying:
                this.RenderPrePlaying();
                break;
				
        }
    }

	private void RenderPlayingGUI()
	{
        var player = this.PlayerController.Players.First(p => p.NetworkPlayer == Network.player);
        var ped = this.PedController.Peds.First(p => p.Id == player.PedId);

//		Debug.Log("Screen height: " + Screen.height);
        var tradeGroupBounds = new Rect(Screen.width - 200.0f, Screen.height - 200.0f, 200.0f, 200.0f);
        var originalColor = GUI.color;



		Texture currentTexture = ped.IsTrading || !ped.HasItem ? noItemLogo : this.IsReadyToTrade ? itemTradeLogo : itemNoTradeLogo;


//        GUI.color = ped.IsTrading || !ped.HasItem ? Color.red : this.IsReadyToTrade ? Color.green : Color.white;
        var readyToTrade = GUI.Toggle(tradeGroupBounds, this.IsReadyToTrade, currentTexture, GUI.skin.button);
//        GUI.color = originalColor;

        if(!ped.IsTrading && ped.HasItem)
        {
            if (readyToTrade && !this.IsReadyToTrade)
            {
                this.IsReadyToTrade = true;
                this.NetworkMessageController.SetReadyToTradeFromClient(this.IsReadyToTrade);
            }
            else if (!readyToTrade && this.IsReadyToTrade)
            {
                this.IsReadyToTrade = false;
                this.NetworkMessageController.SetReadyToTradeFromClient(this.IsReadyToTrade);
            }
        }

		int unitSize = Mathf.RoundToInt((float)Screen.height * 0.20f);

		upArrowRect = new Rect(unitSize, Screen.height - 3 * unitSize, unitSize, unitSize);
		downArrowRect = new Rect(unitSize, Screen.height - unitSize, unitSize, unitSize);
		leftArrowRect = new Rect(0, Screen.height - 2 * unitSize, unitSize, unitSize);
		rightArrowRect = new Rect(2 * unitSize, Screen.height - 2 * unitSize, unitSize, unitSize);


		GUI.DrawTexture(upArrowRect, upArrow, ScaleMode.StretchToFill);
		GUI.DrawTexture(downArrowRect, downArrow, ScaleMode.StretchToFill);
		GUI.DrawTexture(leftArrowRect, leftArrow, ScaleMode.StretchToFill);
		GUI.DrawTexture(rightArrowRect, rightArrow, ScaleMode.StretchToFill);

        //var bounds = new Rect(Screen.width * 0.5f - 200.0f, Screen.height * 0.5f - 200.0f, 400.0f, 400.0f);
        //GUI.BeginGroup(bounds);
        //foreach (KeyValuePair<string, TimeMeasure.Data> x in TimeMeasure.Stats)
        //{
        //    GUILayout.Label(string.Concat(x.Key, " [Count=", x.Value.Count, " Min=", x.Value.Min, " Max=", x.Value.Max, " Avg=", x.Value.Elapsed / x.Value.Count, "]"));
        //}
        //GUI.EndGroup();


//		Rect upLeft = new Rect(Menu.Instance.leftArrowRect.xMin, Menu.Instance.upArrowRect.yMin, Menu.Instance.leftArrowRect.width, Menu.Instance.upArrowRect.height);
//		Rect upRight = new Rect(Menu.Instance.rightArrowRect.xMin, Menu.Instance.upArrowRect.yMin, Menu.Instance.leftArrowRect.width, Menu.Instance.upArrow.height);
//		Rect downLeft = new Rect(Menu.Instance.leftArrowRect.xMin, Menu.Instance.downArrowRect.yMin, Menu.Instance.leftArrowRect.width, Menu.Instance.upArrow.height);
//		Rect downRight = new Rect(Menu.Instance.rightArrowRect.xMin, Menu.Instance.downArrowRect.yMin, Menu.Instance.leftArrowRect.width, Menu.Instance.upArrow.height);
//
//
//		GUI.DrawTexture(upLeft, rightArrow, ScaleMode.StretchToFill);
//		GUI.DrawTexture(upRight, rightArrow, ScaleMode.StretchToFill);
//		GUI.DrawTexture(downLeft, rightArrow, ScaleMode.StretchToFill);
//		GUI.DrawTexture(downRight, rightArrow, ScaleMode.StretchToFill);


	}

    private void RenderNetworkGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Player name");
        PlayerPrefsVars.PlayerName = GUILayout.TextField(PlayerPrefsVars.PlayerName);
        GUILayout.EndHorizontal();

        this.serverListScrollPosition = GUILayout.BeginScrollView(this.serverListScrollPosition, true, true);
        foreach (var server in this.servers)
        {
            if (GUILayout.Button(server.gameName))
            {
                if (!string.IsNullOrEmpty(PlayerPrefsVars.PlayerName))
                {
                    this.errorMessage = string.Empty;
                    Network.Connect(server);
                }
            }
        }
        GUILayout.EndScrollView();
        if (GUILayout.Button("Refresh server list"))
        {
            this.RefreshServerList();
        }

        GUILayout.BeginHorizontal();
        GUILayout.Label("Game name");
        this.gameName = GUILayout.TextField(this.gameName);
        if (GUILayout.Button("Host"))
        {
            if (!string.IsNullOrEmpty(this.gameName))
            {
                this.errorMessage = string.Empty;
                Network.InitializeServer(1000, 12345, true);
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.Label(this.errorMessage);
    }

    private void RenderLogo()
    {
        var logoRect = new Rect(300.0f, -120.0f, 1024.0f * 0.45f, 1024.0f * 0.45f);
        GUI.DrawTexture(logoRect, this.logo);

        GUI.BeginGroup(new Rect(logoRect.x, logoRect.xMax, logoRect.width, 400.0f));
        GUI.EndGroup();
    }

    private void RenderLobbyGUI()
    {
        var players = this.PlayerController.Players;
        this.playerListScrollPosition = GUILayout.BeginScrollView(this.playerListScrollPosition, true, true);
        foreach (var player in players)
        {
            GUILayout.Label(player.Name);
        }
        GUILayout.EndScrollView();
        if (Network.isServer)
        {
            if (GUILayout.Button("Play"))
            {
				Network.maxConnections = 0;
                this.NetworkMessageController.PrePlay();
            }
        }
        if (GUILayout.Button("Cancel"))
        {
            Network.Disconnect();
        }
    }

    private void RenderPrePlaying()
    {
        GUILayout.Label("Loading..");
    }

    private void OnFailedToConnectToMasterServer(NetworkConnectionError info)
    {
        this.errorMessage = info.ToString();
    }

    private void OnServerInitialized()
    {
        MasterServer.RegisterHost(Menu.gameTypeName, this.gameName);
        //this.NetworkMessageController.SetPlayerInfo(PlayerPrefsVars.PlayerName);
    }

    private void OnDestroy()
    {
        MasterServer.UnregisterHost();
    }

    private void OnMasterServerEvent(MasterServerEvent serverEvent)
    {
        switch (serverEvent)
        {
            case MasterServerEvent.RegistrationFailedGameName:
            case MasterServerEvent.RegistrationFailedGameType:
            case MasterServerEvent.RegistrationFailedNoServer:
                this.errorMessage = serverEvent.ToString();
                break;
            case MasterServerEvent.HostListReceived:
                this.servers = MasterServer.PollHostList();
                break;
        }
    }

    private void OnConnectedToServer()
    {
        this.NetworkMessageController.SetPlayerInfo(PlayerPrefsVars.PlayerName, Network.player);
        Debug.Log("OnConnectedToServer");
    }

    private void OnDisconnectedFromServer(NetworkDisconnection disconnection)
    {
        Debug.Log("OnDisconnectedFromServer");
    }

    private void OnFailedToConnect(NetworkConnectionError error)
    {
        this.errorMessage = error.ToString();
    }

    private void OnPlayerConnected(NetworkPlayer player)
    {
        Debug.Log("OnPlayerConnected");
    }

    private void OnPlayerDisconnected(NetworkPlayer player)
    {
        Debug.Log("OnPlayerDisconnected");
    }

    private void RefreshServerList()
    {
        MasterServer.RequestHostList(gameTypeName);
    }
}
