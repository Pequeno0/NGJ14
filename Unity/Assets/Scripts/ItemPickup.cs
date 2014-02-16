using UnityEngine;
using System.Collections;

public class ItemPickup : BaseMonoBehaviour {

	public int globalID;
	public bool isOnCooldown;
	private float timeLeft;

	private static float MINTIME = 10;
	private static float MAXTIME = 20;

	protected override void Start ()
	{
		base.Start ();
	}

	void Update()
	{
		if (Network.isServer)
		{
			if (isOnCooldown)
			{
				timeLeft = timeLeft - Time.deltaTime;
				if (timeLeft < 0)
				{
					RespawnItemServer();
				}
			}
			else
			{
				//Check if anyone's within range

				float closestDistance = Mathf.Infinity;
				Ped closestPed = null;

				foreach (Ped p in PedController.Peds)
				{
					float d = Vector3.Distance(p.Transform.position, transform.position);

					if (d < 0.7f && p.HasItem == false)
					{
						if (d < closestDistance)
						{
							closestDistance = d;
							closestPed= p;
						}
					}
				}

				if (closestPed != null)
				{
					GivePlayerItemServer(closestPed);
					ConsumeItemServer();
				}

			}
		}
	}


	public void GivePlayerItemServer(Ped ped)
	{
		ped.HasItem = true;
	}

	public bool ConsumeItemServer()
	{
		if (isOnCooldown)
		{
			return false;		
		}
		else
		{
			renderer.enabled = true;
			isOnCooldown = true;
			timeLeft = UnityEngine.Random.Range(MINTIME, MAXTIME);
			ConsumeItem();
			NetworkMessageController.ConsumeItemPickupFromServer(globalID);
			return true;
		}
	}

	public void ConsumeItem()
	{
		renderer.enabled = false;
	}

	public void RespawnItemServer()
	{
		RespawnItem();
		NetworkMessageController.RespawnItemPickupFromServer(globalID);
	}

	public void RespawnItem()
	{
		renderer.enabled = true;
	}



	public void AssignID(int id)
	{
		globalID = id;
	}



}
