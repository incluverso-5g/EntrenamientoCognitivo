using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wave.OpenXR;
using Wave.Essence;
using Wave.Native;

public class GrabObject_David : MonoBehaviour
{
    public GameObject grabbedObject = null;
    private Transform originalParent = null;
    private Rigidbody grabbedRigidbody = null;
    private WVR_DeviceType device_right = WVR_DeviceType.WVR_DeviceType_Controller_Right;
    private WVR_DeviceType device_left = WVR_DeviceType.WVR_DeviceType_Controller_Left;
    private bool grabbed = false;

    //To check grabbed object
    private GameObject waveRig;
    VariablesNiveles variablesNiveles;

    private void Start()
    {
        waveRig = GameObject.Find("Wave Rig");
        variablesNiveles = waveRig.GetComponent<VariablesNiveles>();
        variablesNiveles.grabbedObject = null;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (!grabbed && collider.attachedRigidbody != null && !collider.CompareTag("NotGrabbable"))
        {
            Debug.Log("Collision with: " + collider.gameObject.name);
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
        //Debug.Log("Grabbed object: " + variablesNiveles.grabbedObject.name);

        Vector3 rightControllerPosition = Vector3.zero;

        bool success = WXRDevice.GetControllerPosition(XR_Hand.Right, ref rightControllerPosition);

        if (success)
        {

            Debug.Log("Right Controller Global Position: " + rightControllerPosition);
        }
        else
        {
            Debug.LogError("Failed to get right controller position.");
        }

        // Y Position of the controller: 
        float YRightControllerPosition = rightControllerPosition.y;

        if (WXRDevice.ButtonHold(device_right, WVR_InputId.WVR_InputId_Alias1_Grip))
        {
            if (grabbedObject != null && !grabbed)
            {
                Grab();
            }
            else if (grabbedObject != null &&  grabbed && YRightControllerPosition < grabbedRigidbody.transform.position.y)
            {
                Release();

                //Debug.Log("Position of Right Controller (Colisión): " + YRightControllerPosition);
                //Debug.Log("Position of Plate (Colisión) " + grabbedRigidbody.transform.position.y);
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
        variablesNiveles.grabbedObject = grabbedObject;
        originalParent = grabbedObject.transform.parent;
        grabbedObject.transform.SetParent(this.transform, true);

        // Obtener o agregar Rigidbody
        grabbedRigidbody = grabbedObject.GetComponent<Rigidbody>();

        if (grabbedRigidbody == null)
        {
            grabbedRigidbody = grabbedObject.AddComponent<Rigidbody>();
        }

        // Configuración del Rigidbody para evitar rebotes
        grabbedRigidbody.isKinematic = false;
        grabbedRigidbody.useGravity = false;
        grabbedRigidbody.detectCollisions = true;
        grabbedRigidbody.drag = 5f; // Aumenta la fricción lineal para amortiguar el movimiento
        grabbedRigidbody.angularDrag = 5f; // Aumenta la fricción angular para evitar giros bruscos

        // Crear y asignar Physic Material para evitar rebotes
        PhysicMaterial noBounceMaterial = new PhysicMaterial();
        noBounceMaterial.bounciness = 0;
        noBounceMaterial.dynamicFriction = 0.6f;
        noBounceMaterial.staticFriction = 0.6f;
        noBounceMaterial.bounceCombine = PhysicMaterialCombine.Minimum;
        noBounceMaterial.frictionCombine = PhysicMaterialCombine.Multiply;

        // Asigna el material al collider
        Collider collider = grabbedObject.GetComponent<Collider>();
        if (collider != null)
        {
            collider.material = noBounceMaterial;
        }

        Debug.Log("Grabbed object: " + grabbedObject.name);

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
        variablesNiveles.grabbedObject = null;
        originalParent = null;
    }
}



/*

*/