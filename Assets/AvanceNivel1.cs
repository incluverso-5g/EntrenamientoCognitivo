using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Wave.Essence;
using Wave.Native;

public class AvanceNivel1 : MonoBehaviour
{
    private GameObject objetoNiveles;
    private VariablesComunes variablesComunes;
    public int currentLevel;

    private bool errorContado = false;
    private bool aciertoContado = false;

    private int platosRecogidos;
    //private int errores_repeticion;

    //private int numeroPlatos = 2; //private int[] numeroPlatos = new int[] { 1, 2, 3, 5 };

    public bool bajaNivel = false;

    private HashSet<GameObject> countedPlates = new HashSet<GameObject>();
    private Dictionary<GameObject, float> plateTimers = new Dictionary<GameObject, float>();

    private bool lanzarNivelFinalizado = false;
    private bool lanzarError = false;

    private GameObject waveRig;
    LogSaver logSaver;
    VariablesNiveles variablesNiveles;
    Nivel1 nivel1;

    private string grabbedObjectName;

    void Start()
    {
        objetoNiveles = GameObject.Find("CambioNivel");
        variablesComunes = objetoNiveles.GetComponent<VariablesComunes>();
        nivel1 = objetoNiveles.GetComponent<Nivel1>();

        waveRig = GameObject.Find("Wave Rig");
        logSaver = waveRig.GetComponent<LogSaver>();

        variablesNiveles = waveRig.GetComponent<VariablesNiveles>();

        variablesComunes.errores_repeticion = 0;

        //canvasFinalNivel.SetActive(false);
    }

    void Update()
    {
        currentLevel = variablesComunes.currentLevel;
        grabbedObjectName = variablesComunes.grabbedObject != null ? variablesComunes.grabbedObject.name : "None";

        if (variablesNiveles.reiniciaNivel == true)
        {
            ReiniciarNivel();
            variablesNiveles.reiniciaNivel = false;
        }
        else if (variablesNiveles.lanzaError)
        {
            //variablesComunes.errores += 1;
            variablesComunes.errores_repeticion += 1;
            variablesComunes.audioSource_error.Play();
            StartCoroutine(variablesComunes.LaunchError(Camera.main.transform.position, Camera.main.transform.rotation));
            variablesNiveles.lanzaError = false;
        }

        if (variablesNiveles.Recoloca == true)
        {
            RestorePlates();
            variablesNiveles.Recoloca = false;
        }

        if (variablesComunes.grabbedObject != null)
        {
            if (!variablesComunes.grabbedObject.name.Contains("plato_sucio"))
            {
                //Debug.Log("Colision con: " + variablesComunes.grabbedObject.name);

                //VibrateRightController(500, 2);

                if (!errorContado)
                {
                    lanzarError = true;

                    if (lanzarError)
                    {
                        logSaver.SetLogEvent("Lanza_Error");
                        variablesComunes.errores_repeticion += 1;
                        variablesComunes.audioSource_error.Play();
                        StartCoroutine(LaunchError());

                        lanzarError = false;
                    }
                    errorContado = true;
                }
            }
            else
            {
                errorContado = false;
                lanzarError = false;
            }
        }
        else
        {
            errorContado = false;
            lanzarError = false;
        }

        if (variablesComunes.faseActual < variablesComunes.fases_max)
        {
            if (platosRecogidos == variablesComunes.platos_a_recoger)
            {
                variablesComunes.faseActual += 1;
                platosRecogidos = 0;
            }
        }
        else if (variablesComunes.faseActual == variablesComunes.fases_max)
        {
            if (!lanzarNivelFinalizado)
            {
                StartCoroutine(LaunchFinalNivel());
            }
        }  
    }

