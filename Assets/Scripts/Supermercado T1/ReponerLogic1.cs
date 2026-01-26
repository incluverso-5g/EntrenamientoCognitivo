using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI.Table;
using System.Linq;
using Wave.Native;
using Unity.VisualScripting;
//using UnityEditor.Animations;

public class ReponerLogic1 : MonoBehaviour
{
    private GrabObject grabObj; // = GetComponent<GrabObject>();
    public GameObject products;

    public GameObject randomizeShelfPosition_Shelf2;
    public GameObject randomizeShelfPosition_Shelf3;

    public RandomizeShelfPositions2 randShelfPos2;
    public RandomizeShelfPositions3 randShelfPos3;

    public static ReponerLogic1 instance;

    private Vector3 position_fruit_box = new Vector3(-27.763f, 1.1989f, 0.731f);

    private List<string> fruits = new List<string> { "perry", "orange", "apple", "pepper_red", "onion", "artichoke", "avocado", "tomato" };

    private Vector3 prod_rot = new Vector3(270, 0, 0);

    private Vector3 book_rot = new Vector3(180, 0, 0);

    private Vector3 dog_cat_rot = new Vector3(90, -90, 0);

    private List<Vector3> positions_products = new List<Vector3> { new Vector3(0.101000004f,0f,0.172999993f),
                                                                new Vector3(-0.252000004f,0f,0.172999993f),
                                                                new Vector3(0.828999996f,0f,0.172999993f),
                                                                new Vector3(0.458000004f,0f,0.172999993f),
                                                                new Vector3(-0.259000003f,0f,-0.147f),
                                                                new Vector3(0.0829999968f,0f,-0.147f),
                                                                new Vector3(0.453999996f,0f,-0.147f),
                                                                new Vector3(0.829999983f,0f,-0.147f)};


    private List<bool> position_products_assigned = new List<bool> { false, false, false, false, false, false, false, false };

    class BoxInfo
    {
        public Vector3 initialPosition;
        public Quaternion initialRotation;
        public GameObject boxPrefab;
        public GameObject currentBoxInstance;

        public BoxInfo(Vector3 position, Quaternion rotation, GameObject prefab, GameObject boxInstance)
        {
            initialPosition = position;
            initialRotation = rotation;
            boxPrefab = prefab;
            currentBoxInstance = boxInstance;
        }
    }


    private List<BoxInfo> boxesInfo = new List<BoxInfo>();

    public GameObject[] boxPrefabs;

    private Regex pattern = new Regex(@"box_(.+?)( \((\d+)\)| (\d+))?$");

    public GameObject shelveFruta;
    public GameObject shelveLibros;
    public GameObject shelveComida;

    //public bool use_all = false;

    public List<GameObject> game_shelves = new List<GameObject>();
    private List<GameObject> all_shelves = new List<GameObject>();
    public int objetosAReponer = 15;
    public int objetosRepuestos = 0;

    //public bool new_product;
    public int fallos = 0;
    public bool new_fallo = false;
    public bool new_acierto = false;

    public GameObject tick;
    public AudioSource audioSource_ok;
    
    public GameObject cross;
    public AudioSource audioSource_error;
    


    public GameObject SuperaErrores3;
    public AudioSource audioSource_SuperaErrores3;

    public GameObject canvasInstrucciones;

    public GameObject canvasFinalNivel;
    private AudioSource audioSource_win;

    public GameObject canvasRepeatCount;
    public AudioSource audioSourceRepeatCount;


    public GameObject Supera_Errores;
    public AudioSource audioSource_Supera_Errores;
    
    private bool new_turn = true;

    private float time = 0f;
    public float waiting_time = 3f;
    public GameObject WaveRig;
    private Transform RightController;
    private LogSaver logSaver;
    private VariablesNiveles VN;
    public bool collide_products;
    public bool reset = false;
    private float startupTime = 1f;
    private bool last_separadores;

    public SupermercadoNiveles SupermercadoNiveles;

    public List<List<GameObject>> shelvesByLevel = new List<List<GameObject>>();
    public List<int> productsByLevel = new List<int>();
    public List<int> RandomProducts = new List<int>();
    public List<int> RandomProductsDiff = new List<int>();

    private GameObject oggettoInMano;
    private bool recentAcierto = false;
    private bool recentFallo = false;

    public int objetosRipostiperFrame = 0;
    
    private bool currently_resetting = false;

    void Awake()
    {
        /*if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }*/
    }

    void Start()
    {

        ResetAssignedPositions();

        randShelfPos2 = randomizeShelfPosition_Shelf2.GetComponent<RandomizeShelfPositions2>();
        randShelfPos3 = randomizeShelfPosition_Shelf3.GetComponent<RandomizeShelfPositions3>();

        WaveRig = GameObject.Find("Wave Rig");
        if (WaveRig == null)
        {
            //Debug.Log("BOXCOLL can't find WaveRig");
        }
        grabObj = WaveRig.GetComponent<GrabObject>();


        canvasInstrucciones = GameObject.Find("Instrucciones_Iniciales");
        canvasFinalNivel = GameObject.Find("Nivel_finalizado");
        audioSource_win = canvasFinalNivel.GetComponentInChildren<AudioSource>();

        canvasRepeatCount = GameObject.Find("Nivel_finalizado_Ripe");
        audioSourceRepeatCount = canvasRepeatCount.GetComponentInChildren<AudioSource>();

        SuperaErrores3 = GameObject.Find("3Errores");
        audioSource_SuperaErrores3 = SuperaErrores3.GetComponentInChildren<AudioSource>();

        Supera_Errores = GameObject.Find("Supera_Errores");
        audioSource_Supera_Errores = Supera_Errores.GetComponentInChildren<AudioSource>();

        canvasFinalNivel.SetActive(false);
        canvasRepeatCount.SetActive(false);
        Supera_Errores.SetActive(false);
        SuperaErrores3.SetActive(false);


        for (int i = 0; i < WaveRig.transform.childCount; i++)
        {
            if (WaveRig.transform.GetChild(i).name == "WaveRightController")
            {
                RightController = WaveRig.transform.GetChild(i);
                ////Debug.Log("BOXCOLL right contr " + RightController.name);
            }
        }

        VN = WaveRig.GetComponent<VariablesNiveles>();
        ////Debug.Log("BOXCOLL " + VN.ToString());

        tick = GameObject.Find("Nivel_ok");
        audioSource_ok = tick.GetComponentInChildren<AudioSource>();
        tick.SetActive(false);


        cross = GameObject.Find("Nivel_erroneo");
        audioSource_error = cross.GetComponentInChildren<AudioSource>();
        cross.SetActive(false);

        logSaver = WaveRig.GetComponent<LogSaver>();

        collide_products = !VN.objetosChocan;
        last_separadores = !VN.Separadores;
    }

