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

public class PedidoLogic : MonoBehaviour
{
    private GrabObject grabObj;// = GetComponent<GrabObject>();
    public GameObject products;

    //public GameObject randomizeShelfPosition_Shelf2;
    //public GameObject randomizeShelfPosition_Shelf3;

    private Vector3 position_fruit_box = new Vector3(-27.763f, 1.1989f, 0.731f);

    private List<string> fruits = new List<string> { "perry", "orange", "apple", "pepper_red", "onion", "artichoke", "avocado", "tomato" };

    private Vector3 prod_rot = new Vector3(0, 0, 0);

    private Vector3 book_rot = new Vector3(180, 0, 0);

    private Vector3 dog_cat_rot = new Vector3(90, -90, 0);

    private List<Vector3> position_fruits = new List<Vector3> { new Vector3(0.912f, 0f, -0.078f),
                                                                new Vector3(0.912f, 0f, 0.0373f),
                                                                new Vector3(0.912f, 0f, 0.15f),
                                                                new Vector3(0.751f, 0f, -0.078f),
                                                                new Vector3(0.751f, 0f, 0.0373f),
                                                                new Vector3(0.751f, 0f, 0.15f)};

    private List<Vector3> positions_products = new List<Vector3> { new Vector3(0.425f, 0f, -0.08f),
                                                                new Vector3(0.077f, 0f, -0.08f),
                                                                new Vector3(-0.278f, 0f, -0.08f),
                                                                new Vector3(0.425f, 0f, 0.248f),
                                                                new Vector3(0.077f, 0f, 0.248f),
                                                                new Vector3(-0.278f, 0f, 0.248f)};

    private List<Vector3> bocadilloPositions = new List<Vector3> { new Vector3(2.18f, -0.05f, -0.033f),
                                                                new Vector3(0.759f, -0.05f, -0.033f),
                                                                new Vector3(-0.631f, -0.05f, -0.033f),
                                                                new Vector3(-2.01f, -0.05f, -0.033f),
                                                                new Vector3(2.18f, -0.05f, -0.629f),
                                                                new Vector3(0.759f, -0.05f, -0.629f),
                                                                new Vector3(-0.631f, -0.05f, -0.629f),
                                                                new Vector3(-2.01f, -0.05f, -0.629f)};

    private List<bool> position_fruits_assigned = new List<bool> { false, false, false, false, false, false };
    private List<bool> position_products_assigned = new List<bool> { false, false, false, false, false, false };

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

    // List to store initial state of each box
    private List<BoxInfo> boxesInfo = new List<BoxInfo>();
    // Store the prefab references directly in Unity Editor or programmatically
    public GameObject[] boxPrefabs; // Array of box prefabs (can be assigned in Inspector)

    private Regex pattern = new Regex(@"box_(.+?)( \((\d+)\)| (\d+))?$");

    public GameObject shelveFruta;
    public GameObject shelveLibros;
    public GameObject shelveComida;

    //public bool use_all = false;

    private List<GameObject> game_shelves = new List<GameObject>();
    private List<GameObject> all_shelves = new List<GameObject>();
    
    
    //public bool new_product;
    public int fallos = 0;
    public bool new_fallo = false;
    public bool new_acierto = false;
    public bool new_pedido_completado = false;
    public GameObject tick;
    public AudioSource audioSource_ok;
    public GameObject cross;
   
    public GameObject SuperaErrores3;
    public GameObject Supera_Errores;

    public AudioSource audioSource_SuperaErrores3;
    public AudioSource audioSource_error;
    public AudioSource audioSource_Supera_Errores;

    public GameObject canvasRepeatCount;
    public AudioSource audioSourceRepeatCount;

    public GameObject canvasInstrucciones;
    public GameObject canvasFinalNivel;
    private AudioSource audioSource_win;
    private bool new_turn = true;

    private float time = 0f;
    public float waiting_time = 15f;
    public GameObject WaveRig;
    private Transform RightController;
    private LogSaver logSaver;
    private VariablesNiveles VN;
    public bool collide_products;
    public bool reset = false;
    private float startupTime = 1f;

    public GameObject pedido1;
    public GameObject pedido2;
    public GameObject pedido3;
    public List<GameObject> pedidos;
    public List<GameObject> activePedidos;

    public GameObject randomizeShelfPosition_Shelf2;
    public GameObject randomizeShelfPosition_Shelf3;
    public RandomizeShelfPositions2T2 randShelfPos2;
    public RandomizeShelfPositions3T2 randShelfPos3;
    //private int pedidosToComplete = 4;

    private void Awake()
    {
        //canvasFinalNivel = GameObject.Find("Nivel_finalizado");
        //canvasFinalNivel.SetActive(false);
        //canvasInstrucciones.SetActive(true);
        ResetAssignedPositions();
    }

    private Dictionary<string, Transform> productDictionary = new Dictionary<string, Transform>();

   

