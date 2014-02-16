using UnityEngine;
using System.Collections;

public class BaseMonoBehaviour : MonoBehaviour
{
    public GameStateController GameStateController
    {
        get;
        private set;
    }

    public PedController PedController
    {
        get;
        private set;
    }
    public PlayerController PlayerController
    {
        get;
        private set;

    }

    public NetworkMessageController NetworkMessageController
    {
        get;
        private set;
    }


    public PrePlayingController PrePlayingController
    {
        get;
        private set;
    }

    public TradingController TradingController
    {
        get;
        private set;
    }

    public Menu Menu
    {
        get;
        private set;
    }

	public ItemPickupController ItemPickupController
	{
		get;
		private set;
	}
	
	protected virtual void Start()
    {
        this.GameStateController = GameStateController.Singleton;
        this.PedController = PedController.Singleton;
        this.PlayerController = PlayerController.Singleton;
        this.NetworkMessageController = GameObject.FindObjectOfType<NetworkMessageController>();
        this.PrePlayingController = PrePlayingController.Singleton;
        this.TradingController = TradingController.Singleton;
        this.Menu = GameObject.FindObjectOfType<Menu>();
		this.ItemPickupController = ItemPickupController.Singleton;
    }
}
