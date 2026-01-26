using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Wave.Essence;
using Wave.Native;

public class NoPasarZonaNoJuego : MonoBehaviour
{
    private GameObject prefabNoPasar;

    private GameObject areaJuegoDcha;
    private GameObject areaJuegoIzq;
    private GameObject areaJuegoDelante;
    private GameObject areaJuegoDetras;

    void Start()
    {
        areaJuegoDcha = GameObject.Find("AreaJuego2");
        areaJuegoIzq = GameObject.Find("AreaJuego");
        areaJuegoDelante = GameObject.Find("AreaJuego3");
        areaJuegoDetras = GameObject.Find("AreaJuego4");

        prefabNoPasar = GameObject.Find("Prohibido_Paso");
    }

    void Update()
    {
        if (Camera.main.transform.position.z > areaJuegoDelante.transform.position.z)
        {
            LanzaNoPasar();
        }
        else if (Camera.main.transform.position.z < areaJuegoDetras.transform.position.z)
        {
            LanzaNoPasar();
        }
        else if (Camera.main.transform.position.x < areaJuegoIzq.transform.position.x)
        {
            LanzaNoPasar();
        }
        else if (Camera.main.transform.position.x > areaJuegoDcha.transform.position.x)
        {
            LanzaNoPasar();
        }
        else
        {
            prefabNoPasar.SetActive(false);
        }
    }

    private void LanzaNoPasar()
    {
        prefabNoPasar.SetActive(true);

        Vector3 targetPosition = Camera.main.transform.position + Camera.main.transform.forward * 0.4f + new Vector3(0f, 0f, 0f);
        Quaternion cameraRotation = Camera.main.transform.rotation;
        Quaternion yRotation = Quaternion.Euler(0, cameraRotation.eulerAngles.y, 0);

        prefabNoPasar.transform.SetPositionAndRotation(targetPosition, yRotation);
        VibrateRightController(500, 1);
    }

    void VibrateRightController(uint duration, uint frequency)
    {
        Interop.WVR_TriggerVibration(
            WVR_DeviceType.WVR_DeviceType_Controller_Right,
            WVR_InputId.WVR_InputId_17,
            duration,
            frequency,
            WVR_Intensity.WVR_Intensity_Normal
        );
    }
}
