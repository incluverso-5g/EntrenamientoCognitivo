using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wave.OpenXR;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using Wave.Essence;
using Wave.Native;

public class GrabObjectCollision : MonoBehaviour
{
    GameObject grabbedObject = null;
    Transform originalParent = null;
    private WVR_DeviceType device_left = WVR_DeviceType.WVR_DeviceType_Controller_Left;


    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Colision");
        if (collision.gameObject.GetComponent<Rigidbody>() != null)
        {
            Debug.Log("Colision con: " + collision.gameObject.name);
            grabbedObject = collision.gameObject;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        Debug.Log("Entra deja colision: " + grabbedObject);
        if (collision.gameObject == grabbedObject)
        {
            grabbedObject = null;
            Debug.Log("Finalmente deja colision");
        }
    }

    void Update()
    {
        if (WXRDevice.ButtonHold(device_left, WVR_InputId.WVR_InputId_Alias1_Grip))
        {
            Debug.Log("Pulso grip");
            if (grabbedObject != null)
            {
                Debug.Log("Pulso trigger con el objecto agarrado");
                if (originalParent == null)
                {
                    Debug.Log("Voy a coger el objecto");
                    originalParent = grabbedObject.transform.parent;
                    grabbedObject.transform.SetParent(this.transform);
                    Debug.Log("Voy a coger el objecto: " + originalParent.name);
                }
            }
        }
        else
        {
            if (grabbedObject != null && grabbedObject.transform.parent == this.transform)
            {
                grabbedObject.transform.SetParent(originalParent);
                originalParent = null;
                grabbedObject = null;
            }
        }
    }
}