    void RestorePlates()
    {
        //if (variablesNiveles.Recoloca)
        //{
            for (int i = 0; i < variablesComunes.instantiatedPlatosSucios.Count; i++)
            {
                if (i < variablesComunes.savedPositionsPlatosSucios.Count && i < variablesComunes.savedRotationsPlatosSucios.Count)
                {
                    variablesComunes.instantiatedPlatosSucios[i].transform.position = variablesComunes.savedPositionsPlatosSucios[i];
                    variablesComunes.instantiatedPlatosSucios[i].transform.rotation = Quaternion.Euler(variablesComunes.savedRotationsPlatosSucios[i]);
                }
            }

            for (int i = 0; i < variablesComunes.instantiatedPlatosComida.Count; i++)
            {
                if (i < variablesComunes.savedPositionsPlatosComida.Count && i < variablesComunes.savedRotationsPlatosComida.Count)
                {
                    variablesComunes.instantiatedPlatosComida[i].transform.position = variablesComunes.savedPositionsPlatosComida[i];
                    variablesComunes.instantiatedPlatosComida[i].transform.rotation = Quaternion.Euler(variablesComunes.savedRotationsPlatosComida[i]);

                }
            }

            //variablesNiveles.Recoloca = false;
        //}
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Contains("plato_sucio") && !countedPlates.Contains(collision.gameObject))
        {
            Debug.Log("Acierto");
            StartCoroutine(HandleCollision(collision.gameObject));
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.name.Contains("plato_sucio"))
        {
            errorContado = false;
            
        }
    }

    public IEnumerator LaunchFinalNivel()
    {
        variablesComunes.canvasFinalNivel.SetActive(true);

        Vector3 targetPosition = Camera.main.transform.position + Camera.main.transform.forward * 0.7f + new Vector3(0f, 0.34f, 0f);
        Quaternion cameraRotation = Camera.main.transform.rotation;
        Quaternion yRotation = Quaternion.Euler(0, cameraRotation.eulerAngles.y, 0);

        variablesComunes.canvasFinalNivel.transform.SetPositionAndRotation(targetPosition, yRotation);

        AudioSource audioNivelfin = variablesComunes.canvasFinalNivel.GetComponentInChildren<AudioSource>();
        audioNivelfin.Play();

        lanzarNivelFinalizado = true;

        variablesComunes.repeticiones += 1;
        logSaver.SetLogEvent("Nivel_finalizado_" + SceneManager.GetActiveScene().name + "_" + variablesComunes.currentLevel + "_Repeticion_" + variablesComunes.repeticiones);

        yield return new WaitForSeconds(3f);

        if (variablesComunes.repeticiones < variablesComunes.repeticiones_max)
        {
            variablesComunes.canvasFinalNivel.SetActive(false);
            audioNivelfin.Stop();
            ReiniciarNivel();
            lanzarNivelFinalizado = false;
        }
        //else
        //{
            //logSaver.SetLogEvent("Fin_Niveles");
        //}
    }

    public IEnumerator LaunchError()
    {
        variablesComunes.canvasError.SetActive(true);

        Vector3 targetPosition = Camera.main.transform.position + Camera.main.transform.forward * 0.5f + new Vector3(0f, 0.34f, 0f);
        Quaternion cameraRotation = Camera.main.transform.rotation;
        Quaternion yRotation = Quaternion.Euler(0, cameraRotation.eulerAngles.y, 0);

        variablesComunes.canvasError.transform.SetPositionAndRotation(targetPosition, yRotation);

        yield return new WaitForSeconds(2f);

        variablesComunes.canvasError.SetActive(false);
        variablesComunes.audioSource_error.Stop();

        bool errorLaunched = false;

        if (variablesComunes.errores_repeticion >= 3) // && variablesComunes.errores > 0)
        {
            variablesComunes.errores -= 1;
            variablesComunes.errores_repeticion = 0;

            if (variablesComunes.errores > 0)
            {
                StartCoroutine(WaitUntilResetLevel());
                logSaver.SetLogEvent("3Errores_Nivel_" + variablesComunes.currentLevel + "_ReiniciaNivel");
                errorLaunched = true;
            }
            else if(variablesComunes.errores <= 0)
            {
                logSaver.SetLogEvent("Pierde3Vidas_Nivel_" + variablesComunes.currentLevel + "_VuelveNivelAnterior");
                StartCoroutine(WaitUntilLowerLevel());
            }
            
            
        }

        //if (variablesComunes.errores <= 0 && !errorLaunched)
        //{
            //if (variablesComunes.currentLevel > 1)
            //{
                //logSaver.SetLogEvent("Pierde3Vidas_Nivel_" + variablesComunes.currentLevel + "_VuelveNivelAnterior");
                //StartCoroutine(WaitUntilLowerLevel());
            //}
            /*
            else if(variablesComunes.currentLevel == 1)
            {
                logSaver.SetLogEvent("Pierde3Vidas_Nivel_" + variablesComunes.currentLevel);
            } */
        //}
    }

    IEnumerator WaitUntilResetLevel()
    {
        variablesComunes.Launch_Supera_Errores(Camera.main.transform.position, Camera.main.transform.rotation);
        yield return new WaitForSeconds(5f);
        variablesComunes.audioSource_supera_erroes.Stop();
        variablesComunes.canvasSuperaErrores.SetActive(false);
        ReiniciarNivel();
    }

    IEnumerator WaitUntilLowerLevel()
    {
        variablesComunes.Launch_Pierde_Vidas(Camera.main.transform.position, Camera.main.transform.rotation);
        yield return new WaitForSeconds(5f);
        variablesComunes.audioSource_supera_erroes.Stop();
        variablesComunes.canvasSuperaErrores.SetActive(false);
        //variablesComunes.currentLevel -= 1;
        //ReiniciarNivel();
    }

    private void ReiniciarNivel()
    {
        countedPlates = new HashSet<GameObject>();
        plateTimers = new Dictionary<GameObject, float>();

        variablesComunes.faseActual = 0;
        variablesComunes.faseAnterior = -1;
        variablesComunes.errores_repeticion = 0;
        platosRecogidos = 0;

        GameObject[] allPlates = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject plate in allPlates)
        {
            if (plate.name.Contains("plato_sucio(Clone)") || plate.name.Contains("Plate") || plate.name.Contains("plato_sucio_alitas(Clone)") || plate.name.Contains("plato_sucio_fruta(Clone)") || plate.name.Contains("plato_sucio_pizza(Clone)"))
            {
                plate.SetActive(false);
            }
        }
    }

    IEnumerator HandleCollision(GameObject plate)
    {
        if (!aciertoContado)
        {
            variablesComunes.LaunchSuccess(waveRig.transform.position, waveRig.transform.rotation);
            variablesComunes.audioSource_ok.Play();
            aciertoContado = true;
        }
 
        plateTimers[plate] = Time.time + 3f;

        yield return new WaitUntil(() => Time.time >= plateTimers[plate]);

        if (!countedPlates.Contains(plate))
        {            
            logSaver.SetLogEvent("Lanza_Acierto");
            countedPlates.Add(plate);
            plate.SetActive(false);
            variablesComunes.tick.SetActive(false);
            variablesComunes.audioSource_ok.Stop();
            platosRecogidos += 1;
            aciertoContado = false;
            
        }
    }    
}
