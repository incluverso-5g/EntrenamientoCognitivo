using System;
using System.Collections;
using UnityEngine;
using Wave.Essence;
using Wave.Native;

public class EyeTrackingCalibration : MonoBehaviour
{
    private WVR_DeviceType device_rigth = WVR_DeviceType.WVR_DeviceType_Controller_Right;

    //private const string JavaClassName = "com.htc.customizedlib.BroadcastManager";

    void Update()
    {
        Debug.Log("Starting calibration...");
        //LoadMethodsBroadcats();

        if (WXRDevice.ButtonPress(device_rigth, WVR_InputId.WVR_InputId_Alias1_A))
        {
            Debug.Log("Holi");
            StartEyeTrackingCalibration2();
        }
        //StartEyeTrackingCalibration();
    }

    void StartEyeTrackingCalibration()
    {
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
        {
            activity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
                using (AndroidJavaClass broadcastManager = new AndroidJavaClass("com.htc.customizedlib.BroadcastManager"))
                {
                    bool success = broadcastManager.CallStatic<bool>("launchViveEyeCalibration");
                    Debug.Log("Calibración iniciada: " + success);
                }
            }));
        }
    }


    public void StartEyeTrackingCalibration2()
    {
        try
        {
            using (AndroidJavaClass javaClass = new AndroidJavaClass("com.htc.customizedlib.BroadcastManager"))
            {
                Debug.Log("BroadcastManager class loaded successfully " + javaClass.ToString()); //BroadcastManager class loaded successfully UnityEngine.AndroidJavaClass

                try
                {
                    using (AndroidJavaClass broadcastManager = new AndroidJavaClass("com.htc.customizedlib.BroadcastManager"))
                    {
                        bool success = broadcastManager.CallStatic<bool>("launchViveEyeCalibration");
                        Debug.Log("Calibración iniciada: " + success);
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error al llamar a launchViveEyeCalibration: " + e);
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error while starting calibration: {e}");
        }
    }
}
