using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wave.Essence;
using Wave.Native;
public class DeactivateInstructions : MonoBehaviour
{
    GameObject waveRig;
    VariablesNiveles variablesNiveles;

    private GameObject canvasInstrucciones;
    
    private void Awake()
    {
        waveRig = GameObject.Find("Wave Rig");
        variablesNiveles = waveRig.GetComponent<VariablesNiveles>();

        canvasInstrucciones = GameObject.Find("Instrucciones_Iniciales");
        canvasInstrucciones.SetActive(false);
    }

    private void Start()
    {
        //StartCoroutine(WaitSeconds());
    }

    void Update()
    {
        if (variablesNiveles.lanzaInstrucciones == true)
        {
            canvasInstrucciones.SetActive(true);
        }
        else if (variablesNiveles.lanzaInstrucciones == false)
        {
            canvasInstrucciones.SetActive(false);
        }
        /*
        if (WXRDevice.ButtonHold(WVR_DeviceType.WVR_DeviceType_Controller_Right, WVR_InputId.WVR_InputId_Alias1_B))
        {
            canvasInstrucciones.SetActive(false);
        }
        */
    }


    IEnumerator WaitSeconds()
    {
        yield return new WaitForSeconds(3);
        canvasInstrucciones.SetActive(true);

        float distance = 1.095f;
        float yOffset = 0.34f;

        Vector3 targetPosition = Camera.main.transform.position + Camera.main.transform.forward * distance + new Vector3(0f, yOffset, 0f);
        Quaternion cameraRotation = Camera.main.transform.rotation;
        Quaternion yRotation = Quaternion.Euler(0, cameraRotation.eulerAngles.y, 0);

        canvasInstrucciones.transform.SetPositionAndRotation(targetPosition, yRotation);


    }
}
