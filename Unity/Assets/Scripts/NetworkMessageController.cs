using UnityEngine;
using System.Collections;

public class NetworkMessageController : MonoBehaviour
{
    private PlayerController playerController;
    private PedController pedController;

    public NetworkView Reliable;
    public NetworkView Unreliable;

    private void Start()
    {
        this.playerController = PlayerController.Singleton;
        this.pedController = PedController.Singleton;
    }

    public void SetPlayerInfo(string name)
    {
        this.Reliable.RPC("OnSetPlayerInfo", RPCMode.AllBuffered, name);
    }

    [RPC]
    private void OnSetPlayerInfo(string name, NetworkMessageInfo messageInfo)
    {
        this.playerController.SetPlayerName(messageInfo.sender, name);
    }

    public void AddPed(int id, Vector3 position, Vector3 rotation)
    {
        if (Network.isServer)
        {
            this.Reliable.RPC("OnAddPed", RPCMode.All, id, position, rotation);
        }
    }

    [RPC]
    private void OnAddPed(int id, Vector3 position, Vector3 rotation, NetworkMessageInfo messageInfo)
    {
        this.pedController.AddPed(id, position, rotation);
    }

    public void UpdatePed(int id, Vector3 position, Vector3 rotation)
    {
        if(Network.isServer)
        {
            this.Unreliable.RPC("OnUpdatePed", RPCMode.All, position, rotation);
        }
    }

    [RPC]
    private void OnUpdatePed(int id, Vector3 position, Vector3 rotation, NetworkMessageInfo messageInfo)
    {
        this.pedController.UpdatePed(id, position, rotation);
    }
}