    private int repeatCount = 0;
    private int maxRepeats = 3;
    private int maxErrors = 3;


    public int totalRepeatCount = 0;


    void Update()
    {

        Transform dynamicChild = GetDynamicChild(RightController, "DynamicObject");
        if (randShelfPos2 != null) //RandomizeShelfPositions2.instance != null)
        {
            if (dynamicChild != null)
            {
                ////Debug.Log($"IIII [ReponerLogic] dynamicChild aggiornato: {dynamicChild.name}");
                //RandomizeShelfPositions2.instance.dynamicChild = dynamicChild;
                randShelfPos2.dynamicChild = dynamicChild;
            }
            else
            {
                ////Debug.Log("IIII [ReponerLogic] Nessun oggetto dinamico trovato. Resetta dynamicChild.");
                //RandomizeShelfPositions2.instance.dynamicChild = null;
                randShelfPos2.dynamicChild = null;
            }
        }
        //else
        //{
            //Debug.LogError("IIII [ReponerLogic] L'istanza di RandomizeShelfPositions2 è null!");
        //}
        if (randShelfPos3!=null) //RandomizeShelfPositions3.instance3 != null)
        {
            if (dynamicChild != null)
            {
                ////Debug.Log($"IIII [ReponerLogic3] dynamicChild aggiornato: {dynamicChild.name}");
                //RandomizeShelfPositions3.instance3.dynamicChild3 = dynamicChild;
                randShelfPos3.dynamicChild3 = dynamicChild;
            }
            else
            {
                ////Debug.Log("IIII [ReponerLogic3] Nessun oggetto dinamico trovato. Resetta dynamicChild.");
                //RandomizeShelfPositions3.instance3.dynamicChild3 = null;
                randShelfPos3.dynamicChild3 = null;
            }
        }
        //else
        //{
            //Debug.LogError("IIII [ReponerLogic3] L'istanza di RandomizeShelfPositions2 è null!");
        //}


        /*if (startupTime > 0)
        {

            startupTime -= Time.deltaTime;
        }
        else
        {

            if (VN.objetosChocan != collide_products)
            {
                // //Debug.Log("Logging " + VN.objetosChocan);
                SetCollisions(VN.objetosChocan);
                collide_products = VN.objetosChocan;
            }
        }*/


        if (VN.Reset)
        {
            TotResetButton();
            VN.Reset = false;
        }
        if (VN.lanzaAcierto)
        {
            LaunchSuccess(WaveRig.transform.position, WaveRig.transform.rotation);
            VN.lanzaAcierto = false;
        }
        if (VN.lanzaError)
        {
            LaunchError(WaveRig.transform.position, WaveRig.transform.rotation);
            VN.erroresNivel += 1;

            VN.lanzaError = false;
        }
        if (VN.reiniciaNivel == true)
        {
            RestartScene();
            VN.reiniciaNivel = false;
        }
        if (!reset)
        {
            time += Time.deltaTime;
            if (new_acierto)
            {
                ////Debug.Log("BOXCOLL good one");
                LaunchSuccess(WaveRig.transform.position, WaveRig.transform.rotation);
                new_acierto = false;
                recentAcierto = true;
                StartCoroutine(RecentAciertoSetFalse());
                objetosRepuestos += 1;
                objetosRipostiperFrame++;

                if (objetosRipostiperFrame >= NumberProducts)
                {
                    ResetAssignedPositions();
                    new_turn = true;
                    objetosRipostiperFrame = 0;
                    OriginalPositions();
                    //StartCoroutine(NewObjectsAsync());
                    time = 0;
                }

                if (objetosRepuestos >= objetosAReponer)
                {
                    //CONTATORE DI LIVELLI

                    if (repeatCount < maxRepeats - 1)
                    {
                        if (!currently_resetting)
                        {
                            ShowCanvasRepeatCount();
                            repeatCount++;
                            totalRepeatCount++;
                            StartCoroutine(ResetLevelStateAsync(3f));
                            logSaver.SetLogEvent("Nivel_finalizado_" + SceneManager.GetActiveScene().name + "_" + currentLevel + "_Repeticion_" + totalRepeatCount);
                        }
                        //if (!new_turn)
                        //{
                        //    OriginalPositions();
                        //}

                        //ResetLevelState();
                        
                        //Debug.Log("PROVA_" + currentLevel);
                        //Debug.Log("RIPETIZIONE Ripetizione del livello: " + totalRepeatCount + " RepeatCount: " + repeatCount);
                    }
                    else
                    {
                        if (!currently_resetting)
                        {
                            ShowCanvasFinal();
                            totalRepeatCount++;
                            new_turn = false;
                            //Debug.Log("RIPETIZIONE Livello completato dopo " + totalRepeatCount + " ripetizioni.");
                            FinalLevel();
                            logSaver.SetLogEvent("Nivel_finalizado_" + SceneManager.GetActiveScene().name + "_" + currentLevel + "_Repeticion_" + totalRepeatCount);
                            logSaver.SetLogEvent("Fin_Niveles");
                        }
                    }

                }
            }
            else if (!recentFallo & !recentAcierto)
            {
                if (new_fallo)
                {
                    VibrateRightController(500, 2);
                    LaunchError(WaveRig.transform.position, WaveRig.transform.rotation);
                    new_fallo = false;
                    recentFallo = true;
                    StartCoroutine(RecentFalloSetFalse());

                    VN.erroresNivel += 1;
                    //Debug.Log("RIPETIZIONE  ERRORE numero: " + VN.erroresNivel);


                    if (VN.erroresNivel >= maxErrors)
                    {
                        //Debug.Log("RIPETIZIONE ERRORE Raggiunti 3 errori. ");

                        VN.erroresNivel = 0;
                        //CorazonesVar--;
                        VN.corazones--;
                        logSaver.SetLogEvent("3Errores_Nivel_" + totalRepeatCount + "_ReiniciaNivel");

                        //Debug.Log(" RIPETIZIONE ERRORE Corazones rimanenti: " + VN.corazones);


                        if (VN.corazones <= 0)
                        {
                            if (VN.grabbedObject != null)
                            {
                                VN.grabbedObject.SetActive(false);
                                VN.grabbedObject = null;

                                //Debug.Log("Oggetto in mano disattivato per il reset del livello.");
                            }

                            ShowSuperaErrores();
                            FinalLevel();

                            logSaver.SetLogEvent("Pierde3Vidas_Nivel_" + totalRepeatCount + "_VuelveNivelAnterior");

                            //Debug.Log("ERRORE GAME OVER: Nessun corazones rimanente.");

                        }
                        else
                        {
                            //Debug.Log("Restore original Positions 3 errores.");
                            if (!currently_resetting)
                            {
                                Show3Errores();
                                totalRepeatCount++;
                                resetCorot = StartCoroutine(ResetLevelStateAsync(3f));
                            }    
                            //ResetLevelState();
                        }
                    }
                }
            }
            else
            {
                new_fallo = false;
            }

            if (new_turn & time > waiting_time)
            {
                //Debug.Log("!!!! Entrato nell'if di new_turn");
                new_turn = false;
                var (shelveNum, childNum, productName, prodottiRimossiShelve2, prodottiRimossiShelve3, Prodottifinali) = ChooseAndEmptyBox();
                //Debug.Log("!!!!" + shelveNum.ToString() + " " + childNum.ToString() + " " + productName);

            }
        }

        if(VN.Separadores != last_separadores)
        {
            foreach (GameObject shelve in game_shelves)
            {
                for (int i = 0; i < shelve.transform.childCount; i++)
                {
                    Transform box = shelve.transform.GetChild(i);
                    if (box.name.StartsWith("BOX"))
                    {
                        foreach (Transform child in box)
                        {
                            if (child.name.StartsWith("Cube"))
                            {
                                child.gameObject.SetActive(VN.Separadores);
                                // logSaver.SetLogEvent("True separadores estanterías");
                            }
                        }
                    }
                }
            }
            last_separadores = VN.Separadores;
        }
    }


