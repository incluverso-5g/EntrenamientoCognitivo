using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Save_Me : MonoBehaviour
{
    private static Save_Me instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
