using UnityEngine;
using Wave.OpenXR;
using Wave.Essence;
using Wave.Native;

public class MakeDetatchChild : MonoBehaviour
{
    private Transform currentPlate;
    private WVR_DeviceType device_right = WVR_DeviceType.WVR_DeviceType_Controller_Right;
    private WVR_DeviceType device_left = WVR_DeviceType.WVR_DeviceType_Controller_Left;
    private bool isGrabbed = false;
    private bool grab_food = false;

    
    private void Awake()
    {
        if (grab_food == true) {
            DetachFromParent(this.gameObject);
            currentPlate = null;
        }
    }
    

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Plate") && !grab_food)
        {
            Transform plateTransform = collision.transform;

            if (currentPlate != null && currentPlate != plateTransform)
            {
                transform.SetParent(null);
            }

            transform.SetParent(plateTransform);
            currentPlate = plateTransform;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Plate") && !isGrabbed && !grab_food)
        {
            Transform plateTransform = collision.transform;
            if (plateTransform == currentPlate)
            {
                transform.SetParent(null);
                currentPlate = null;
            }
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("WaveRightController") || other.CompareTag("WaveLeftController"))
        {
            if (WXRDevice.ButtonHold(device_right, WVR_InputId.WVR_InputId_Alias1_Grip) || WXRDevice.ButtonHold(device_left, WVR_InputId.WVR_InputId_Alias1_Grip))
            {
                grab_food = true;
            }
            else
            {
                grab_food = false;
            }
        }
    }

    
    public void DetachFromParent(GameObject obj)
    {
        if (obj.transform.parent != null)
        {
            obj.transform.SetParent(null);
        }
    }
    
}
