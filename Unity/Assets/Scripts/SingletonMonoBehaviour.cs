using UnityEngine;
using System.Collections;

public class SingletonMonoBehaviour<T> : BaseMonoBehaviour where T : Component
{
    private static T singleton;

    public static T Singleton
    {
        get
        {
            if (singleton == null)
            {
                var gameObject = new GameObject(typeof(T).Name);
                singleton = gameObject.AddComponent<T>();
            }
            return singleton;
        }
    }
}
