using UnityEngine;
using System.Collections;

public partial class NetworkMessageController : BaseMonoBehaviour
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

    public void UpdatePed(int id, Vector3 position, Vector3 rotation, Vector3 direction, bool trading, bool backstabbing)
    {
        if (Network.isServer)
        {
            this.Unreliable.RPC("OnUpdatePed", RPCMode.All, id, position, rotation, direction, trading, backstabbing);
        }
    }

    [RPC]
    private void OnUpdatePed(int id, Vector3 position, Vector3 rotation, Vector3 direction, bool trading, bool backstabbing, NetworkMessageInfo messageInfo)
    {
        this.PedController.UpdatePedFromServer(id, position, rotation, direction, trading, backstabbing);
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
    public void CreateMapBorder(int horiontalChunkCount, int verticalChuckCount, int chunkWidth, int chunkHeight)
    {
        var border = Resources.Load("InvisibleBorder", typeof(Transform)) as Transform;
        if (border != null)
        {
            var offsetX = ((horiontalChunkCount - 1f) / 2f) * chunkWidth;
            var offsetY = ((verticalChuckCount - 1f) / 2f) * chunkHeight;

            var tempX = horiontalChunkCount * chunkWidth;
            var tempY = verticalChuckCount * chunkHeight;

            var left = -chunkWidth / 2 - 1;
            var right = chunkWidth * horiontalChunkCount - chunkWidth / 2;
            var bottom = -chunkHeight / 2 - 1;
            var top = chunkHeight * verticalChuckCount - chunkHeight / 2;

            var borderParent = new GameObject().transform;
            borderParent.name = "Border";

            var scale = new Vector3(tempX + 2, 1, 1);
            border.localScale = scale;

            var temp = Transform.Instantiate(border, new Vector3(offsetX, top, -.5f), border.rotation) as Transform;
            temp.name = "Border top";
            temp.parent = borderParent;
            temp = Transform.Instantiate(border, new Vector3(offsetX, bottom, -.5f), border.rotation) as Transform;
            temp.parent = borderParent;
            temp.name = "Border bottom";

            scale = new Vector3(1, tempY + 2, 1);
            border.localScale = scale;

            temp = Transform.Instantiate(border, new Vector3(right, offsetY, -.5f), border.rotation) as Transform;
            temp.name = "Border right";
            temp.parent = borderParent;
            temp = Transform.Instantiate(border, new Vector3(left, offsetY, -.5f), border.rotation) as Transform;
            temp.name = "Border left";
            temp.parent = borderParent;
        }
    }

    public void PrePlay()
    {
        this.Reliable.RPC("OnPrePlay", RPCMode.All);
    }

    [RPC]
    private void OnPrePlay(NetworkMessageInfo messageInfo)
    {
        this.GameStateController.PrePlay();
    }

    public void Play()
    {
        this.Reliable.RPC("OnPlay", RPCMode.All);
    }

    [RPC]
    private void OnPlay(NetworkMessageInfo messageInfo)
    {
        this.GameStateController.Play();
    }
}