    private IEnumerator NewObjectsAsync()
    {
        yield return new WaitForSecondsRealtime(1f);
        ResetAssignedPositions();
        yield return new WaitForSecondsRealtime(0.5f);
        new_turn = true;
        objetosRipostiperFrame = 0;
        OriginalPositions();
    }

    private IEnumerator RecentAciertoSetFalse()
    {
        yield return new WaitForSecondsRealtime(1f);
        recentAcierto = false;
    }

    private IEnumerator RecentFalloSetFalse()
    {
        yield return new WaitForSecondsRealtime(1f);
        recentFallo = false;
    }

    /*private IEnumerator RecentFallo(bool evento)
    {
        yield return new WaitForSecondsRealtime(2f);
        evento = false;
    }*/


    Transform GetDynamicChild(Transform parent, string tag)
    {
        foreach (Transform child in parent)
        {
            if (child.CompareTag(tag))
            {
                return child;
            }
        }
        return null;
    }


    public void FinalLevel()
    {

        GameObject table = GameObject.Find("table_2");
        if (table != null)
        {
            foreach (Transform child in table.transform)
            {
                child.gameObject.SetActive(false);
                ////Debug.Log($"Oggetto {child.name} disattivato in {table}.");
            }
        }

        if (products != null)
        {
            foreach (Transform child in products.transform)
            {
                child.gameObject.SetActive(false);
                ////Debug.Log($"Oggetto {child.name}  disattivato.");
            }
        }
        foreach (Transform randomObject in selectedRandomObjects)
        {
            randomObject.gameObject.SetActive(false);
            ////Debug.Log($"Oggetto '{randomObject.gameObject.name}' disattivato per la nuova ripetizione.");
        }

        foreach (Transform randomObjectDiff in selectedRandomObjectsDiff)
        {
            randomObjectDiff.gameObject.SetActive(false);
            ////Debug.Log($"Oggetto '{randomObjectDiff.gameObject.name}' disattivato per la nuova ripetizione.");
        }


        OriginalPositions();

    }



    private void ShowCanvasFinal()
    {
        StartCoroutine(ShowAndHideCanvasAfterSeconds(canvasFinalNivel, audioSource_win, 2f, 99f));
        /*
        //Debug.Log("Entrato in ShowCanvasFinal");
        Vector3 targetPosition = Camera.main.transform.position + Camera.main.transform.forward * 0.7f + new Vector3(0f, 0.34f, 0f);
        Quaternion cameraRotation = Camera.main.transform.rotation;
        Quaternion yRotation = Quaternion.Euler(0, cameraRotation.eulerAngles.y, 0);

        canvasFinalNivel.transform.SetPositionAndRotation(targetPosition, yRotation);
        canvasFinalNivel.SetActive(true);
        audioSource_win.Play();
        //Debug.Log("Canvas mostrato correttamente.");
        */
    }

    private void ShowCanvasRepeatCount()
    {
        StartCoroutine(ShowAndHideCanvasAfterSeconds(canvasRepeatCount, audioSourceRepeatCount, 2f, 3f));
        ////Debug.Log("Entrato in ShowCanvasRepeatCount");
        /*
        Vector3 targetPosition = Camera.main.transform.position + Camera.main.transform.forward * 0.7f + new Vector3(0f, 0.34f, 0f);
        Quaternion cameraRotation = Camera.main.transform.rotation;
        Quaternion yRotation = Quaternion.Euler(0, cameraRotation.eulerAngles.y, 0);

        canvasRepeatCount.transform.SetPositionAndRotation(targetPosition, yRotation);
        canvasRepeatCount.SetActive(true);


        if (audioSourceRepeatCount != null)
        {
            audioSourceRepeatCount.Play();
            ////Debug.Log("Audio di repeatCount riprodotto.");
        }
        //else
        //{
        //    //Debug.LogWarning("Audio Source per repeatCount non trovato.");
        //}

        StartCoroutine(HideCanvasAfterSeconds(canvasRepeatCount, 3f));
        */
        ////Debug.Log("Canvas repeatCount mostrato correttamente.");
    }


    private IEnumerator HideCanvasAfterSeconds(GameObject canvas, float delay)
    {
        yield return new WaitForSeconds(delay);
        canvas.SetActive(false);
        ////Debug.Log($"{canvas.name} nascosto dopo {delay} secondi.");
    }

