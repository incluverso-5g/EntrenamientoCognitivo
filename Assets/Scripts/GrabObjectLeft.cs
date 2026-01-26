using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wave.OpenXR;
using Wave.Essence;
using Wave.Native;

public class GrabObjectLeft : MonoBehaviour
{
    private GameObject grabbedObject = null;
    private Transform originalParent = null;
    private Rigidbody grabbedRigidbody = null;
    //private WVR_DeviceType device_right = WVR_DeviceType.WVR_DeviceType_Controller_Right;
    private WVR_DeviceType device_left = WVR_DeviceType.WVR_DeviceType_Controller_Left;
    private bool grabbed = false;

    void OnTriggerEnter(Collider collider)
    {
        if (!grabbed && collider.attachedRigidbody != null)
        {
            grabbedObject = collider.gameObject;
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject == grabbedObject && !grabbed)
        {
            grabbedObject = null;
        }
    }

    void Update()
    {
        if (WXRDevice.ButtonHold(device_left, WVR_InputId.WVR_InputId_Alias1_Grip))
        {
            if (grabbedObject != null && !grabbed)// && !grabbedObject.CompareTag("NotGrabbable"))
            {
                Grab();
            }
        }
        else
        {
            if (grabbed)
            {
                Release();
            }
        }
    }

    public void Grab()
    {
        grabbed = true;
        originalParent = grabbedObject.transform.parent;
        grabbedObject.transform.SetParent(this.transform);
        grabbedRigidbody = grabbedObject.GetComponent<Rigidbody>();
        //Debug.Log("Grab rigid body: " + grabbedRigidbody);

        if (grabbedRigidbody != null)
        {
            grabbedRigidbody.isKinematic = true;
            grabbedRigidbody.useGravity = false;
        }
    }

    private void Release()
    {
        grabbed = false;
        grabbedObject.transform.SetParent(originalParent);

        if (grabbedRigidbody != null)
        {
            grabbedRigidbody.isKinematic = false;
            grabbedRigidbody.useGravity = true;
            grabbedRigidbody = null;
        }

        grabbedObject = null;
        originalParent = null;
    }
}
