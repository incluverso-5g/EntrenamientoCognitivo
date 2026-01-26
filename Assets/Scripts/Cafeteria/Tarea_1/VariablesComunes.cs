using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VariablesComunes : MonoBehaviour
{
    public int currentLevel;
    public int faseActual;
    public int faseAnterior;
    public int repeticiones;

    public int repeticiones_max;
    public int platos_a_recoger;
    public int fases_max;

    public Vector3[] posMesa1 = new Vector3[] { new Vector3(-0.086f, 0.801f, 2.003f), new Vector3(0.515f, 0.801f, 2.003f), new Vector3(0.179f, 0.801f, 1.846f) };
    public Vector3[] rotMesa1 = new Vector3[] { new Vector3(-90, 0, 90), new Vector3(-90, -90, 0), new Vector3(-90, 0, 0) };

    public Vector4[] posMesa2 = new Vector4[] { new Vector3(-0.556f, 0.801f, 0.239f), new Vector3(-1.181f, 0.801f, 0.239f), new Vector3(-0.89f, 0.801f, 0.58f), new Vector3(-0.89f, 0.801f, -0.071f) };
    public Vector4[] rotMesa2 = new Vector4[] { new Vector3(-90, 0, -90), new Vector3(-90, 0, 90), new Vector3(-90, 0, 180), new Vector3(-90, 0, 0) };

    public Vector3[] posMesa3 = new Vector3[] { new Vector3(-0.147f, 0.801f, -2.207f), new Vector3(-0.196f, 0.801f, -1.917f), new Vector3(-0.556f, 0.801f, -2.09f) };
    public Vector3[] rotMesa3 = new Vector3[] { new Vector3(-90, 0, 0), new Vector3(-90, 0, 180), new Vector3(-90, 0, 90) };

    public bool baja_nivel_entrenamiento;

    private GameObject WaveRig;
    LogSaver logSaver;
    VariablesNiveles variablesNiveles;
    public GameObject grabbedObject = null;

    public GameObject tick;
    public AudioSource audioSource_ok;

    public GameObject canvasError;
    public AudioSource audioSource_error;

    public GameObject canvasFinalNivel;

    public GameObject canvasSuperaErrores;
    public AudioSource audioSource_supera_erroes;

    public GameObject pierdeTodasLasVidas;
    public AudioSource audioSource_pierdeTodasLasVidas;

    private GameObject platosSucios1;
    private GameObject platosSucios2;
    private GameObject platosSucios3;

    private GameObject timer;
    Timer timerScript;

    public int errores;
    public int errores_repeticion;

    public List<GameObject> instantiatedPlatosSucios = new List<GameObject>();
    public List<GameObject> instantiatedPlatosComida = new List<GameObject>();

    //public List<Vector3> savedPositions = new List<Vector3>();
    //public List<Vector3> savedRotations = new List<Vector3>();
    public List<Vector3> savedPositionsPlatosSucios = new List<Vector3>();
    public List<Vector3> savedRotationsPlatosSucios = new List<Vector3>();

    public List<Vector3> savedPositionsPlatosComida = new List<Vector3>();
    public List<Vector3> savedRotationsPlatosComida = new List<Vector3>();


    private void Start()
    {        
        faseActual = 0;
        faseAnterior = -1;
        repeticiones = 0;

        repeticiones_max = 3;
        errores_repeticion = 0;
        //currentLevel = 2;

        posMesa1 = new Vector3[] { new Vector3(-0.086f, 0.801f, 2.003f), new Vector3(0.515f, 0.801f, 2.003f), new Vector3(0.179f, 0.801f, 1.846f) };

        baja_nivel_entrenamiento = false;

        WaveRig = GameObject.Find("Wave Rig");
        logSaver = WaveRig.GetComponent<LogSaver>();
        //logSaver.SetLogEvent("Escena_" + SceneManager.GetActiveScene().name);

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

        platosSucios1 = GameObject.Find("PlatosSucios");
        platosSucios2 = GameObject.Find("PlatosSucios2");
        platosSucios3 = GameObject.Find("PlatosSucios3");

        timer = GameObject.Find("Timer");
        timerScript = timer.GetComponent<Timer>();
        timer.SetActive(false);

        variablesNiveles = WaveRig.GetComponent<VariablesNiveles>();
        errores = 3;
    }

    private void Update()
    {
        variablesNiveles.corazones = errores;
        variablesNiveles.erroresNivel = errores_repeticion;

        currentLevel = variablesNiveles.nivel_cafeteria_T1;
        //currentLevel = 5;
        
        if (currentLevel == 1 | currentLevel == 2 | currentLevel == 3 | currentLevel == 4)
        {
            platosSucios1.SetActive(true);
        }
        else
        {
            platosSucios1.SetActive(false);
        }

        if (currentLevel == 5)
        {
            platosSucios2.SetActive(true);
        }
        else
        {
            platosSucios2.SetActive(false);
        }

        if (currentLevel == 6)
        {
            platosSucios3.SetActive(true);
        }
        else
        {
            platosSucios3.SetActive(false);
        }

        if (currentLevel == 1)
        {
            platos_a_recoger = 2;
            fases_max = 3;
        }
        else if (currentLevel == 2)
        {
            platos_a_recoger = 3;
            fases_max = 2;
        }
        else if (currentLevel == 3)
        {
            platos_a_recoger = 6;
            fases_max = 1;
        }
        else if (currentLevel == 4)
        {
            platos_a_recoger = 6;
            fases_max = 1;
        }
        else if (currentLevel == 5)
        {
            platos_a_recoger = 6;
            fases_max = 1;
        }
        else if (currentLevel == 6)
        {
            platos_a_recoger = 6;
            fases_max = 1;
        }
        //Debug.Log("Current level cafeteria: " + currentLevel);
        grabbedObject = variablesNiveles.grabbedObject;
    }

    public GameObject SelectRandomPlate(GameObject prefab1, GameObject prefab2, GameObject prefab3)
    {
        GameObject[] prefabs = { prefab1, prefab2, prefab3};
        int randomIndex = Random.Range(0, prefabs.Length);

        return prefabs[randomIndex];
    }

    public GameObject SelectRandomPlate_6(GameObject prefab1, GameObject prefab2, GameObject prefab3, GameObject prefab4, GameObject prefab5, GameObject prefab6)
    {
        GameObject[] prefabs = { prefab1, prefab2, prefab3, prefab4, prefab5, prefab6 };
        int randomIndex = Random.Range(0, prefabs.Length);

        return prefabs[randomIndex];
    }

    public GameObject SelectRandomPlate_List(List<GameObject> prefabs)
    {
        int randomIndex = Random.Range(0, prefabs.Count);

        return prefabs[randomIndex];
    }

    public GameObject CreateNewPlateRandomList(GameObject prefab, Vector3 posMesa, Vector3 rotMesa)
    {
        GameObject newPlato;

        //if (prefab.name == "plato_sucio")
        if (prefab.name =="plato_sucio")
        {
            newPlato = Instantiate(prefab, posMesa, Quaternion.Euler(rotMesa));
            BoxCollider boxCollider = newPlato.AddComponent<BoxCollider>();
            boxCollider.size = new Vector3(0.2344088f, 0.2298172f, 0.02085057f);
            boxCollider.center = new Vector3(-0.002295926f, 0f, 0.01229661f);

            newPlato.AddComponent<Rigidbody>();

            //savedPositionsPlatosSucios.Add(new Vector3(posMesa.x, posMesa.y, posMesa.z));
            //savedRotationsPlatosSucios.Add(new Vector3(rotMesa.x, rotMesa.y, rotMesa.z));
        }
        else if (prefab.name.Contains("plato_sucio_fruta") | prefab.name.Contains("plato_sucio_alitas") | prefab.name.Contains("plato_sucio_pizza"))
        {
            //Debug.Log("Entra aqui" + prefab.name);
            float rot_plato_y = rotMesa.z + 180;
            //Debug.Log("Rot mesa: " + rotMesa +" of " + prefab.name);
            newPlato = Instantiate(prefab, posMesa, Quaternion.Euler(0, rot_plato_y, prefab.transform.rotation.z));

            //savedPositionsPlatosSucios.Add(new Vector3(posMesa.x, posMesa.y, posMesa.z));
            //savedRotationsPlatosSucios.Add(new Vector3(0, rot_plato_y, prefab.transform.rotation.z));
        }
        else
        {
            newPlato = Instantiate(prefab, posMesa, Quaternion.Euler(0, rotMesa.y, prefab.transform.rotation.z));

            //savedPositionsPlatosComida.Add(new Vector3(posMesa.x, posMesa.y, posMesa.z));
            //savedRotationsPlatosComida.Add(new Vector3(0, rotMesa.y, prefab.transform.rotation.z));
        }

        return newPlato;
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

    public IEnumerator LaunchError(Vector3 posCabeza, Quaternion rotCabeza)
    {
        canvasError.SetActive(true);

        Vector3 targetPosition = Camera.main.transform.position + Camera.main.transform.forward * 0.7f + new Vector3(0f, 0.34f, 0f);
        Quaternion cameraRotation = Camera.main.transform.rotation;
        Quaternion yRotation = Quaternion.Euler(0, cameraRotation.eulerAngles.y, 0);

        canvasError.transform.SetPositionAndRotation(targetPosition, yRotation);

        yield return new WaitForSeconds(2f);

        canvasError.SetActive(false);
        audioSource_error.Stop();
    }

    public void LaunchSuccess(Vector3 posCabeza, Quaternion rotCabeza)
    {
        //GameObject tick = GameObject.Find("Nivel_ok");
        tick.SetActive(true);

        Vector3 targetPosition = Camera.main.transform.position + Camera.main.transform.forward * 0.5f + new Vector3(0f, 0.34f, 0f);
        Quaternion cameraRotation = Camera.main.transform.rotation;
        Quaternion yRotation = Quaternion.Euler(0, cameraRotation.eulerAngles.y, 0);

        tick.transform.SetPositionAndRotation(targetPosition, yRotation);
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

    public IEnumerator WaitSeconds(GameObject object_deactivate)
    {
        yield return new WaitForSeconds(3f);
        object_deactivate.SetActive(false);
    }
}