    void Start()
    {
        Debug.Log("ERRORE START Controllo Ripetizione=" + totalRepeatCount);

        WaveRig = GameObject.Find("Wave Rig");
        if (WaveRig == null)
        {
            Debug.Log("BOXCOLL can't find WaveRig");
        }
        grabObj = WaveRig.GetComponent<GrabObject>();

        canvasInstrucciones = GameObject.Find("Instrucciones_Iniciales");
        canvasFinalNivel = GameObject.Find("Nivel_finalizado");
        audioSource_win = canvasFinalNivel.GetComponentInChildren<AudioSource>();

        canvasRepeatCount = GameObject.Find("Nivel_finalizado_Ripe");
        audioSourceRepeatCount = canvasRepeatCount.GetComponentInChildren<AudioSource>();

        canvasFinalNivel.SetActive(false);
        canvasRepeatCount.SetActive(false);

        for (int i = 0; i < WaveRig.transform.childCount; i++)
        {
            if (WaveRig.transform.GetChild(i).name == "WaveRightController")
            {
                RightController = WaveRig.transform.GetChild(i);
                //Debug.Log("BOXCOLL right contr " + RightController.name);
            }
        }

        VN = WaveRig.GetComponent<VariablesNiveles>();
        //Debug.Log("BOXCOLL " + VN.ToString());

        tick = GameObject.Find("Nivel_ok");
        audioSource_ok = tick.GetComponentInChildren<AudioSource>();
        tick.SetActive(false);

        cross = GameObject.Find("Nivel_erroneo");
        audioSource_error = cross.GetComponentInChildren<AudioSource>();
        cross.SetActive(false);

        logSaver = WaveRig.GetComponent<LogSaver>();

        randShelfPos2 = randomizeShelfPosition_Shelf2.GetComponent<RandomizeShelfPositions2T2>();
        randShelfPos3 = randomizeShelfPosition_Shelf3.GetComponent<RandomizeShelfPositions3T2>();

        //collide_products = !VN.objetosChocan;

        //Debug.Log("CHOOSE");
        /* List<string> ped1 = ChoosePedidoProducts(1);
         List<string> ped2 = ChoosePedidoProducts(2);
         List<string> ped3 = ChoosePedidoProducts(2);*/

        /* Debug.Log("PLACE");
         BocadilloProducts(ped1, pedido1);
         pedido2.SetActive(false);
         pedido3.SetActive(false);
         //bocadilloProducts(ped2, pedido2); 
         //bocadilloProducts(ped3, pedido3); */

    }


    private int repeatCount = 0;
    private int maxRepeats = 3;
    private int maxErrors = 3;
    
    public int totalRepeatCount = 0;

