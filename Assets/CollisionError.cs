using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionError : MonoBehaviour
{
    private GameObject objetoNiveles;
    private VariablesComunes variablesComunes;

    private bool lanzarError = false;
    private bool errorContado = false;

    private GameObject waveRig;
    LogSaver logSaver;

    void Start()
    {
        objetoNiveles = GameObject.Find("CambioNivel");
        variablesComunes = objetoNiveles.GetComponent<VariablesComunes>();

        waveRig = GameObject.Find("Wave Rig");
        logSaver = waveRig.GetComponent<LogSaver>();
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Contains("plato_sucio(Clone)"))
        {
            if (!errorContado)
            {
                logSaver.SetLogEvent("Lanza_Error");
                variablesComunes.errores += 1;
                variablesComunes.audioSource_error.Play();
                StartCoroutine(variablesComunes.LaunchError(Camera.main.transform.position, Camera.main.transform.rotation));

                errorContado = true;
            } 
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.name.Contains("plato_sucio(Clone)"))
        {
            errorContado = false;
        }
    }
}
