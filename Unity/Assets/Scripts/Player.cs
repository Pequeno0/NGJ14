using UnityEngine;
using System.Collections;

public class Player
{
    public NetworkPlayer NetworkPlayer
    {
        get;
        set;
    }

    public string Name
    {
        get;
        set;
    }

    public int PedId
    {
        get;
        set;
    }
}
