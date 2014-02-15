using UnityEngine;
using System.Collections;

public class Menu : BaseMonoBehaviour
{
    private const string gameTypeName = "{00AC2942-5140-404A-B826-BDB25F8A7BE2}";
    private Vector2 serverListScrollPosition;
    private Vector2 playerListScrollPosition;
    private HostData[] servers = new HostData[0];
    private string gameName = string.Empty;
    private string errorMessage = string.Empty;
    private float GuiRatio = 1f;

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

	void Awake()
	{
		Instance = this;
	}

    protected override void Start()
    {
        base.Start();
        this.RefreshServerList();
        GuiRatio = Screen.width / 800f;
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
        
        
        if (this.GameStateController.CurrentGameState != GameState.Playing)
        {
			SetGUIScale();
			
            var bounds = new Rect(0, 0, 300 , Screen.height / GuiRatio);
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
		Debug.Log("Screen height: " + Screen.height);

		int unitSize = Mathf.RoundToInt((float)Screen.height * 0.20f);

		upArrowRect = new Rect(unitSize, Screen.height - 3 * unitSize, unitSize, unitSize);
		downArrowRect = new Rect(unitSize, Screen.height - unitSize, unitSize, unitSize);
		leftArrowRect = new Rect(0, Screen.height - 2 * unitSize, unitSize, unitSize);
		rightArrowRect = new Rect(2 * unitSize, Screen.height - 2 * unitSize, unitSize, unitSize);


		GUI.DrawTexture(upArrowRect, upArrow, ScaleMode.StretchToFill);
		GUI.DrawTexture(downArrowRect, downArrow, ScaleMode.StretchToFill);
		GUI.DrawTexture(leftArrowRect, leftArrow, ScaleMode.StretchToFill);
		GUI.DrawTexture(rightArrowRect, rightArrow, ScaleMode.StretchToFill);


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
                this.errorMessage = string.Empty;
                Network.Connect(server);
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
            this.errorMessage = string.Empty;
            Network.InitializeServer(1000, 12345, true);
        }
        GUILayout.EndHorizontal();
        GUILayout.Label(this.errorMessage);
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
