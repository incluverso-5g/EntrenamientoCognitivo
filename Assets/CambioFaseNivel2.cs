using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioFaseNivel2 : MonoBehaviour
{
    private GameObject objetoNiveles;
    private VariablesComunes variablesComunes;
    public int currentLevel;

    private CambioFaseNivel1 cambioFaseN1;

    private GameObject colisionPlatos;

    private bool errorContado = false;

    //public int faseActual;
    private int platosRecogidos;

    private int[] numeroPlatos = new int[] { 1, 2, 4, 6 };

    public bool siguienteNivel;
    public bool bajaNivel = false;

    private float distance = 0.5f;
    private float yOffset = 0.34f;

    private HashSet<GameObject> countedPlates = new HashSet<GameObject>();
    private Dictionary<GameObject, float> plateTimers = new Dictionary<GameObject, float>();

    private bool lanzarNivelFinalizado = false;
    private bool lanzarError = false;

    private GameObject waveRig;
    LogSaver logSaver;
    VariablesNiveles variablesNiveles;

    private string grabbedObjectName;


    void Start()
    {
        objetoNiveles = GameObject.Find("CambioNivel");
        variablesComunes = objetoNiveles.GetComponent<VariablesComunes>();

        waveRig = GameObject.Find("Wave Rig");
        logSaver = waveRig.GetComponent<LogSaver>();

        variablesNiveles = waveRig.GetComponent<VariablesNiveles>();
        cambioFaseN1 = GetComponent<CambioFaseNivel1>();
    }

    void Update()
    {
        currentLevel = variablesComunes.currentLevel;
        grabbedObjectName = variablesComunes.grabbedObject != null ? variablesComunes.grabbedObject.name : "None";

        if (currentLevel == 2)
        {
            if (variablesNiveles.reiniciaNivel == true)
            {
                ReiniciarNivel();
                variablesNiveles.reiniciaNivel = false;
            }
            else if (variablesNiveles.lanzaError)
            {
                variablesComunes.errores += 1;
                variablesComunes.audioSource_error.Play();
                StartCoroutine(variablesComunes.LaunchError(Camera.main.transform.position, Camera.main.transform.rotation));
                variablesNiveles.lanzaError = false;
            }

            if (variablesComunes.grabbedObject != null)
            {
                if (!variablesComunes.grabbedObject.name.Contains("plato_sucio"))
                {
                    Debug.Log("Colision con: " + variablesComunes.grabbedObject.name);

                    if (!errorContado)
                    {
                        lanzarError = true;

                        if (lanzarError)
                        {
                            logSaver.SetLogEvent("Lanza_Error");
                            variablesComunes.errores += 1;
                            variablesComunes.audioSource_error.Play();
                            StartCoroutine(variablesComunes.LaunchError(Camera.main.transform.position, Camera.main.transform.rotation));
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

            if (variablesComunes.faseActual < 4)
            {
                if (platosRecogidos == numeroPlatos[variablesComunes.faseActual])
                {
                    variablesComunes.faseActual += 1;
                    platosRecogidos = 0;
                }
            }
            else if (variablesComunes.faseActual == 4)
            {
                if (!lanzarNivelFinalizado)
                {
                    cambioFaseN1.canvasFinalNivel.SetActive(true);

                    Vector3 targetPosition = Camera.main.transform.position + Camera.main.transform.forward * distance + new Vector3(0f, yOffset, 0f);
                    Quaternion cameraRotation = Camera.main.transform.rotation;
                    Quaternion yRotation = Quaternion.Euler(0, cameraRotation.eulerAngles.y, 0);

                    cambioFaseN1.canvasFinalNivel.transform.SetPositionAndRotation(targetPosition, yRotation);

                    AudioSource audioNivelfin = cambioFaseN1.canvasFinalNivel.GetComponentInChildren<AudioSource>();
                    audioNivelfin.Play();

                    lanzarNivelFinalizado = true;

                    logSaver.SetLogEvent("Nivel_finalizado_" + SceneManager.GetActiveScene().name + "_" + variablesComunes.currentLevel);

                    //StartCoroutine(WaitUntilNextLevel());
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (currentLevel == 2)
        {
            if (collision.gameObject.name.Contains("plato_sucio") && !countedPlates.Contains(collision.gameObject))
            {
                Debug.Log("Acierto");
                StartCoroutine(HandleCollision(collision.gameObject));
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.name.Contains("plato_sucio"))
        {
            errorContado = false;
        }
    }

    private void ResetPlatePosition(GameObject plate, Vector3 posMesa, Vector3 rotMesa)
    {
        plate.transform.SetPositionAndRotation(posMesa, Quaternion.Euler(0, rotMesa.y, rotMesa.z));
    }

    private void ReiniciarNivel()
    {
        countedPlates = new HashSet<GameObject>();
        plateTimers = new Dictionary<GameObject, float>();

        variablesComunes.faseActual = 0;
        platosRecogidos = 0;
        siguienteNivel = false;

        GameObject[] allPlates = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject plate in allPlates)
        {
            if (plate.name.Contains("plato_sucio(Clone)"))
            {
                plate.SetActive(false);
            }
        }
    }

    IEnumerator HandleCollision(GameObject plate)
    {
        variablesComunes.LaunchSuccess(waveRig.transform.position, waveRig.transform.rotation);
        variablesComunes.audioSource_ok.Play();

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
        }
    }
}
