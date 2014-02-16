using UnityEngine;
using System.Linq;
using System.Collections;

public partial class NetworkMessageController : BaseMonoBehaviour
{

    public NetworkView Reliable;
    public NetworkView Unreliable;

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
        using (new TimeMeasure("OnAddPed"))
        {
            this.PedController.AddPed(id, position, rotation);
        }
    }

    public void UpdatePed(int id, Vector3 position, Vector3 rotation, Vector3 direction, bool trading, bool backstabbing, bool hasItem)
    {
        if (Network.isServer)
        {
            this.Unreliable.RPC("OnUpdatePed", RPCMode.All, id, position, rotation, direction, trading, backstabbing, hasItem);
        }
    }

    [RPC]
    private void OnUpdatePed(int id, Vector3 position, Vector3 rotation, Vector3 direction, bool trading, bool backstabbing, bool hasItem, NetworkMessageInfo messageInfo)
    {
        using (new TimeMeasure("OnUpdatePed"))
        {
            this.PedController.UpdatePedFromServer(id, position, rotation, direction, trading, backstabbing, hasItem);
        }
    }

    [RPC]
    public void CreateMapChunk(string chunkName, string levelChunkName, Vector3 position, Quaternion rotation)
    {
        using (new TimeMeasure("CreateMapChunk"))
        {
            this.StartCoroutine(createMapChunkCoroutine(chunkName, levelChunkName, position, rotation));
        }
    }

    private IEnumerator createMapChunkCoroutine(string chunkName, string levelChunkName, Vector3 position, Quaternion rotation)
    {
        Application.LoadLevelAdditive(chunkName);
        yield return null;
        var chunk = GameObject.Find(chunkName);
        chunk.name = levelChunkName;
        Transform[] objects = chunk.GetComponentsInChildren<Transform>();

        if (Network.isClient)
        {
            Collider[] colliders = chunk.GetComponentsInChildren<Collider>();
            foreach (Collider col in colliders)
            {
                col.enabled = false;
            }
        }

        var grounds = objects.Where(d => d.name == "Ground");
        GameObject groundsObject = GameObject.Find("Grounds");
        if (groundsObject == null)
            groundsObject = new GameObject();
        groundsObject.transform.position = Vector3.zero;
        groundsObject.name = "Grounds";
        foreach (Transform g in grounds)
        {
            g.position = position;
            g.parent = groundsObject.transform;
        }

        chunk.transform.position = position;
        chunk.transform.rotation = rotation;
        foreach (Transform t in objects)
        {
            if (t.gameObject.name.Contains("Level chunk") || t.gameObject.name == "Ground")
                continue;
            //if ()
            //{
            //    t.transform.localRotation = Quaternion.Inverse(rotation);
            //}
            t.transform.rotation = Quaternion.identity;
        }

    }

    [RPC]
    public void CreateMapBorder(int horiontalChunkCount, int verticalChuckCount, int chunkWidth, int chunkHeight)
    {
        using (new TimeMeasure("CreateMapBorder"))
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
    }

    public void PrePlay()
    {
        this.Reliable.RPC("OnPrePlay", RPCMode.All);
    }

    [RPC]
    private void OnPrePlay(NetworkMessageInfo messageInfo)
    {
        using (new TimeMeasure("OnPrePlay"))
        {
            this.GameStateController.PrePlay();
        }
    }

    public void Play()
    {
        this.Reliable.RPC("OnPlay", RPCMode.All);
		ItemPickupController.Setup();
    }

    [RPC]
    private void OnPlay(NetworkMessageInfo messageInfo)
    {
        using (new TimeMeasure("OnPlay"))
        {
            this.GameStateController.Play();
        }
    }

    public void StartTradeGrahicsOnClients(float duration, NetworkPlayer networkPlayer)
    {
        Debug.Log(string.Concat("StartTradeGrahicsOnClients[NetworkPlayer=", networkPlayer, "]"));
        this.Reliable.RPC("OnStartTradeGrahicsOnClients", RPCMode.All, duration, networkPlayer);
    }

    [RPC]
    public void OnStartTradeGrahicsOnClients(float duration, NetworkPlayer networkPlayer)
    {
        using (new TimeMeasure("OnStartTradeGrahicsOnClients"))
        {
            var player = this.PlayerController.Players.First(p => p.NetworkPlayer.Equals(networkPlayer));
            var ped = this.PedController.Peds.First(p => p.Id == player.PedId);
            Debug.Log(string.Concat("OnStartTradeGrahicsOnClients[NetworkPlayer=", networkPlayer, " Player=", player.NetworkPlayer, " Ped=", ped.Id, " Ped expected=", player.PedId, "]"));
            var graphics = ped.Transform.GetComponentInChildren<TradeProgressGraphics>();
            graphics.StartTradingGraphics(duration);
        }
    }

    public void StopTradingGraphics(bool disrupted, NetworkPlayer networkPlayer)
    {
        this.Reliable.RPC("OnStopTradingGraphics", RPCMode.All, disrupted, networkPlayer);
    }

    [RPC]
    private void OnStopTradingGraphics(bool disrupted, NetworkPlayer networkPlayer)
    {
        using (new TimeMeasure("OnStopTradingGraphics"))
        {
            var player = this.PlayerController.Players.First(p => p.NetworkPlayer.Equals(networkPlayer));
            var ped = this.PedController.Peds.First(p => p.Id == player.PedId);
            var graphics = ped.Transform.GetComponentInChildren<TradeProgressGraphics>();
            graphics.StopTradingGraphics();
            // TODO: Insert explosion of items?

        }
    }

    public void SetReadyToTradeFromClient(bool isReadyToTrade)
    {
        if (Network.isServer)
        {
            this.OnSetReadyToTradeFromClient(isReadyToTrade, Network.player);
        }
        else
        {
            this.Reliable.RPC("OnSetReadyToTradeFromClient", RPCMode.Server, isReadyToTrade, Network.player);
        }
    }

    [RPC]
    private void OnSetReadyToTradeFromClient(bool isReadyToTrade, NetworkPlayer networkPlayer)
    {
        using (new TimeMeasure("OnSetReadyToTradeFromClient"))
        {
            if (Network.isServer)
            {
                this.TradingController.SetReadyToTrade(networkPlayer, isReadyToTrade);
            }
        }
    }

    public void SetReadyToTradeFromServer(bool isReadyToTrade, NetworkPlayer networkPlayer)
    {
        if (Network.isServer && Network.player.Equals(networkPlayer))
        {
            this.OnSetReadyToTradeFromServer(isReadyToTrade, networkPlayer);
        }
        else
        {
            this.Reliable.RPC("OnSetReadyToTradeFromServer", networkPlayer, isReadyToTrade, networkPlayer);
        }
    }

    [RPC]
    private void OnSetReadyToTradeFromServer(bool isReadyToTrade, NetworkPlayer networkPlayer)
    {
        using (new TimeMeasure("OnSetReadyToTradeFromServer"))
        {
            Debug.Log("OnSetReadyToTradeFromServer " + networkPlayer);
            this.Menu.IsReadyToTrade = isReadyToTrade;
        }
    }

	public void RespawnItemPickupFromServer(int globalID)
	{
		this.Reliable.RPC("OnRespawnItemPickupFromServer", RPCMode.Others, globalID);
	}

	[RPC]
	private void OnRespawnItemPickupFromServer(int globalID)
	{
        using (new TimeMeasure("OnRespawnItemPickupFromServer"))
        {
            ItemPickupController.GetItemPickup(globalID).RespawnItem();
        }
	}

	public void ConsumeItemPickupFromServer(int globalID)
	{
		this.Reliable.RPC("OnConsumeItemPickupFromServer", RPCMode.Others, globalID);
	}
	
	[RPC]
	private void OnConsumeItemPickupFromServer(int globalID)
	{
        using (new TimeMeasure("OnConsumeItemPickupFromServer"))
        {
            ItemPickupController.GetItemPickup(globalID).ConsumeItem();
        }
		
	}

    public void RemovePlayerFromServer(int pedId, NetworkPlayer networkPlayer)
    {
        this.Reliable.RPC("OnRemovePlayerFromServer", RPCMode.All, pedId, networkPlayer);
    }

    [RPC]
    private void OnRemovePlayerFromServer(int pedId, NetworkPlayer networkPlayer)
    {
        using (new TimeMeasure("OnRemovePlayerFromServer"))
        {
            this.PedController.RemovePed(pedId);
            this.PlayerController.RemovePlayer(networkPlayer);
        }
    }
}