    private IEnumerator ShowAndHideCanvasAfterSeconds(GameObject canvas, AudioSource audio, float delayShow, float delayHide)
    {
        yield return new WaitForSecondsRealtime(delayShow);
        Vector3 targetPosition = Camera.main.transform.position + Camera.main.transform.forward * 0.7f + new Vector3(0f, 0.34f, 0f);
        Quaternion cameraRotation = Camera.main.transform.rotation;
        Quaternion yRotation = Quaternion.Euler(0, cameraRotation.eulerAngles.y, 0);
        canvas.transform.SetPositionAndRotation(targetPosition, yRotation);
        canvas.SetActive(true);
        audio.Play();
        yield return new WaitForSecondsRealtime(delayHide);
        canvas.SetActive(false);
        ////Debug.Log($"{canvas.name} nascosto dopo {delay} secondi.");
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


    void Show3Errores()
    {
        StartCoroutine(ShowSupera3ErroresTemporarily());
    }


    void ShowSuperaErrores()
    {
        StartCoroutine(ShowSuperaErroresAfter(2f));
        /*Vector3 targetPosition = Camera.main.transform.position + Camera.main.transform.forward * 0.7f + new Vector3(0f, 0.34f, 0f);
        Quaternion cameraRotation = Camera.main.transform.rotation;
        Quaternion yRotation = Quaternion.Euler(0, cameraRotation.eulerAngles.y, 0);

        Supera_Errores.transform.SetPositionAndRotation(targetPosition, yRotation);
        Supera_Errores.SetActive(true);
        audioSource_Supera_Errores.Play();*/
    }


    IEnumerator ShowSuperaErroresAfter(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        Vector3 targetPosition = Camera.main.transform.position + Camera.main.transform.forward * 0.7f + new Vector3(0f, 0.34f, 0f);
        Quaternion cameraRotation = Camera.main.transform.rotation;
        Quaternion yRotation = Quaternion.Euler(0, cameraRotation.eulerAngles.y, 0);

        Supera_Errores.transform.SetPositionAndRotation(targetPosition, yRotation);
        Supera_Errores.SetActive(true);
        audioSource_Supera_Errores.Play();
    }


    IEnumerator ShowSupera3ErroresTemporarily()
    {
        yield return new WaitForSeconds(2f);
        Vector3 targetPosition = Camera.main.transform.position + Camera.main.transform.forward * 0.7f + new Vector3(0f, 0.34f, 0f);
        Quaternion cameraRotation = Camera.main.transform.rotation;
        Quaternion yRotation = Quaternion.Euler(0, cameraRotation.eulerAngles.y, 0);

        SuperaErrores3.transform.SetPositionAndRotation(targetPosition, yRotation);
        SuperaErrores3.SetActive(true);
        audioSource_SuperaErrores3.Play();

        yield return new WaitForSeconds(3f);
        SuperaErrores3.SetActive(false);
    }


    private IEnumerator ResetLevelStateAsync(float delay)
    {
        if (currently_resetting)
        {
            yield break;
        }

        currently_resetting = true;
        yield return new WaitForSecondsRealtime(delay);
        if (VN.grabbedObject != null)
        {
            VN.grabbedObject.SetActive(false);
            VN.grabbedObject = null;
            ////Debug.Log("Oggetto in mano disattivato per il reset del livello.");
        }

        Supera_Errores.SetActive(false);
        canvasFinalNivel.SetActive(false);

        objetosRepuestos = 0;
        objetosRipostiperFrame = 0;
        new_turn = true;
        new_acierto = false;
        new_fallo = false;
        time = 0;
        VN.erroresNivel = 0;

        ResetAssignedPositions();

        foreach (Transform randomObject in selectedRandomObjects)
        {
            randomObject.gameObject.SetActive(false);
            ////Debug.Log($"Oggetto '{randomObject.gameObject.name}' disattivato per la nuova ripetizione.");
        }

        foreach (Transform randomObjectDiff in selectedRandomObjectsDiff)
        {
            randomObjectDiff.gameObject.SetActive(false);
            ////Debug.Log($"Oggetto '{randomObjectDiff.gameObject.name}' disattivato per la nuova ripetizione.");
        }

        OriginalPositions();
        currently_resetting = false;
    }


    public void ResetLevelState()
    {
        //Debug.Log("Entrato in ResetLevelState");

        if (VN.grabbedObject != null)
        {
            VN.grabbedObject.SetActive(false);
            VN.grabbedObject = null;
            ////Debug.Log("Oggetto in mano disattivato per il reset del livello.");
        }

        Supera_Errores.SetActive(false);
        canvasFinalNivel.SetActive(false);

        objetosRepuestos = 0;
        objetosRipostiperFrame = 0;
        new_turn = true;
        time = 0;
        VN.erroresNivel = 0;

        ResetAssignedPositions();

        foreach (Transform randomObject in selectedRandomObjects)
        {
            randomObject.gameObject.SetActive(false);
            ////Debug.Log($"Oggetto '{randomObject.gameObject.name}' disattivato per la nuova ripetizione.");
        }

        foreach (Transform randomObjectDiff in selectedRandomObjectsDiff)
        {
            randomObjectDiff.gameObject.SetActive(false);
            ////Debug.Log($"Oggetto '{randomObjectDiff.gameObject.name}' disattivato per la nuova ripetizione.");
        }

        OriginalPositions();
    }

    public void OriginalPositions()
    {
        if (randShelfPos3!= null) //RandomizeShelfPositions3.instance3 != null)
        {
            //Debug.Log(" Restore original reset level state");
            //RandomizeShelfPositions3.instance3.RestoreOriginalPositions3();
            randShelfPos3.RestoreOriginalPositions3();
        }

        if (randShelfPos2!=null) //RandomizeShelfPositions2.instance != null)
        {
            //RandomizeShelfPositions2.instance.RestoreOriginalPositions();
            randShelfPos2.RestoreOriginalPositions();
        }

    }


    private int currentLevel;
    public int NumberProducts;
    public int NumberProductsRandom;
    public int NumberProductsRandomDiff;
    public int GetShelvesByLevel(int level)
    {
        //Debug.LogWarning("!!!! Entrto in GetShelvesByLevel");


        currentLevel = level;
        // SHELVES

        shelvesByLevel.Add(new List<GameObject> { shelveComida, shelveLibros }); // Livello 1
        shelvesByLevel.Add(new List<GameObject> { shelveComida, shelveLibros }); // Livello 2
        shelvesByLevel.Add(new List<GameObject> { shelveComida, shelveLibros }); // Livello 3
        shelvesByLevel.Add(new List<GameObject> { shelveComida, shelveLibros }); // Livello 4
        shelvesByLevel.Add(new List<GameObject> { shelveComida, shelveLibros, shelveFruta }); // Livello 5
        shelvesByLevel.Add(new List<GameObject> { shelveComida, shelveLibros, shelveFruta }); // Livello 6


        //Debug.Log("!!!! shelvesByLevel popolato con Count = " + shelvesByLevel.Count);

        // Non e selezionato nessun livello

        if (level < 1 || level > shelvesByLevel.Count)
        {
            //Debug.LogWarning("Livello non valido: " + level);
            game_shelves = new List<GameObject>();
            return -1;
        }

        // Aggiorna shelves con livello selezionato

        game_shelves = shelvesByLevel[level - 1];
        //Debug.LogWarning("Livello Selezionato con: " + game_shelves);

        //foreach (GameObject shelf in game_shelves)
        //{
         ////Debug.LogWarning("Quale scaffale: " + shelf.name);
        //}

        // PRODOTTI DA RIMETTERE IN ORDINE

        productsByLevel = new List<int> { 1, 1, 2, 2, 3, 2 };
        NumberProducts = productsByLevel[level - 1];
        //Debug.Log("!!!! Numero di prodotti da scegliere per il livello " + level + ": " + NumberProducts);

        // PRODOTTI RANDOM

        RandomProducts = new List<int> { 0, 0, 1, 2, 3, 4 };
        NumberProductsRandom = RandomProducts[level - 1];

        // PRODOTTI RANDOM NON PRESENTI NEL SUPERMERCATO

        RandomProductsDiff = new List<int> { 0, 1, 1, 1, 1, 2 };
        NumberProductsRandomDiff = RandomProductsDiff[level - 1];

        return level;
    }



    int shelveNumber = 0;
    int small_boxNumber = 0;
    string productName = "";
    public List<List<string>> prodottiRimossiShelve2 = new List<List<string>>();
    public List<List<string>> prodottiRimossiShelve3 = new List<List<string>>();
    public List<GameObject> Prodottifinali = new List<GameObject>();


    private List<GameObject> TableObjects = new List<GameObject>();
    public (int, int, string, List<List<string>>, List<List<string>>, List<GameObject>) ChooseAndEmptyBox()
    {
        //Debug.Log("!!!! Valore di objetosRepuestos: " + objetosRepuestos);

        prodottiRimossiShelve2.Clear();
        prodottiRimossiShelve3.Clear();
        Prodottifinali.Clear();

        RandomObjectDiff();
        RandomObject();
        for (int i = 0; i < NumberProducts; i++)
        {

            // SHELVE

            shelveNumber = UnityEngine.Random.Range(0, game_shelves.Count);  //  ex-> shelveNumber=1
            GameObject shelve = game_shelves[shelveNumber];
            //////Debug.Log("!!!! shelve name: " + shelve.name);


            //BOX

            int BOXCount = shelve.transform.childCount; //sceglie un figlio dello shelve. ex: shelve1-> childCount=32

            Transform randomBOX = null;


            //controlla che il figlio inizi con BOX (!randomBOX.name.StartsWith("BOX")) e che sia attivo (!randomBOX.gameObject.activeSelf)

            while (randomBOX == null || !randomBOX.name.StartsWith("BOX") || !randomBOX.gameObject.activeSelf)
            {
                int BOXNumber = UnityEngine.Random.Range(0, BOXCount); // scheglie un figlio da (0,32)--> da 0 a 31   ex-->15->BOX_rvine01

                randomBOX = shelve.transform.GetChild(BOXNumber);  // Si prende una BOX

            }

            ////Debug.Log("!!!! selected BOX : " + randomBOX.name);

            //BOX--->front

            Transform front = randomBOX.Find("front");

            //front --> box_

            int small_boxCount = front.transform.childCount;  // Numero di box piccoli in front
            small_boxNumber = UnityEngine.Random.Range(0, small_boxCount);  // Sceglie un box piccolo casuale
            Transform small_box = front.GetChild(small_boxNumber);  // Ottiene il box piccolo
            string box_name = small_box.gameObject.name;   // Ottengo il nome del box --> box_rwine01

            ////Debug.Log("!!!! box name: " + box_name);


            // box_ --> products
            Transform product = small_box.transform.Find("products");


            if (product != null)
            {
                ////Debug.Log("!!!! Prodotto trovato: " + product.name);

                if (shelve.name == "ray_store_game2")
                {
                    List<string> gerarchiaOggetto = new List<string>
                            {
                                randomBOX.name.Replace("(Clone)", ""),
                                front.name,
                                small_box.name,
                                product.name
                            };
                    ////Debug.Log("!!!! Aggiungo la gerarchia a prodottiRimossiShelve2: " + string.Join(" -> ", gerarchiaOggetto));
                    prodottiRimossiShelve2.Add(gerarchiaOggetto);
                }
                else if (shelve.name == "ray_store_game3")
                {

                    List<string> gerarchiaOggetto3 = new List<string>
                            {
                                randomBOX.name.Replace("(Clone)", ""),
                                front.name,
                                small_box.name,
                                product.name
                            };


                    ////Debug.Log("!!!! Aggiungo la gerarchia a prodottiRimossiShelve3: " + string.Join(" -> ", gerarchiaOggetto3));
                    prodottiRimossiShelve3.Add(gerarchiaOggetto3);
                }

                product.gameObject.SetActive(false);
            }
            //else
            //{
                //Debug.LogError("!!!! Prodotto non trovato in " + small_box.name);
            //}



            if (randomizeShelfPosition_Shelf2 != null)
            {
                //Debug.Log("randomizeShelfPosition_Shelf2: " + randomizeShelfPosition_Shelf2.name);

                RandomizeShelfPositions2 randomizeScript = randomizeShelfPosition_Shelf2.GetComponent<RandomizeShelfPositions2>();

                if (randomizeScript != null)
                {
                    ////Debug.Log("!!! RandomizeShelfPositions2 trovato, passando lista prodottiRimossiShelve2...");
                    randomizeScript.PassaLaLista(prodottiRimossiShelve2);
                }
            }
            else
            {
                Debug.LogError("!!!! randomizeShelfPosition_Shelf2 è null");
            }


            if (randomizeShelfPosition_Shelf3 != null)
            {
                //Debug.Log("randomizeShelfPosition_Shelf3: " + randomizeShelfPosition_Shelf3.name);

                RandomizeShelfPositions3 randomizeScript3 = randomizeShelfPosition_Shelf3.GetComponent<RandomizeShelfPositions3>();

                if (randomizeScript3 != null)
                {
                    //Debug.Log("!!! RandomizeShelfPositions3 trovato, passando lista prodottiRimossiShelve3...");
                    randomizeScript3.PassaLaLista3(prodottiRimossiShelve3);
                }        
            }
            else
            {
                Debug.LogError("!!!! randomizeShelfPosition_Shelf3 è null");
            }




            // Table Products

            string productName = "";
            Match match = pattern.Match(box_name);
            productName = match.Groups[1].Value;
            //Debug.Log("!!!! reponer Selected product: {productName}");


            Transform productToPlace = products.transform.Find("Product_" + productName);
            if (productToPlace == null)
            {
                Debug.LogError($"!!!! Il prodotto '{productName}' non esiste nella gerarchia.");
            }

            else
            {
                // Verifica se è già attivo, e in tal caso crea una copia
                if (productToPlace.gameObject.activeSelf)
                {
                    //Debug.Log("!!!! Il prodotto è già attivo. Creando una copia.");
                    productToPlace = Instantiate(productToPlace, products.transform); // Crea una copia del prodotto
                    productToPlace.name = "!!!! Product_" + productName + "(Clone)"; // Rinomina la copia
                }

                var (selectedPosition, position_index) = GetRandomAvailablePosition(productName);

                Vector3 product_rot = prod_rot;
                if (productName.Contains("dog") || productName.Contains("cat"))
                {
                    product_rot = dog_cat_rot;
                }
                else if (productName.Contains("book") || productName.Contains("pencils"))
                {
                    product_rot = book_rot;
                    if (selectedPosition.x == -27.80f)
                    {
                        selectedPosition.z = 1.347f;
                    }
                }

                productToPlace.transform.SetLocalPositionAndRotation(selectedPosition, Quaternion.Euler(product_rot.x, product_rot.y, product_rot.z));
                productToPlace.gameObject.SetActive(true);

                Prodottifinali.Add(productToPlace.gameObject);
            }
        }
        return (shelveNumber, small_boxNumber, productName, prodottiRimossiShelve2, prodottiRimossiShelve3, Prodottifinali);

    }

    public GameObject ProductosFalsos;
    public GameObject ProductosFalsosDiff;
    public List<Transform> selectedRandomObjects = new List<Transform>();
    public List<Transform> selectedRandomObjectsDiff = new List<Transform>();
    public List<Transform> RandomObject()
    {
        //Debug.Log("RANDOM Entrato in RandomObject");


        if (NumberProductsRandom == 0)
        {
            //Debug.LogWarning("RANDOM Nessun prodotto da selezionare, NumberProductsRandom è 0.");
            return selectedRandomObjects; // Restituisce una lista vuota senza fare altre operazioni
        }

        // PRODOTTI UGUALI

        List<Transform> availableChildren = new List<Transform>();
        selectedRandomObjects.Clear();

        //Debug.Log($" RANDOM Numero di figli in 'ProductosFalsos': {ProductosFalsos.transform.childCount}");


        foreach (Transform child in ProductosFalsos.transform)
        {
            if (!child.gameObject.activeSelf)
            {
                availableChildren.Add(child);
                ////Debug.Log($"RANDOM Oggetto disponibile trovato: {child.gameObject.name}");
            }

        }

        for (int i = 0; i < NumberProductsRandom && availableChildren.Count > 0; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, availableChildren.Count);
            Transform randomChild = availableChildren[randomIndex];


            string randomProductName = randomChild.gameObject.name;

            ////Debug.Log($"RANDOM Selezionato oggetto: {randomProductName}");
            var (selectedPosition, position_index) = GetRandomAvailablePosition(randomProductName);

            Quaternion originalRotation = randomChild.localRotation;

            ////Debug.Log($"RANDOM Posizione assegnata per '{randomProductName}': {selectedPosition}");


            randomChild.SetLocalPositionAndRotation(selectedPosition, originalRotation);
            randomChild.gameObject.SetActive(true);

            selectedRandomObjects.Add(randomChild);

            ////Debug.Log($"RANDOM Oggetto '{randomProductName}' posizionato e attivato");

            availableChildren.RemoveAt(randomIndex);
        }

        return selectedRandomObjects;

    }

