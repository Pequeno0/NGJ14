using UnityEngine;
using System.Collections;

public class NetworkMessageController : MonoBehaviour
{
    private PlayerController playerController;

    public NetworkView Reliable;
    public NetworkView Unreliable;

    private void Start()
    {
        this.playerController = PlayerController.Singleton;
    }

    public void SetPlayerInfo(string name)
    {
        this.Reliable.RPC("OnSetPlayerInfo", RPCMode.Server, name);
    }

    [RPC]
    private void OnSetPlayerInfo(string name, NetworkMessageInfo messageInfo)
    {
        this.playerController.SetPlayerName(messageInfo.sender, name);
    }
}