    void Update()
    {
        if (startupTime > 0)
        {
            startupTime -= Time.deltaTime;
        }
        else
        {
            SetCollisions(true);
            
        }
        if (VN.Reset)
        {
            RecolocaProductos();
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
            //StartCoroutine(ResetLevelState());
            ResetLevelState();
            VN.reiniciaNivel = false;
        } 
        if (!reset)
        {
            time += Time.deltaTime;
            if (new_fallo)
            {
                //Debug.Log("BOXCOLL that was an error");
                VibrateRightController(500, 2);
                LaunchError(WaveRig.transform.position, WaveRig.transform.rotation);
                new_fallo = false;
                VN.erroresNivel += 1;

              /*
                if (RandomizeShelfPositions3T2.instance3T2 != null)
                {
                    RandomizeShelfPositions3T2.instance3T2.RestoreOriginalPositions3T2();
                }

                if (RandomizeShelfPositions2T2.instance2T2 != null)
                {
                    RandomizeShelfPositions2T2.instance2T2.RestoreOriginalPositions2T2();
                }
              */

                Debug.Log("ERRORE numero: " + VN.erroresNivel);

                if (VN.erroresNivel >= maxErrors)
                {

                    Debug.Log("ERRORE Raggiunti 3 errori. ");
                    VN.erroresNivel = 0;
                    //CorazonesVar--;
                    VN.corazones--;
                    logSaver.SetLogEvent("3Errores_Nivel_" + totalRepeatCount + "_ReiniciaNivel");

                    Debug.Log("ERRORE Corazones rimanenti: " + VN.corazones);
                    Debug.Log("ERRORE Controllo Ripetizione="+ totalRepeatCount);

                    if (VN.corazones <= 0)
                    {
                        if (VN.grabbedObject != null)
                        {
                            VN.grabbedObject.SetActive(false);
                            VN.grabbedObject = null;

                            Debug.Log("Oggetto in mano disattivato per il reset del livello.");
                        }

                        StartCoroutine(ShowSuperaErroresAfterDelay(3f));
                        StartCoroutine(DeactivatePedidosAfterDelay(1f));
                        //DeactivatePedidos();
                        //ShowSuperaErroresDelayed();
                        logSaver.SetLogEvent("Pierde3Vidas_Nivel_" + totalRepeatCount + "_VuelveNivelAnterior");
                        Debug.Log("ERRORE GAME OVER: Nessun corazones rimanente.");
                    }
                    else
                    {
                        StartCoroutine(ShowCanvasTemporarily(SuperaErrores3, audioSource_SuperaErrores3));
                        //Show3Errores();
                        totalRepeatCount++;
                        ResetLevelState();
                    }
                }
            }
            else if (new_acierto)
            {
                LaunchSuccess(WaveRig.transform.position, WaveRig.transform.rotation);

                EditableTotalObjects--;
                Debug.Log($"ERRORE Confronto: EditableTotalObjects = {EditableTotalObjects}, TotalObjectsForLevel = {TotalObjectsForLevel}");

               /* if (RandomizeShelfPositions3T2.instance3T2 != null)
                {
                    RandomizeShelfPositions3T2.instance3T2.RestoreOriginalPositions3T2();
                }

                if (RandomizeShelfPositions2T2.instance2T2 != null)
                {
                    RandomizeShelfPositions2T2.instance2T2.RestoreOriginalPositions2T2();
                }*/

                new_acierto = false;

                if (EditableTotalObjects == 0)
                {
                    if (repeatCount < maxRepeats - 1)
                    {
                        StartCoroutine(ShowCanvasTemporarily(canvasRepeatCount, audioSourceRepeatCount));
                        //ShowCanvasRepeatCount();
                        repeatCount++;
                        totalRepeatCount++;

                        ResetLevelState();
                        //StartCoroutine(ResetLevelState());

                        Debug.Log("ERRORE RIPETIZIONE Ripetizione del livello: " + totalRepeatCount);
                        logSaver.SetLogEvent("Nivel_finalizado_" + SceneManager.GetActiveScene().name + "_" + currentLevel + "_Repeticion_" + totalRepeatCount);
                    }
                    else
                    {
                        totalRepeatCount++;
                        StartCoroutine(ShowCanvasFinalAfterDelay(3f));
                        //ShowCanvasFinal();
                        new_turn = false;
                        Debug.Log("ERRORE RIPETIZIONE Livello completato dopo " + totalRepeatCount + " ripetizioni.");
                        logSaver.SetLogEvent("Nivel_finalizado_" + SceneManager.GetActiveScene().name + "_" + currentLevel + "_Repeticion_" + totalRepeatCount);
                        logSaver.SetLogEvent("Fin_Niveles");
                    }
                }  
            }
            //if (time > waiting_time)
            //{
            //    time = 0;
                //NewRandomPedido(1, 4);
                //Debug.Log("!!!! Entrato nell'if di new_turn");
            //    new_turn = false;
                //ResetAssignedPositions();
                //var (shelveNum, childNum, productName, prodottiRimossiShelve2, prodottiRimossiShelve3) = ChooseAndEmptyBox();
                //Debug.Log("!!!!" + shelveNum.ToString() + " " + childNum.ToString() + " " + productName);
            //}     
        }
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

    public void ShowSuperaErroresDelayed()
    {
        StartCoroutine(ShowSuperaErroresAfterDelay(3f)); 
    }

    private IEnumerator ShowSuperaErroresAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        Vector3 targetPosition = Camera.main.transform.position + Camera.main.transform.forward * 0.7f + new Vector3(0f, 0.34f, 0f);
        Quaternion cameraRotation = Camera.main.transform.rotation;
        Quaternion yRotation = Quaternion.Euler(0, cameraRotation.eulerAngles.y, 0);

        Supera_Errores.transform.SetPositionAndRotation(targetPosition, yRotation);
        Supera_Errores.SetActive(true);
        audioSource_Supera_Errores.Play();
        Debug.Log("Supera_Errores visualizzato dopo " + delay + " secondi.");
    }

    IEnumerator ShowSupera3ErroresTemporarily()
    {
        yield return new WaitForSeconds(3f);
        Vector3 targetPosition = Camera.main.transform.position + Camera.main.transform.forward * 0.7f + new Vector3(0f, 0.34f, 0f);
        Quaternion cameraRotation = Camera.main.transform.rotation;
        Quaternion yRotation = Quaternion.Euler(0, cameraRotation.eulerAngles.y, 0);

        SuperaErrores3.transform.SetPositionAndRotation(targetPosition, yRotation);
        SuperaErrores3.SetActive(true);
        audioSource_SuperaErrores3.Play();

        yield return new WaitForSeconds(3f);
        SuperaErrores3.SetActive(false);
    }

    IEnumerator ShowCanvasTemporarily(GameObject canvas, AudioSource audio)
    {
        yield return new WaitForSeconds(2f);
        Vector3 targetPosition = Camera.main.transform.position + Camera.main.transform.forward * 0.7f + new Vector3(0f, 0.34f, 0f);
        Quaternion cameraRotation = Camera.main.transform.rotation;
        Quaternion yRotation = Quaternion.Euler(0, cameraRotation.eulerAngles.y, 0);

        canvas.transform.SetPositionAndRotation(targetPosition, yRotation);
        canvas.SetActive(true);
        audio.Play();
        yield return new WaitForSeconds(3f);
        canvas.SetActive(false);
    }


