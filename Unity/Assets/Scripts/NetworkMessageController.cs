using UnityEngine;
using System.Collections;

public class NetworkMessageController : BaseMonoBehaviour
{
    public NetworkView Reliable;
    public NetworkView Unreliable;

    public void SetPlayerInfo(string name)
    {
        this.Reliable.RPC("OnSetPlayerInfo", RPCMode.AllBuffered, name);
    }

    [RPC]
    private void OnSetPlayerInfo(string name, NetworkMessageInfo messageInfo)
    {
        var networkPlayer = messageInfo.GetActualSender();
        this.PlayerController.SetPlayerName(networkPlayer, name);
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
        this.PedController.AddPed(id, position, rotation);
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
        this.PedController.UpdatePed(id, position, rotation);
    }

    [RPC]
    public void CreateMapChunk(string chunkName, string levelChunkName, Vector3 position, Quaternion rotation)
    {
        this.StartCoroutine(createMapChunkCoroutine(chunkName, levelChunkName, position, rotation));
    }

    private IEnumerator createMapChunkCoroutine(string chunkName, string levelChunkName, Vector3 position, Quaternion rotation)
    {
        Application.LoadLevelAdditive(chunkName);
        yield return null;
        var chunk = GameObject.Find(chunkName);
        chunk.name = levelChunkName;
        chunk.transform.position = position;
        chunk.transform.rotation = rotation;
    }

    [RPC]
    public void CreateMapBorder()
    {
        //var parentGo = GameObject.Find("Border");
        //if (parent == null)
        //    parent = new GameObject().transform;


    }
}
