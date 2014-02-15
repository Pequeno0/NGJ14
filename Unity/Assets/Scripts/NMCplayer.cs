using UnityEngine;
using System.Collections;

partial class NetworkMessageController : BaseMonoBehaviour
{
    [RPC]
    public void UpdatePlayerDirection(int id, Vector3 direction)
    {
        print("Recieved Update Player Direction");
        if (Network.isServer)
        {
            this.PedController.UpdatePedFromClient(id, direction);
        }
    }
}

