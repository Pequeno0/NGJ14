using UnityEngine;
using System.Collections;

public class Ped
{
    public int Id
    {
        get;
        set;
    }

    public Transform Transform
    {
        get;
        set;
    }

    public bool IsTrading
    {
        get;
        set;
    }

    public bool IsBackstabbing
    {
        get;
        set;
    }

    public Vector3 Direction
    {
        get;
        set;
    }

    public Vector3 LastPosSent
    {
        get;
        set;
    }

    public bool DirectionZeroSent
    {
        get;
        set;
    }


    public int FramesSinceLastUpdate
    {
        get;
        set;
    }

	private bool hasItem;

    public bool HasItem
    {
		get { return hasItem; }
		set 
		{ 
			hasItem = value; 

//			Debug.Log(value + " " + new System.Diagnostics.StackTrace());
		}

    }
}
