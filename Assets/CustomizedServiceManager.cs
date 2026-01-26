using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizedServiceManager : MonoBehaviour
{
    private AndroidJavaObject customizedService;

    void Start()
    {
        Debug.Log("Entramos en start de Customized Service Manager");
        InitializeCustomizedService();
    }

    private void InitializeCustomizedService()
    {
        try
        {
            using (AndroidJavaClass customizedServiceClass = new AndroidJavaClass("com.htc.customizedlib.CustomizedService"))
            {
                customizedServiceClass.CallStatic("init", GetUnityActivity(), new InitListener());
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error al inicializar CustomizedService: " + e.Message);
        }
    }

    private AndroidJavaObject GetUnityActivity()
    {
        using (AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            return unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
        }
    }

    private class InitListener : AndroidJavaProxy
    {
        public InitListener() : base("com.htc.customizedlib.CustomizedService$InitListener") { }

        public void onConnected()
        {
            Debug.Log("CustomizedService está conectado.");
        }

        public void onDisconnected()
        {
            Debug.Log("CustomizedService está desconectado.");
        }
    }
}