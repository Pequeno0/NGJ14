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

    protected override void Start()
    {
        base.Start();
        this.RefreshServerList();
    }

    private void OnGUI()
    {
        if (this.GameStateController.CurrentGameState != GameState.Playing)
        {
            var bounds = new Rect(0, 0, 300, Screen.height);
            GUI.Window(1, bounds, this.RenderWindow, this.GameStateController.CurrentGameState.ToString());
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
                this.NetworkMessageController.PrePlay();
                //this.GameStateController.PrePlay();
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
        this.NetworkMessageController.SetPlayerInfo(PlayerPrefsVars.PlayerName);
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
        this.NetworkMessageController.SetPlayerInfo(PlayerPrefsVars.PlayerName);
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
