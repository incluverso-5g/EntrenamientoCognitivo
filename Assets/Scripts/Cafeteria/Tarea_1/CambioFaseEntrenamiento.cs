using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioFaseEntrenamiento : MonoBehaviour
{
    private GameObject objetoNiveles;
    private VariablesComunes variablesComunes;

    //public int faseActual;
    private int platosRecogidos;

    private int[] numeroPlatos = new int[] { 1, 2, 4, 8 };

    public bool siguienteNivel;

    private GameObject canvasFinalNivel;
    private float distance = 1.095f;
    private float yOffset = 0.34f;

    private HashSet<GameObject> countedPlates;
    private Dictionary<GameObject, float> plateTimers;

    private bool lanzarNivelFinalizado = false;

    private bool baja_nivel_entrenamiento;

    private GameObject WaveRig;
    private VariablesNiveles variablesNiveles;
    LogSaver logSaver;

    private void Awake()
    {
        canvasFinalNivel = GameObject.Find("Nivel_finalizado");
        canvasFinalNivel.SetActive(false);
        countedPlates = new HashSet<GameObject>();
        plateTimers = new Dictionary<GameObject, float>();
    }

    void Start()
    {
        objetoNiveles = GameObject.Find("CambioNivel");
        variablesComunes = objetoNiveles.GetComponent<VariablesComunes>();

        platosRecogidos = 0;
        siguienteNivel = false;

        WaveRig = GameObject.Find("Wave Rig");
        variablesNiveles = WaveRig.GetComponent<VariablesNiveles>();
        logSaver = WaveRig.GetComponent<LogSaver>();
    }

    void Update()
    {
        baja_nivel_entrenamiento = variablesComunes.baja_nivel_entrenamiento;
        

        if (baja_nivel_entrenamiento) 
        {
            ReiniciarNivel();
            variablesComunes.baja_nivel_entrenamiento = false;
        }
        else if (variablesNiveles.reiniciaNivel == true)
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
            //Debug.Log("Entra aqui");
            if (!lanzarNivelFinalizado)
            {
                canvasFinalNivel.SetActive(true);

                Vector3 targetPosition = Camera.main.transform.position + Camera.main.transform.forward * distance + new Vector3(0f, yOffset, 0f);
                Quaternion cameraRotation = Camera.main.transform.rotation;
                Quaternion yRotation = Quaternion.Euler(0, cameraRotation.eulerAngles.y, 0);

                canvasFinalNivel.transform.SetPositionAndRotation(targetPosition, yRotation);

                AudioSource audioNivelfin = canvasFinalNivel.GetComponentInChildren<AudioSource>();
                audioNivelfin.Play();

                lanzarNivelFinalizado = true;

                logSaver.SetLogEvent("Nivel_finalizado_" + SceneManager.GetActiveScene().name);

                //StartCoroutine(WaitUntilNextLevel());
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Contains("plato_sucio(Clone)") && !countedPlates.Contains(collision.gameObject)) //plate.name.Contains("plato_sucio(Clone)")
        {
            StartCoroutine(HandleCollision(collision.gameObject));
        }
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
        variablesComunes.LaunchSuccess(WaveRig.transform.position, WaveRig.transform.rotation);
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

    IEnumerator WaitUntilNextLevel() 
    {
        yield return new WaitForSeconds(10f);

        siguienteNivel = true;
        //variablesComunes.faseActual = 5;
        canvasFinalNivel.SetActive(false);

        SceneManager.LoadScene("Cafeteria_Tarea_1");

        //currentLevel += 1;
        //variablesComunes.faseActual = 0;
        //logSaver.SetLogEvent("Nivel_" + currentLevel);
        //variablesComunes.currentLevel = currentLevel;
    }

}
/*else if (variablesNiveles.lanzaAcierto)
        {
            variablesComunes.audioSource_ok.Play();
            variablesComunes.LaunchSuccess(Camera.main.transform.position, Camera.main.transform.rotation);
            StartCoroutine(variablesComunes.WaitSeconds(variablesComunes.tick));
            variablesComunes.audioSource_ok.Stop();
            variablesNiveles.lanzaAcierto = false;
        }*/