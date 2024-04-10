using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public bool verbose = true;
    public bool keepAlive = false;
    public bool duplicateAllow = true;

    private static T _instance;
    public static T Instance
    {
        get
        {
            if (!_instance)
                _instance = GameObject.FindObjectOfType<T>();
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance && !duplicateAllow)
        {
            if (verbose)
                Debug.Log("Destroy duplicate singleton");
            Destroy(gameObject);
        }
        if (!duplicateAllow && keepAlive)
            DontDestroyOnLoad(gameObject);
        _instance = GetComponent<T>();
    }
}