    public List<Transform> RandomObjectDiff()
    {
        //Debug.Log("RANDOM_DIFF Entrato in RandomObjectDiff");

        if (NumberProductsRandomDiff == 0)
        {
            //Debug.LogWarning("RANDOM_DIFF Nessun prodotto da selezionare, NumberProductsRandomDiff è 0.");
            return selectedRandomObjectsDiff; // Restituisce una lista vuota senza fare altre operazioni
        }

        // PRODOTTI NON PRESENTI NEL SUPERMERCATO
        List<Transform> availableChildrenDiff = new List<Transform>();
        selectedRandomObjectsDiff.Clear();

        foreach (Transform child in ProductosFalsosDiff.transform)
        {
            if (!child.gameObject.activeSelf)
            {
                availableChildrenDiff.Add(child);
                ////Debug.Log($"RANDOM_DIFF Oggetto disponibile trovato: {child.gameObject.name}");
            }
        }

        for (int i = 0; i < NumberProductsRandomDiff && availableChildrenDiff.Count > 0; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, availableChildrenDiff.Count);
            Transform randomChild = availableChildrenDiff[randomIndex];

            var (selectedPosition, position_index) = GetRandomAvailablePosition(randomChild.name);

            randomChild.SetLocalPositionAndRotation(selectedPosition, randomChild.localRotation);
            randomChild.gameObject.SetActive(true);

            selectedRandomObjectsDiff.Add(randomChild);

            availableChildrenDiff.RemoveAt(randomIndex);
        }

