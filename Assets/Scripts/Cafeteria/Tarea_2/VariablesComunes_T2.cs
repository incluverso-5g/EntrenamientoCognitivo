using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariablesComunes_T2 : MonoBehaviour
{
    public int currentLevel;
    public int faseActual;
    public int repeticiones;

    public int repeticiones_max;
    public int platos_a_servir;
    public int fases_max;

    VariablesNiveles variablesNiveles;

    //public Vector4[] posBarra = new Vector4[] { new Vector3(2.67f, 0.62f, 1.43f), new Vector3(2.67f, 0.62f, 1.08f), new Vector3(2.67f, 0.62f, 0.73f), new Vector3(2.67f, 0.62f, 0.38f) };
    //public Vector4[] rotBarra = new Vector4[] { new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0) };

    public List<Vector3> posBarra = new List<Vector3>
    {
        new Vector3(4.17f, 0.62f, 1.43f),
        new Vector3(4.17f, 0.62f, 1.08f),
        new Vector3(4.17f, 0.62f, 0.73f),
        new Vector3(4.17f, 0.62f, 0.38f)
    };

    public List<GameObject> instantiatedDrinksBocadilloM1 = new List<GameObject>();
    public List<GameObject> instantiatedDrinksBocadilloM2 = new List<GameObject>();
    public List<GameObject> instantiatedDrinksBocadilloM3 = new List<GameObject>();

    public List<GameObject> instantiatedDrinksBarra = new List<GameObject>();
    public List<GameObject> instantiatedDrinksBocadillo = new List<GameObject>();

    public GameObject prefabCocaCola;
    public GameObject prefabCafe;
    public GameObject prefabFanta;

    public bool baja_nivel_entrenamiento;

    private GameObject WaveRig;
    LogSaver logSaver;

    public GameObject tick;
    public AudioSource audioSource_ok;

    public GameObject canvasError;
    public AudioSource audioSource_error;

    public GameObject canvasFinalNivel;

    public GameObject canvasSuperaErrores;
    public AudioSource audioSource_supera_erroes;

    public GameObject pierdeTodasLasVidas;
    public AudioSource audioSource_pierdeTodasLasVidas;

    public int errores;
    public int vidas; 
    public GameObject grabbedObject = null;

    public Dictionary<GameObject, (Vector3, Quaternion)> objetosPosicionesIniciales = new Dictionary<GameObject, (Vector3, Quaternion)>();

    private void Start()
    {
        //currentLevel = 0;
        faseActual = 0;
        baja_nivel_entrenamiento = false;
        errores = 0;
        vidas = 3; 

        repeticiones = 0;
        repeticiones_max = 3;

        WaveRig = GameObject.Find("Wave Rig");
        logSaver = WaveRig.GetComponent<LogSaver>();

        tick = GameObject.Find("Nivel_ok");
        audioSource_ok = tick.GetComponentInChildren<AudioSource>();
        tick.SetActive(false);

        canvasError = GameObject.Find("Nivel_erroneo");
        audioSource_error = canvasError.GetComponentInChildren<AudioSource>();
        canvasError.SetActive(false);

        canvasFinalNivel = GameObject.Find("Nivel_finalizado");
        canvasFinalNivel.SetActive(false);

        canvasSuperaErrores = GameObject.Find("Supera_Errores");
        audioSource_supera_erroes = canvasSuperaErrores.GetComponentInChildren<AudioSource>();
        canvasSuperaErrores.SetActive(false);

        pierdeTodasLasVidas = GameObject.Find("3Errores");
        audioSource_pierdeTodasLasVidas = pierdeTodasLasVidas.GetComponentInChildren<AudioSource>();
        pierdeTodasLasVidas.SetActive(false);
    }

    private void Update()
    {
        variablesNiveles = WaveRig.GetComponent<VariablesNiveles>();
        variablesNiveles.erroresNivel = errores;
        variablesNiveles.corazones = vidas;
        currentLevel = variablesNiveles.nivel_cafeteria_T2;
        //currentLevel = 6;

        //Debug.Log("Errores nivel: " + errores);
        //Debug.Log("Corazones nivel: " + vidas);

        if (currentLevel == 1)
        {
            platos_a_servir = 1;
            fases_max = 5;
        }
        else if(currentLevel == 2)
        {
            platos_a_servir = 2;
            fases_max = 3;
        }
        else if (currentLevel == 3)
        {
            platos_a_servir = 3;
            fases_max = 2;
        }
        else if (currentLevel == 4)
        {
            platos_a_servir = 3;
            fases_max = 2;
        }
        else if (currentLevel == 5)
        {
            platos_a_servir = 6;
            fases_max = 1;
        }
        else if (currentLevel == 6)
        {
            platos_a_servir = 6;
            fases_max = 1;
        }


        grabbedObject = variablesNiveles.grabbedObject;
    }

    public GameObject CreateNewPlateRandomList(GameObject prefab, Vector3 posMesa, Vector3 rotMesa)
    {
        GameObject newPlato = Instantiate(prefab, posMesa, Quaternion.Euler(rotMesa.x, rotMesa.y, rotMesa.z));
        
        return newPlato;
    }

    public void LaunchSuccess()
    {
        tick.SetActive(true);

        Vector3 targetPosition = Camera.main.transform.position + Camera.main.transform.forward * 0.5f + new Vector3(0f, 0.34f, 0f);
        Quaternion cameraRotation = Camera.main.transform.rotation;
        Quaternion yRotation = Quaternion.Euler(0, cameraRotation.eulerAngles.y, 0);

        tick.transform.SetPositionAndRotation(targetPosition, yRotation);
    }

    public IEnumerator LaunchError()
    {
        canvasError.SetActive(true);
        audioSource_error.Play();

        Vector3 targetPosition = Camera.main.transform.position + Camera.main.transform.forward * 0.5f + new Vector3(0f, 0.34f, 0f);
        Quaternion cameraRotation = Camera.main.transform.rotation;
        Quaternion yRotation = Quaternion.Euler(0, cameraRotation.eulerAngles.y, 0);

        canvasError.transform.SetPositionAndRotation(targetPosition, yRotation);

        yield return new WaitForSeconds(2f);

        canvasError.SetActive(false);
        audioSource_error.Stop();
    }

    public void ShuffleList(List<Vector3> listToShuffle)
    {
        for (int i = listToShuffle.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);

            // Swap elements at indices i and j
            Vector3 temp = listToShuffle[i];
            listToShuffle[i] = listToShuffle[j];
            listToShuffle[j] = temp;
        }
    }

    public List<GameObject> ShuffleListGameObject(List<GameObject> listToShuffle)
    {
        for (int i = listToShuffle.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);

            GameObject temp = listToShuffle[i];
            listToShuffle[i] = listToShuffle[j];
            listToShuffle[j] = temp;
        }

        return listToShuffle;
    }


    public void ShuffleArraysV3(Vector3[] array1, Vector3[] array2)
    {
        for (int i = array1.Length - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);

            Vector3 temp1 = array1[i];
            array1[i] = array1[j];
            array1[j] = temp1;

            Vector3 temp2 = array2[i];
            array2[i] = array2[j];
            array2[j] = temp2;
        }
    }

    public void ShuffleArraysV4(Vector4[] array1, Vector4[] array2)
    {
        for (int i = array1.Length - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);

            Vector3 temp1 = array1[i];
            array1[i] = array1[j];
            array1[j] = temp1;

            Vector3 temp2 = array2[i];
            array2[i] = array2[j];
            array2[j] = temp2;
        }
    }

    public bool IsExpectedDrink(GameObject drink)
    {
        foreach (GameObject expectedDrink in instantiatedDrinksBocadilloM1)
        {
            if (drink.name.Contains(expectedDrink.name))
            {
                return true;
            }
        }
        return false;
    }

    public void Launch_Supera_Errores(Vector3 posCabeza, Quaternion rotCabeza)
    {
        canvasSuperaErrores.SetActive(true);

        Vector3 targetPosition = Camera.main.transform.position + Camera.main.transform.forward * 0.7f + new Vector3(0f, 0.34f, 0f);
        Quaternion cameraRotation = Camera.main.transform.rotation;
        Quaternion yRotation = Quaternion.Euler(0, cameraRotation.eulerAngles.y, 0);

        audioSource_supera_erroes.Play();
        canvasSuperaErrores.transform.SetPositionAndRotation(targetPosition, yRotation);
    }

    public void Launch_Pierde_Vidas(Vector3 posCabeza, Quaternion rotCabeza)
    {
        pierdeTodasLasVidas.SetActive(true);

        Vector3 targetPosition = Camera.main.transform.position + Camera.main.transform.forward * 0.7f + new Vector3(0f, 0.34f, 0f);
        Quaternion cameraRotation = Camera.main.transform.rotation;
        Quaternion yRotation = Quaternion.Euler(0, cameraRotation.eulerAngles.y, 0);

        audioSource_pierdeTodasLasVidas.Play();
        pierdeTodasLasVidas.transform.SetPositionAndRotation(targetPosition, yRotation);
    }
}
