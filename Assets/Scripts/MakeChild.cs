using UnityEngine;
using Wave.OpenXR;
using Wave.Essence;
using Wave.Native;

public class MakeChild : MonoBehaviour
{
    private Transform currentPlate;
    private Rigidbody cakeRigidbody;  // Reference to the Rigidbody component on the cake
    private WVR_DeviceType device_right = WVR_DeviceType.WVR_DeviceType_Controller_Right;
    private WVR_DeviceType device_left = WVR_DeviceType.WVR_DeviceType_Controller_Left;

    private void Start()
    {
        // Get the Rigidbody component on the cake, if it exists
        cakeRigidbody = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Contains("Plate"))
        {
            Transform plateTransform = collision.transform;

            // If already attached to another plate, detach from it
            if (currentPlate != null && currentPlate != plateTransform)
            {
                DetachFromPlate();
            }

            // Attach to the new plate
            AttachToPlate(plateTransform);
        }
    }

    private void AttachToPlate(Transform plateTransform)
    {
        transform.SetParent(plateTransform);
        currentPlate = plateTransform;

        // Disable the Rigidbody if it exists, to avoid extra weight
        if (cakeRigidbody != null)
        {
            cakeRigidbody.isKinematic = true;
        }
    }

    private void DetachFromPlate()
    {
        transform.SetParent(null);
        currentPlate = null;

        // Re-enable the Rigidbody to restore normal physics
        if (cakeRigidbody != null)
        {
            cakeRigidbody.isKinematic = false;
        }
    }

    /*
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Plate"))
        {
            Transform plateTransform = collision.transform;
            if (plateTransform == currentPlate)
            {
                DetachFromPlate();
            }
        }
    }
    */
}
