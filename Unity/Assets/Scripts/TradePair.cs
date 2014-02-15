using UnityEngine;
using System.Collections;

public class TradePair
{
    public GameObject Initiater
    {
        get;
        set;
    }

    public GameObject Other
    {
        get;
        set;
    }

    public float StartTime
    {
        get;
        set;
    }

    public float Duration
    {
        get;
        set;
    }

    public Ped InitiaterPed
    {
        get;
        set;
    }

    public Ped OtherPed
    {
        get;
        set;
    }

    public Player InitiaterPlayer
    {
        get;
        set;
    }

    public Player OtherPlayer
    {
        get;
        set;
    }
}
