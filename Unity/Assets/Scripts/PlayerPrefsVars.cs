using UnityEngine;
using System.Collections;

public static class PlayerPrefsVars
{
    public static string PlayerName
    {
        get
        {
            return PlayerPrefs.GetString("PlayerName");
        }
        set
        {
            PlayerPrefs.SetString("PlayerName", value);
        }
    }
}
