using UnityEngine;
using System.Collections;

public class ItemPickup : MonoBehaviour {

	public int globalID;
	public bool isOnCooldown;

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
		}
	}

	public bool RespawnItemServer()
	{

	}



	public void AssignID(int id)
	{
		globalID = id;
	}



}
