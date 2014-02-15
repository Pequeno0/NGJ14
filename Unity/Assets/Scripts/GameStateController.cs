using UnityEngine;
using System.Collections;

public class GameStateController : SingletonMonoBehaviour<GameStateController>
{
    public GameState CurrentGameState
    {
        get;
        private set;
    }

    protected override void Start()
    {
        base.Start();
        this.CurrentGameState = GameState.NetworkMenu;
    }

    private void OnServerInitialized()
    {
        this.CurrentGameState = GameState.LobbyMenu;
    }

    private void OnConnectedToServer()
    {
        this.CurrentGameState = GameState.LobbyMenu;
    }

    private void OnDisconnectedFromServer(NetworkDisconnection disconnection)
    {
        this.CurrentGameState = GameState.NetworkMenu;
    }

    public void PrePlay()
    {
        this.CurrentGameState = GameState.PrePlaying;
    }

    public void Play()
    {
        this.CurrentGameState = GameState.Playing;
    }
}
