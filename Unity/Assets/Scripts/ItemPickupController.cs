using UnityEngine;
using System.Collections;
using System.Linq;

public class ItemPickupController : SingletonMonoBehaviour<ItemPickupController> {

	private ItemPickup[] itemPickups;

	public void Setup()
	{
		itemPickups = GameObject.FindObjectsOfType<ItemPickup>();

		itemPickups = itemPickups.OrderBy(i => i.transform.position.GetHashCode()).ToArray();

		for (int i = 0; i < itemPickups.Length; i++)
		{
			itemPickups[i].globalID = i;
		}

	}

	public ItemPickup GetItemPickup(int globalID)
	{
		for (int i = 0; i < itemPickups.Length; i++)
		{
			if (itemPickups[i].globalID == globalID)
			{
				return itemPickups[i];
			}
			
		}

		Debug.LogError("Couldn't find the ItemPickup with id " + globalID);
		return null;
	}


}