    private void ShowCanvasRepeatCount()
    {
        Debug.Log("Entrato in ShowCanvasRepeatCount");
        Vector3 targetPosition = Camera.main.transform.position + Camera.main.transform.forward * 0.7f + new Vector3(0f, 0.34f, 0f);
        Quaternion cameraRotation = Camera.main.transform.rotation;
        Quaternion yRotation = Quaternion.Euler(0, cameraRotation.eulerAngles.y, 0);

        canvasRepeatCount.transform.SetPositionAndRotation(targetPosition, yRotation);
        canvasRepeatCount.SetActive(true);


        if (audioSourceRepeatCount != null)
        {
            audioSourceRepeatCount.Play();
            Debug.Log("Audio di repeatCount riprodotto.");
        }
        else
        {
            Debug.LogWarning("Audio Source per repeatCount non trovato.");
        }

        StartCoroutine(HideCanvasAfterSeconds(canvasRepeatCount, 3f));
        Debug.Log("Canvas repeatCount mostrato correttamente.");
    }

    private IEnumerator HideCanvasAfterSeconds(GameObject canvas, float delay)
    {
        yield return new WaitForSeconds(delay);
        canvas.SetActive(false);
        Debug.Log($"{canvas.name} nascosto dopo {delay} secondi.");
    }

    private void ShowCanvasFinal()
    {
        Debug.Log("Entrato in ShowCanvasFinal");
        Vector3 targetPosition = Camera.main.transform.position + Camera.main.transform.forward * 0.7f + new Vector3(0f, 0.34f, 0f);
        Quaternion cameraRotation = Camera.main.transform.rotation;
        Quaternion yRotation = Quaternion.Euler(0, cameraRotation.eulerAngles.y, 0);

        canvasFinalNivel.transform.SetPositionAndRotation(targetPosition, yRotation);
        canvasFinalNivel.SetActive(true);
        audioSource_win.Play();
        Debug.Log("Canvas mostrato correttamente.");
    }

    private IEnumerator ShowCanvasFinalAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log("Entrato in ShowCanvasFinal");
        Vector3 targetPosition = Camera.main.transform.position + Camera.main.transform.forward * 0.7f + new Vector3(0f, 0.34f, 0f);
        Quaternion cameraRotation = Camera.main.transform.rotation;
        Quaternion yRotation = Quaternion.Euler(0, cameraRotation.eulerAngles.y, 0);