        return selectedRandomObjectsDiff;
    }


    private List<GameObject> activeObjects = new List<GameObject>();
    public void CollectAndReset()
    {
        //Debug.Log($"Entrato in Collect and Reset");
        ResetAssignedPositionsRefresh();
        activeObjects.Clear();

        List<string> productNames = new List<string>();

        // Products
        foreach (Transform child in products.transform)
        {
            if (child.gameObject.activeSelf)
            {
                ////Debug.Log("Collect Tutti i figli attivi di products CollectAndReset : " + child.gameObject.name);
                activeObjects.Add(child.gameObject);
            }
        }

        //  RandomObject
        foreach (var obj in selectedRandomObjects)
        {
            if (obj != null && obj.gameObject.activeSelf)
            {
                activeObjects.Add(obj.gameObject);
            }
        }

        //  RandomObjectDiff
        foreach (var obj in selectedRandomObjectsDiff)
        {
            if (obj != null && obj.gameObject.activeSelf)
            {
                activeObjects.Add(obj.gameObject);
            }
        }

        //Debug.Log($"Collect: Totale prodotti attivi raccolti: {activeObjects.Count}");


        // Riposiziono gli oggetti raccolti
        foreach (GameObject obj in activeObjects)
        {
            try
            {
                obj.SetActive(false); // Disattivo l'oggetto temporaneamente
                var (randomPosition, positionIndex) = GetRandomAvailablePosition(obj.name);

                Vector3 product_rot = prod_rot;
                if (obj.transform.name.Contains("dog") || obj.transform.name.Contains("cat"))
                {
                    product_rot = dog_cat_rot;
                }
                else if (obj.transform.name.Contains("book") || obj.transform.name.Contains("pencils"))
                {
                    product_rot = book_rot;
                }

                obj.transform.SetLocalPositionAndRotation(randomPosition, Quaternion.Euler(product_rot));
                /// Riposiziona e riattiva l'oggetto
                obj.transform.localPosition = randomPosition;
                obj.SetActive(true);

                ////Debug.Log($"Collect: Oggetto {obj.name} riposizionato a {randomPosition} (index: {positionIndex})");
            }
            catch (InvalidOperationException ex)
            {
                Debug.LogError($"Collect Errore nel riposizionare {obj.name}: {ex.Message}");
            }
        }

        //Debug.Log("Collect Riposizionamento completato.");
    }



    public static Transform GetChildWithTag(Transform transform, string tag)
    {
        if (transform.childCount == 0)
        {
            return null;
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).CompareTag(tag))
            {
                return transform.GetChild(i);
            }
            else
            {
                Transform childWithTag = GetChildWithTag(transform.GetChild(i), tag);
                if (childWithTag != null)
                {
                    return childWithTag;
                }
            }
        }

        return null;
    }

    (Vector3, int) GetRandomAvailablePosition(string name)
    {

        //Debug.Log("!!!! Posizioni disponibili prima dell'assegnazione: " + string.Join(", ", position_products_assigned.Select(b => b.ToString())));
        List<int> availableIndices = new List<int>();
        for (int i = 0; i < position_products_assigned.Count; i++)
        {
            if (!position_products_assigned[i])
            {
                availableIndices.Add(i);
            }
        }
        if (availableIndices.Count == 0)
        {
            //Debug.LogError("!!!! No available positions.");
            throw new InvalidOperationException("No available positions.");
        }
        int randomIndex = availableIndices[UnityEngine.Random.Range(0, availableIndices.Count)];


        position_products_assigned[randomIndex] = true;
        //Debug.Log($"!!!! product position: {positions_products[randomIndex]} at index: {randomIndex}");

        //Debug.Log("!!!! Posizioni disponibili dopo l'assegnazione: " + string.Join(", ", position_products_assigned.Select(b => b.ToString())));


        return (positions_products[randomIndex], randomIndex);
    }



    public void ResetBoxesToInitialState()
    {
        foreach (BoxInfo info in boxesInfo)
        {
            // Destroy the current instance of the box
            if (info.currentBoxInstance != null)
            {
                Destroy(info.currentBoxInstance);
            }

            // Instantiate a new box from the prefab at the initial position and rotation
            GameObject newBox = Instantiate(info.boxPrefab, info.initialPosition, info.initialRotation);

            // Update the reference to the current box instance
            info.currentBoxInstance = newBox;
        }
    }


    private IEnumerator RestoreOriginalPositionsFrutaAsync()
    {
        if (shelveFruta != null)
        {
            //Debug.LogError("shelveFruta Nessuno scaffale assegnato!");
            foreach (Transform child in shelveFruta.transform)
            {
                // Verifica se il nome inizia con BOX_
                if (child.name.StartsWith("BOX_"))
                {
                    // Trova il "front"
                    Transform front = child.Find("front");
                    if (front != null)
                    {
                        foreach (Transform frontChild in front)
                        {
                            // Se il figlio di front NON è attivo...
                            if (!frontChild.gameObject.activeSelf)
                            {

                                frontChild.gameObject.SetActive(true);
                                ////Debug.Log($"shelveFruta Attivato: {frontChild.name}");
                            }
                            else
                            {

                                ////Debug.Log($"shelveFruta Già attivo: {frontChild.name}");
                                Transform products = frontChild.Find("products");
                                if (products != null)
                                {
                                    // Se "products" è disattivo, lo attiviamo
                                    if (!products.gameObject.activeSelf)
                                    {
                                        products.gameObject.SetActive(true);
                                        ////Debug.Log($"Attivato products in: {frontChild.name}");
                                    }
                                    else
                                    {
                                        // Se "products" è già attivo, controllo i suoi figli
                                        foreach (Transform prodChild in products)
                                        {
                                            if (!prodChild.gameObject.activeSelf)
                                            {
                                                prodChild.gameObject.SetActive(true);
                                                ////Debug.Log($"Attivato figlio di products: {prodChild.name}");
                                            }
                                            //else
                                            //{
                                            //    //Debug.Log($"Figlio di products già attivo: {prodChild.name}");
                                            //}
                                        }
                                    }
                                }
                                //else
                                //{
                                //    // frontChild attivo ma non ha un "products"
                                //    //Debug.Log($"Nessun 'products' trovato in: {frontChild.name}");
                                //}
                            }
                            yield return null;
                        }
                    }
                }
                yield return null;
            }
        }
    }

    public void RestoreOriginalPositionsFruta()
    {
        //Debug.Log("Entrato in  RestoreOriginalPositionsFruta ");


        if (shelveFruta == null)
        {
            //Debug.LogError("shelveFruta Nessuno scaffale assegnato!");
            return;
        }


        foreach (Transform child in shelveFruta.transform)
        {
            // Verifica se il nome inizia con BOX_
            if (child.name.StartsWith("BOX_"))
            {
                // Trova il "front"
                Transform front = child.Find("front");
                if (front != null)
                {
                    foreach (Transform frontChild in front)
                    {
                        // Se il figlio di front NON è attivo...
                        if (!frontChild.gameObject.activeSelf)
                        {

                            frontChild.gameObject.SetActive(true);
                            ////Debug.Log($"shelveFruta Attivato: {frontChild.name}");
                        }
                        else
                        {

                            ////Debug.Log($"shelveFruta Già attivo: {frontChild.name}");
                            Transform products = frontChild.Find("products");
                            if (products != null)
                            {
                                // Se "products" è disattivo, lo attiviamo
                                if (!products.gameObject.activeSelf)
                                {
                                    products.gameObject.SetActive(true);
                                    ////Debug.Log($"Attivato products in: {frontChild.name}");
                                }
                                else
                                {
                                    // Se "products" è già attivo, controllo i suoi figli
                                    foreach (Transform prodChild in products)
                                    {
                                        if (!prodChild.gameObject.activeSelf)
                                        {
                                            prodChild.gameObject.SetActive(true);
                                            ////Debug.Log($"Attivato figlio di products: {prodChild.name}");
                                        }
                                        //else
                                        //{
                                        //    //Debug.Log($"Figlio di products già attivo: {prodChild.name}");
                                        //}
                                    }
                                }
                            }
                            //else
                            //{
                            //    // frontChild attivo ma non ha un "products"
                            //    //Debug.Log($"Nessun 'products' trovato in: {frontChild.name}");
                            //}
                        }
                    }
                }
            }
        }

    }


    void ResetAssignedPositionsRefresh()
    {

        for (int i = 0; i < position_products_assigned.Count; i++)
        {
            position_products_assigned[i] = false;
        }

    }


    void ResetAssignedPositions()
    {
        // position_products_assigned = new List<bool> { false, false, false, false, false, false};
        for (int i = 0; i < position_products_assigned.Count; i++)
        {
            position_products_assigned[i] = false;
        }
        foreach (Transform child in products.transform)
        {
            child.gameObject.SetActive(false);
        }
        foreach (Transform child in selectedRandomObjectsDiff)
        {
            child.gameObject.SetActive(false);
        }
        foreach (Transform child in selectedRandomObjects)
        {
            child.gameObject.SetActive(false);
        }

        StartCoroutine(RestoreOriginalPositionsFrutaAsync());
        //RestoreOriginalPositionsFruta();
    }


    private bool isResetting = false;
    private Coroutine resetCorot;

    public void TotResetButton()
    {

        if (isResetting)
        {
            //Debug.LogWarning("Il reset è già in corso!");
            return;
        }

        try
        {
            isResetting = true; // Imposta lo stato su "true" quando il metodo inizia
            //Debug.Log("Inizio TotResetButton");

            CollectAndReset();
            OriginalPositions();
        }
        finally
        {
            isResetting = false;
            //Debug.Log("Fine TotResetButton");
        }

    }


    public void LaunchSuccess(Vector3 posCabeza, Quaternion rotCabeza)
    {
        //GameObject tick = GameObject.Find("Nivel_ok");
        logSaver.SetLogEvent("Lanza_Acierto");
        ////Debug.Log("BOXCOLL tiiick");
        tick.SetActive(true);
        Vector3 targetPosition = Camera.main.transform.position + Camera.main.transform.forward * 0.5f + new Vector3(0f, 0.34f, 0f);
        Quaternion cameraRotation = Camera.main.transform.rotation;
        Quaternion yRotation = Quaternion.Euler(0, cameraRotation.eulerAngles.y, 0);
        //Quaternion yRotation = Quaternion.Euler(0, rotCabeza.eulerAngles.y, 0);
        tick.transform.SetPositionAndRotation(targetPosition, yRotation); //new Vector3(posCabeza.x, posCabeza.y + 0.1f, posCabeza.z + 0.5f), yRotation);
        audioSource_ok.Play();
        StartCoroutine(WaitSeconds(tick));
    }

    public void LaunchError(Vector3 posCabeza, Quaternion rotCabeza)
    {
        logSaver.SetLogEvent("Lanza_Error");
        ////Debug.Log("BOXCOLL bwwwaah");
        //GameObject tick = GameObject.Find("Nivel_ok");
        cross.SetActive(true);
        Vector3 targetPosition = Camera.main.transform.position + Camera.main.transform.forward * 0.5f + new Vector3(0f, 0.34f, 0f);
        Quaternion cameraRotation = Camera.main.transform.rotation;
        Quaternion yRotation = Quaternion.Euler(0, cameraRotation.eulerAngles.y, 0);
        //Quaternion yRotation = Quaternion.Euler(0, rotCabeza.eulerAngles.y, 0);
        cross.transform.SetPositionAndRotation(targetPosition, yRotation); //new Vector3(posCabeza.x, posCabeza.y + 0.1f, posCabeza.z + 0.5f), yRotation);
        audioSource_error.Play();
        StartCoroutine(WaitSeconds(cross));
    }

    IEnumerator WaitSeconds(GameObject object_deactivate)
    {
        yield return new WaitForSeconds(3f);
        object_deactivate.SetActive(false);
    }

    void DisablePhysics(Transform pM)
    {
        // Disable MeshCollider if it exists
        MeshCollider meshCollider = pM.GetComponent<MeshCollider>();
        if (meshCollider != null)
        {
            meshCollider.enabled = false;
        }

        // Disable Rigidbody if it exists by setting isKinematic to true
        Rigidbody rb = pM.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // This makes the object unaffected by physics
        }
    }

    void EnablePhysics(Transform pM)
    {
        MeshCollider meshCollider = pM.GetComponent<MeshCollider>();
        if (meshCollider != null)
        {
            meshCollider.enabled = true;
        }

        // Disable Rigidbody if it exists by setting isKinematic to true
        Rigidbody rb = pM.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false; // This makes the object affected by physics
        }
    }

    void RestartScene()
    {
        Transform p = GetChildWithTag(RightController, "DynamicObject");
        ////Debug.Log("BOXCOLL prod " + p.ToString());
        if (p != null)
        {
            ////Debug.Log("BOXCOLL prod " + p.name);
            p.parent = null;
            p.gameObject.SetActive(false);
        }
        reset = true;
        VN.erroresNivel = 0;
        VN.reiniciaNivel = false;
        VN.corazones = 3;
        repeatCount = 0;
        objetosRepuestos = 0;
        objetosRipostiperFrame = 0;
        time = 0;
        new_turn = true;
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    void DisableCollisions()
    {
        foreach (GameObject shelve in game_shelves)
        {
            foreach (Transform box in shelve.transform)
            {
                Transform productsTransform = box.Find("products");
                if (productsTransform != null)
                {
                    foreach (Transform p in productsTransform)
                    {
                        // Disable MeshCollider and Rigidbody for each "pM"
                        DisablePhysics(p);
                    }
                }
            }
        }
    }

    void EnableCollisions()
    {
        foreach (GameObject shelve in game_shelves)
        {
            foreach (Transform box in shelve.transform)
            {
                Transform productsTransform = box.Find("products");
                if (productsTransform != null)
                {
                    foreach (Transform p in productsTransform)
                    {
                        // Disable MeshCollider and Rigidbody for each "pM"
                        EnablePhysics(p);
                    }
                }
            }
        }
    }

    void SetCollisions(bool enable)
    {
        // NEW SET COLLISION
        foreach (GameObject shelve in all_shelves)
        {
            foreach (Transform BOX in shelve.transform)
            {
                Transform front = BOX.Find("front");
                if (front != null)
                {
                    foreach (Transform box in front.transform)
                    {
                        Transform productsTransform = box.Find("products");
                        if (productsTransform != null)
                        {
                            foreach (Transform p in productsTransform)
                            {
                                // Disable MeshCollider and Rigidbody for each "pM"
                                if (enable)
                                {
                                    EnablePhysics(p);
                                }
                                else
                                {
                                    DisablePhysics(p);
                                }
                            }
                        }
                    }
                }
                Transform back = BOX.Find("back");
                if (back != null)
                {
                    foreach (Transform box in back.transform)
                    {
                        Transform productsTransform = box.Find("products");
                        if (productsTransform != null)
                        {
                            foreach (Transform p in productsTransform)
                            {
                                // Disable MeshCollider and Rigidbody for each "pM"
                                if (enable)
                                {
                                    EnablePhysics(p);
                                }
                                else
                                {
                                    DisablePhysics(p);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

}
