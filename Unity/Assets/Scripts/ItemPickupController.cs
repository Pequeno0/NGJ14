using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ItemPickupController : SingletonMonoBehaviour<ItemPickupController> {

	private ItemPickup[] itemPickups;

	public ItemPickup[] ItemPickups
	{
		get
		{
			if(itemPickups == null)
			{
				Setup ();
			}
			return itemPickups;
		}
	}

	public void Setup()
	{

		itemPickups = GameObject.FindObjectsOfType<ItemPickup>();

		itemPickups = itemPickups.OrderBy(i => i.transform.position.GetHashCode()).ToArray();

//		Debug.Log(itemPickups.Count.t);
		Debug.Log(itemPickups.Length);

		for (int i = 0; i < itemPickups.Length; i++)
		{

			itemPickups[i].globalID = i;
		}

	}

	public ItemPickup GetItemPickup(int globalID)
	{
		for (int i = 0; i < ItemPickups.Length; i++)
		{
			if (ItemPickups[i].globalID == globalID)
			{
				return ItemPickups[i];
			}
			
		}

		Debug.LogError("Couldn't find the ItemPickup with id " + globalID);
		return null;
	}


}
