using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            instance = FindObjectOfType(typeof(T)) as T;

            if(instance == null)
            {
                instance = new GameObject(typeof(T).ToString() + "Singleton").AddComponent<T>();
                DontDestroyOnLoad(instance);
            }

            return instance;
        }
    }
}
