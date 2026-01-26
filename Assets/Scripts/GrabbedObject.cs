using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wave.OpenXR;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using Wave.Essence;
using Wave.Native;

public class GrabbedObject : MonoBehaviour
{
    private Rigidbody rb;
    private GameObject grabberObject = null;
    private Transform originalParent = null;
    private Transform rightControllerTransform;


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "WaveRightController") {
            grabberObject = collision.gameObject;
            Debug.Log("Objeto en colision: " + grabberObject);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == grabberObject)
        {
            grabberObject = null;
        }
        Debug.Log("Objeto deja colision: " + grabberObject);
    }

    void Update()
    {
        if (WXRDevice.ButtonHold(WVR_DeviceType.WVR_DeviceType_Controller_Right, WVR_InputId.WVR_InputId_Alias1_Grip))
        {
            //Debug.Log("Grabber object tag: " + grabberObject.tag);
            if (grabberObject.tag == "WaveRightController")
            {
                Debug.Log("colision y coge right controller");
                if (originalParent == null)
                {
                    originalParent = grabberObject.transform.parent;
                    this.transform.SetParent(originalParent);
                }
            }
        }
        else
        {
            //if (grabberObject.tag == "WaveRightController" && grabberObject.transform.parent == rightControllerTransform)
            if (grabberObject.tag == "WaveRightController" && grabberObject.transform.parent == originalParent)
            {
                grabberObject.transform.SetParent(originalParent);
                //rb.isKinematic = false; 
                originalParent = null;
                grabberObject = null;
            }
        }
    }
}