        canvasFinalNivel.transform.SetPositionAndRotation(targetPosition, yRotation);
        canvasFinalNivel.SetActive(true);
        audioSource_win.Play();
    }



    public SupermercadoNiveles SupermercadoT2Niveles;

    public List<List<GameObject>> shelvesByLevelT2 = new List<List<GameObject>>();
    public List<List<(GameObject pedido, int numOggetti)>> PedidosByLevelT2 = new List<List<(GameObject, int)>>();
    public List<(GameObject pedido, int numOggetti)> pedidosForLevel;
    public List<List<GameObject>> pedido = new List<List<GameObject>>();
    public int numOggetti;



    public int TotalObjectsForLevel { get; private set; } // Variabile di sola lettura Tot obj.
    public int EditableTotalObjects;
    public int currentLevel { get; private set; }

    public void GetShelvesByLevelT2(int level)
    {
        Debug.LogWarning("!!!! Entrto in GetShelvesByLevel");
        currentLevel = level;
        if (game_shelves != null)
        {
            foreach (GameObject shelf in game_shelves)
            {
                foreach (Transform child in shelf.transform)
                {
                    child.gameObject.SetActive(false); // Disattiva ogni figlio
                }
            }
        }

        pedidos = new List<GameObject> { pedido1, pedido2, pedido3 };
        // SHELVES

        shelvesByLevelT2.Add(new List<GameObject> { shelveComida }); // Livello 1
        shelvesByLevelT2.Add(new List<GameObject> { shelveComida }); // Livello 2
        shelvesByLevelT2.Add(new List<GameObject> { shelveComida, shelveLibros }); // Livello 3
        shelvesByLevelT2.Add(new List<GameObject> { shelveComida, shelveLibros }); // Livello 4
        shelvesByLevelT2.Add(new List<GameObject> { shelveComida, shelveLibros, shelveFruta }); // Livello 5
        shelvesByLevelT2.Add(new List<GameObject> { shelveComida, shelveLibros, shelveFruta }); // Livello 6


        Debug.Log("!!!! shelvesByLevel popolato con Count = " + shelvesByLevelT2.Count);

        // PEDIDOS Y OBJ PER PEDIDOS

        PedidosByLevelT2.Add(new List<(GameObject, int)>
        {
            (pedido1, 5), // Livello 1
        });

        PedidosByLevelT2.Add(new List<(GameObject, int)>
        {
            (pedido1, 5), // Livello 2
           // (pedido2, 2), 
           
        });

        PedidosByLevelT2.Add(new List<(GameObject, int)>
        {
            (pedido1, 2), // Livello 3
            (pedido2, 4), 
            
        });

        PedidosByLevelT2.Add(new List<(GameObject, int)>
        {
            (pedido1, 3), // Livello 4
            (pedido2, 3), 
             
        });

        PedidosByLevelT2.Add(new List<(GameObject, int)>
        {
            (pedido1, 2), // Livello 5
            (pedido2, 3), 
            (pedido3, 3), 
            
        });

        PedidosByLevelT2.Add(new List<(GameObject, int)>
        {
            (pedido1, 1), // Livello 6
            (pedido2, 4), 
            (pedido3, 3), 
           
        });


        // Controllo livello (non selezionato)

        if (level < 1 || level > shelvesByLevelT2.Count)
        {
            Debug.LogWarning("Livello non valido: " + level);

            game_shelves = new List<GameObject>();
            TotalObjectsForLevel = 0;
            EditableTotalObjects = 0;
            return;
        }

        // Aggiorna shelves con livello selezionato

        game_shelves = shelvesByLevelT2[level - 1];
        Debug.LogWarning("Livello Selezionato con: " + game_shelves);

        foreach (GameObject shelf in game_shelves)
        {
            Debug.LogWarning(" Quale scaffale: " + shelf.name);
        }

        // Aggiorna Pedidos

        if (level <= PedidosByLevelT2.Count)
        {
            pedidosForLevel = PedidosByLevelT2[level - 1];

            TotalObjectsForLevel = pedidosForLevel.Sum(pedido => pedido.numOggetti);
            EditableTotalObjects = TotalObjectsForLevel; 
            Debug.Log($"PEDIDO Totale oggetti per livello {level}: {TotalObjectsForLevel}");


            foreach (var pedido in pedidos)
            {
                if (pedidosForLevel.Any(p => p.pedido == pedido))
                {

                   pedido.SetActive(true);
                    Debug.Log($"Pedido {pedido.name} scelto");
                }
                else
                {
                    pedido.SetActive(false);
                    Debug.Log($"Pedido {pedido.name} disattivato.");
                }

            }

                StartCoroutine(HandlePedidosAfterDelay(level, 0.5f));
           
        }
        else
        {
            Debug.LogWarning($"PEDIDO Nessun Pedido configurato per il livello {level}.");
        }
    }
    /*
    private IEnumerator HandlePedidosAfterDelay(int level, float delay)
    {
       Debug.Log($"PEDIDO Aspetto {delay} secondi prima di configurare i pedidos...");
       yield return new WaitForSeconds(delay);
       EditableTotalObjects = TotalObjectsForLevel;

        Debug.Log($"PEDIDO Configurazione dei pedidos per il livello {level} dopo {delay} secondi.");
        Debug.Log($"PEDIDOS Numero di elementi in pedidosForLevel: {pedidosForLevel?.Count ?? 0}");

        // Ottieni i prodotti per ogni pedido
        Dictionary<GameObject, List<string>> productsPerPedido = ChoosePedidoProducts(level, pedidosForLevel);

        StartCoroutine(BocadilloProducts(products, productsPerPedido));
        //BocadilloProducts(products, productsPerPedido);
    }*/

    private void ClearActiveChildrenDynamicObjects(Transform parent)
    {
        foreach(Transform child in parent)
        {
            if (child.CompareTag("DynamicObject"))
            {
                child.transform.parent = null;
                child.gameObject.SetActive(false);
                Debug.Log($"PEDIDO: {child.name} active={child.gameObject.activeSelf}");
            }
        }
    }


    private IEnumerator HandlePedidosAfterDelay(int level, float delay)
    {
        yield return new WaitForSeconds(1f);
        foreach (var (pedido, _) in pedidosForLevel)
        {
            pedido.SetActive(false);
        }

        Debug.Log($"CORUTINA Aspetto {delay} secondi prima di configurare i pedidos...");
        yield return new WaitForSeconds(delay-1);

        // Aggiorna il conteggio degli oggetti
        EditableTotalObjects = TotalObjectsForLevel;
        Debug.Log($"CORUTINA Configurazione dei pedidos per il livello {level} dopo {delay} secondi.");
        Debug.Log($"CORUTINA Numero di elementi in pedidosForLevel: {pedidosForLevel?.Count ?? 0}");

        
        if (pedidosForLevel == null || pedidosForLevel.Count == 0)
        {
            Debug.LogError("CORUTINA: pedidosForLevel è null o vuoto. Impossibile configurare i pedidos.");
            yield break;
        }


        Debug.Log("CORUTINA: Inizio la pulizia dei bocadillos...");
        foreach (var (pedido, _) in pedidosForLevel)
        {
            if (pedido == null)
            {
                Debug.LogError("CORUTINA: Un pedido è null. Saltando.");
                continue;
            }

            if (pedidosForLevel.Count > 0)
            {
                var (firstPedido, _) = pedidosForLevel[0];
                firstPedido.SetActive(true);
            }

            Transform bocadillo = pedido.transform.Find("Bocadillo")
                                ?? pedido.transform.Find("BocadilloLeftSuper")
                                ?? pedido.transform.Find("BocadilloCenterSuper");

            if (bocadillo != null)
            {
                foreach (Transform child in bocadillo)
                {
                    if (!child.name.Contains("Canvas"))
                    {
                        Destroy(child.gameObject);
                    }   
                }
            }
        }

       // Ottieni i prodotti per ogni pedido

        Dictionary<GameObject, List<string>> productsPerPedido = null;
        try
        {
            productsPerPedido = ChoosePedidoProducts(level, pedidosForLevel);
            foreach (var entry in productsPerPedido)
            {
                Debug.Log($"CORUTINA PRODOTTI PER PEDIDO '{entry.Key.name}': {string.Join(", ", entry.Value)}");
            }

            if (productsPerPedido == null || productsPerPedido.Count == 0)
            {
                Debug.LogError("CORUTINA: productsPerPedido è null o vuoto. Nessun prodotto assegnato ai pedidos.");
                yield break;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"CORUTINA: Errore durante ChoosePedidoProducts - {ex.Message}\n{ex.StackTrace}");
            yield break;
        }

        // Avvia la coroutine per posizionare i prodotti
        try
        {
            StartCoroutine(BocadilloProducts(products, productsPerPedido));
            Debug.Log("CORUTINA: Avviata la configurazione dei prodotti nei bocadillos.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"CORUTINA: Errore durante BocadilloProducts - {ex.Message}\n{ex.StackTrace}");
        }
    }


    private Dictionary<GameObject, List<string>> productsPerPedido;

    public Dictionary<GameObject, List<string>> ChoosePedidoProducts(int level, List<(GameObject pedido, int numOggetti)> pedidosForLevel)
    {
        productsPerPedido = new Dictionary<GameObject, List<string>>();

        /*
        Debug.Log($"ChoosePedidoProducts --- Inizio stampa figli attivi degli scaffali ---");
        foreach (GameObject shelf in game_shelves)
        {
            Debug.Log($"ChoosePedidoProducts SCAFFALE: {shelf.name}");

            foreach (Transform child in shelf.transform)
            {
                if (child.gameObject.activeSelf) 
                {
                    Debug.Log($"ChoosePedidoProducts  Figlio attivo: {child.name}");
                }
            }
        }
        Debug.Log($"ChoosePedidoProducts--- Fine stampa figli attivi degli scaffali ---");
        */

        foreach (var (pedido, numOggetti) in pedidosForLevel)
        {
            Debug.Log($"ChoosePedidoProducts PEDIDOS: Inizio scelta prodotti per '{pedido.name}' con {numOggetti} oggetti.");

            List<string> chosenProducts = new List<string>();
            HashSet<Transform> usedBoxes = new HashSet<Transform>();
            int prodottiAggiunti = 0;

            int shelfIndex = 0;
            HashSet<GameObject> usedShelves = new HashSet<GameObject>();

            while (prodottiAggiunti < numOggetti)
            {
                GameObject shelf = null;
                int attempts = 0;

                while (shelf == null || usedShelves.Contains(shelf))
                {
                    if (attempts >= game_shelves.Count)
                    {
                        //Debug.LogWarning("ChoosePedidoProducts SCAFFALE: Tutti gli scaffali sono stati utilizzati. Ripristino.");
                        usedShelves.Clear();
                    }

                    shelf = game_shelves[shelfIndex];
                    shelfIndex = (shelfIndex + 1) % game_shelves.Count;
                    attempts++;
                }

                usedShelves.Add(shelf);

                var activeChildren = shelf.transform.Cast<Transform>()
                .Where(child => child.gameObject.activeSelf)
                .ToList();


                int BOXCount = activeChildren.Count;
                if (BOXCount == 0)
                {
                   // Debug.LogWarning($"ChoosePedidoProducts SCAFFALE: Lo scaffale '{shelf.name}' è vuoto. Passo al prossimo scaffale.");
                    continue;
                }

                Transform randomBOX = null;
                attempts = 0;

                while (randomBOX == null || usedBoxes.Contains(randomBOX))
                {
                    if (attempts >= BOXCount)
                    {
                       // Debug.LogWarning($"ChoosePedidoProducts SCAFFALE: Tutte le BOX di '{shelf.name}' sono state utilizzate. Passo al prossimo scaffale.");
                        break;
                    }

                    int BOXNumber = UnityEngine.Random.Range(0, BOXCount);
                    randomBOX = activeChildren[BOXNumber];

                    if (randomBOX != null && randomBOX.name.StartsWith("BOX") && randomBOX.gameObject.activeSelf && !usedBoxes.Contains(randomBOX))
                    {
                        usedBoxes.Add(randomBOX);
                        //Debug.Log($"ChoosePedidoProducts SCAFFALE: Selezionata BOX '{randomBOX.name}' da scaffale '{shelf.name}'");
                        break;
                    }
                    attempts++;
                }

                if (randomBOX == null) continue;

                Transform front = randomBOX.Find("front");
                if (front == null) continue;

                Transform productContainer = front.GetChild(0)?.Find("products");
                if (productContainer != null && productContainer.childCount > 0)
                {
                    Transform product = productContainer.GetChild(0);

                    string cleanedProductName = product.name.Split(' ')[0].Trim();
                    if (!chosenProducts.Contains(cleanedProductName))
                    {
                        chosenProducts.Add(cleanedProductName);
                        prodottiAggiunti++;
                        Debug.Log($"ChoosePedidoProducts PRODOTTO: Aggiunto '{cleanedProductName}' da BOX '{randomBOX.name}' nello scaffale '{shelf.name}'");
                    }
                }
            }

            productsPerPedido[pedido] = chosenProducts;

        }

        foreach (var pedido in productsPerPedido)
        {
            Debug.Log($"ChoosePedidoProducts - Contenuto per '{pedido.Key.name}':");

            if (pedido.Value.Count > 0)
            {
                foreach (var product in pedido.Value)
                {
                    Debug.Log($"ChoosePedidoProducts --- Prodotto: {product}");
                }
            }    
        }   
        return productsPerPedido;
    }
    


    public IEnumerator BocadilloProducts(GameObject productParent, Dictionary<GameObject, List<string>> productsPerPedido)
    {
        List<GameObject> orderedPedidos = new List<GameObject> { pedido1, pedido3, pedido2 };

        foreach (var pedidoEntry in productsPerPedido)
        {
            GameObject pedido = pedidoEntry.Key;
            List<string> prodList = pedidoEntry.Value;

            if (pedido == null)
            {
                Debug.LogError("BocadilloProducts PEDIDO: Il GameObject 'pedido' è null.");
                continue;
            }

         //Disattivo tutti i pedidos

            //foreach (var entry in productsPerPedido)
            //{
            //  entry.Key.SetActive(false);
            //}

            pedido.SetActive(true);
            Debug.Log($"BocadilloProducts: Pedido scelto: '{pedido.name}' con prodotti: {string.Join(", ", prodList)}");


            Transform bocadillo = pedido.transform.Find("BocadilloCenterSuper")
                              ?? pedido.transform.Find("Bocadillo")
                              ?? pedido.transform.Find("BocadilloLeftSuper");

            if (bocadillo == null)
            {
                Debug.LogError($"BocadilloProducts PEDIDO '{pedido.name}' non contiene un figlio chiamato 'Bocadillo', 'BocadilloLeftSuper' o 'BocadilloCenterSuper'.");
                continue;
            }

            if (bocadillo.childCount > 0)
            {
                Debug.LogWarning($"BocadilloProducts Attenzione: Bocadillo di '{pedido.name}' non vuoto (Canvas incluso ). Pulisco manualmente.");
                ClearBocadillo(bocadillo);
            }


            int i = 0; // Contatore per posizionare i prodotti
            HashSet<Vector3> usedPositions = new HashSet<Vector3>(); // Traccia le posizioni occupate

            foreach (string productName in prodList)
            {
                //POSIZIONAMENTO

                if (i >= bocadilloPositions.Count)
                {
                    Debug.LogWarning($"BocadilloProducts PEDIDO '{pedido.name}' non ha abbastanza posizioni in 'bocadilloPositions' per tutti i prodotti.");
                    break;
                }
               
                Vector3 selectedPosition = bocadilloPositions[i]; // sceglie una posizione dalla lista


                if (usedPositions.Contains(selectedPosition))
                {
                    Debug.LogWarning($"BocadilloProducts Posizione {selectedPosition} già utilizzata. Passo alla successiva'.");
                    i++; 
                    continue;
                }
                usedPositions.Add(selectedPosition);

                string cleanedProductName = productName.Split(' ')[0];
                Transform productToClone = null;

                foreach (Transform child in products.transform)
                {
                    if (child.name.Equals(cleanedProductName, StringComparison.OrdinalIgnoreCase))
                    {
                        productToClone = child;
                        break; 
                    }
                }


                if (productToClone == null)
                {
                    Debug.LogError($"BocadilloProducts '{pedido.name}' - Il prodotto '{cleanedProductName}' non è stato trovato né nel dizionario né tra i figli di '{productParent.name}'.");
                    Debug.Log("BocadilloProducts  Contenuto di productDictionary:");
                    foreach (var entry in productDictionary)
                    {
                        Debug.Log($"BocadilloProducts  Key: {entry.Key}, Value: {entry.Value.name}");
                    }
                    continue;
                }
                if (productToClone == null)
                {
                    Debug.LogError($"BocadilloProducts - Il prodotto '{cleanedProductName}' non è stato trovato tra i figli di '{products.name}'.");
                }
                else
                {
                    Debug.Log($"Prodotto trovato: {productToClone.name}");
                }

                if (productName.Contains("pizza"))
                {
                    selectedPosition += new Vector3(0, 0, 0.257f);
                }
                else if (productName.Contains("artichoke") || productName.Contains("avocado"))
                {
                    selectedPosition += new Vector3(0, 0, 0.257f);
                }

                Transform newProd = Instantiate(productToClone);
                if (newProd == null)
                {
                    Debug.LogError($"BocadilloProductsPEDIDO '{pedido.name}' - Impossibile istanziare il prodotto '{productName}'.");
                    continue;
                }

                DisablePhysics(newProd);
                newProd.transform.position = bocadillo.TransformPoint(selectedPosition);
                newProd.SetParent(bocadillo, worldPositionStays: true);
                newProd.gameObject.SetActive(true);

                Debug.Log($"BocadilloProductsPEDIDO '{pedido.name}' - Prodotto '{productName}' posizionato correttamente in '{bocadillo.name}' a posizione {newProd.transform.position}");

                i++;
            }

            yield return new WaitForSecondsRealtime(5f);

            //Debug.Log($"BocadilloProducts PEDIDO Aspetto che il bocadillo di '{pedido.name}' venga svuotato...");
            //yield return new WaitUntil(() => !pedido.activeSelf);
            //Debug.Log($"BocadilloProductsPEDIDO '{pedido.name}' completato e disattivato.");
        }
    }


    public IEnumerator DeactivatePedidosAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        DeactivatePedidos();
    }

    public void DeactivatePedidos()
    {
        foreach (var (pedido, _) in pedidosForLevel)
        {
            if (pedido == null)
            {
                Debug.LogError(" ResetLevelState Pedido mancante! Controlla che tutti i pedidos siano assegnati correttamente.");
                continue;
            }

            Transform bocadillo = pedido.transform.Find("BocadilloCenterSuper")
                              ?? pedido.transform.Find("Bocadillo")
                              ?? pedido.transform.Find("BocadilloLeftSuper");

            if (bocadillo != null)
            {
                ClearBocadillo(bocadillo);
            }
            pedido.SetActive(false);
        }
    }


    public void ResetLevelState()
    {
       
        VN.erroresNivel = 0;
        VN.reiniciaNivel = false;

        Debug.Log("Entrato in ResetLevelState");

        if (VN.grabbedObject != null)
        {
            VN.grabbedObject.SetActive(false);
            VN.grabbedObject = null;
            Debug.Log(" ResetLevelState Oggetto in mano disattivato per il reset del livello.");
        }

        Supera_Errores.SetActive(false);
        canvasFinalNivel.SetActive(false);

        // Pulisci e riattiva i pedidos necessari
        //DeactivatePedidos();
        StartCoroutine(DeactivatePedidosAfterDelay(1f));

        EditableTotalObjects = TotalObjectsForLevel;
        StartCoroutine(HandlePedidosAfterDelay(currentLevel, 5f));
    }



    private void ClearBocadillo(Transform bocadillo)
    {
 
        foreach (Transform child in bocadillo)
        {
            if (!child.name.Contains("Canvas"))
            {
                Destroy(child.gameObject);
            }
        } 
    }




    private void RecolocaProductos()
    {
        if (randShelfPos3 != null) //RandomizeShelfPositions3.instance3 != null)
        {
            //Debug.Log(" Restore original reset level state");
            //RandomizeShelfPositions3.instance3.RestoreOriginalPositions3();
            randShelfPos3.RestoreOriginalPositions3T2();
        }

        if (randShelfPos2 != null) //RandomizeShelfPositions2.instance != null)
        {
            //RandomizeShelfPositions2.instance.RestoreOriginalPositions();
            randShelfPos2.RestoreOriginalPositions2T2();
        }
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
        List<int> availableIndices = new List<int>();

        if (fruits.Contains(name))
        {
            for (int i = 0; i < position_fruits_assigned.Count; i++)
            {
                if (!position_fruits_assigned[i])
                {
                    availableIndices.Add(i);
                }
            }
            if (availableIndices.Count == 0)
            {
                Debug.LogError("!!!! No available positions.");
                throw new InvalidOperationException(" No available positions.");
            }
            int randomIndex = availableIndices[UnityEngine.Random.Range(0, availableIndices.Count)];

            // Mark the position as taken
            position_fruits_assigned[randomIndex] = true;
            Debug.Log(" !!!! Assigned fruit position: {position_fruits[randomIndex]} at index: {randomIndex}");
            return (position_fruits[randomIndex], randomIndex);
        }
        else
        {
            for (int i = 0; i < position_products_assigned.Count; i++)
            {
                if (!position_products_assigned[i])
                {
                    availableIndices.Add(i);
                }
            }
            if (availableIndices.Count == 0)
            {
                Debug.LogError("!!!! No available positions.");
                throw new InvalidOperationException("No available positions.");
            }
            int randomIndex = availableIndices[UnityEngine.Random.Range(0, availableIndices.Count)];


            position_products_assigned[randomIndex] = true;
            Debug.Log($"!!!! product position: {positions_products[randomIndex]} at index: {randomIndex}");
            return (positions_products[randomIndex], randomIndex);
        }
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


    void ResetAssignedPositions()
    {
        //position_fruits_assigned = new List<bool> { false, false, false, false, false, false, false};
        for (int i = 0; i < position_fruits_assigned.Count; i++)
        {
            position_fruits_assigned[i] = false;
        }
        // position_products_assigned = new List<bool> { false, false, false, false, false, false};
        for (int i = 0; i < position_products_assigned.Count; i++)
        {
            position_products_assigned[i] = false;
        }
        foreach (Transform child in products.transform)
        {
            child.gameObject.SetActive(false);
        }
    }


    public void LaunchSuccess(Vector3 posCabeza, Quaternion rotCabeza)
    {
        //GameObject tick = GameObject.Find("Nivel_ok");
        logSaver.SetLogEvent("Lanza_Acierto");
        //Debug.Log("BOXCOLL tiiick");
        tick.SetActive(true);
        Vector3 targetPosition = Camera.main.transform.position + Camera.main.transform.forward * 0.7f + new Vector3(0f, 0.34f, 0f);
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
        //Debug.Log("BOXCOLL bwwwaah");
        //GameObject tick = GameObject.Find("Nivel_ok");
        cross.SetActive(true);
        Vector3 targetPosition = Camera.main.transform.position + Camera.main.transform.forward * 0.7f + new Vector3(0f, 0.34f, 0f);
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
