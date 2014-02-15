using UnityEngine;
using System.Collections;

partial class NetworkMessageController : BaseMonoBehaviour
{
    [RPC]
    public void UpdatePlayerDirection(Vector3 direction, NetworkMessageInfo info)
    {
        if (Network.isServer)
        {
            
            this.PedController.UpdatePedFromClient(info.sender, direction);
        }
    }
}

