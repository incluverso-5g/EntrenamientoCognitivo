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

public class ReponerLogic2 : MonoBehaviour
{
    private GrabObject grabObj;// = GetComponent<GrabObject>();
    public GameObject products;

    public GameObject randomizeShelfPosition_Shelf2;
    public GameObject randomizeShelfPosition_Shelf3;

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
    public AudioSource audioSource_Supera_Errores;

    public GameObject SuperaErrores3;
    public AudioSource audioSource_SuperaErrores3;

    public GameObject canvasInstrucciones;
    public GameObject canvasFinalNivel;
    public GameObject Supera_Errores;
    private AudioSource audioSource_win;
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


    public SupermercadoNiveles SupermercadoNiveles;

    public List<List<GameObject>> shelvesByLevel = new List<List<GameObject>>();
    public List<int> productsByLevel = new List<int>();
    public List<int> RandomProducts = new List<int>();
    public List<int> RandomProductsDiff = new List<int>();

    public int objetosRipostiperFrame = 0;

    void Awake()
    {

    }

    void Start()
    {

        ResetAssignedPositions();


        WaveRig = GameObject.Find("Wave Rig");
        if (WaveRig == null)
        {
            Debug.Log("BOXCOLL can't find WaveRig");
        }
        grabObj = WaveRig.GetComponent<GrabObject>();

        canvasInstrucciones = GameObject.Find("Instrucciones_Iniciales");
        canvasFinalNivel = GameObject.Find("Nivel_finalizado");
        audioSource_win = canvasFinalNivel.GetComponentInChildren<AudioSource>();
        canvasFinalNivel.SetActive(false);
        //Supera_Errores.SetActive(false);


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

        collide_products = !VN.objetosChocan;
    }

    private int repeatCount = 0;
    private int maxRepeats = 3;
    private int maxErrors = 3;
    public int CorazonesVar;



    void Update()
    {
        if (startupTime > 0)
        {

            startupTime -= Time.deltaTime;
        }
        else
        {

            if (VN.objetosChocan != collide_products)
            {
                // Debug.Log("Logging " + VN.objetosChocan);
                SetCollisions(VN.objetosChocan);
                collide_products = VN.objetosChocan;
            }
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
            if (new_fallo)
            {
                //errorCount++;
                VibrateRightController(500, 2);
                LaunchError(WaveRig.transform.position, WaveRig.transform.rotation);
                new_fallo = false;
                //fallos += 1;
                VN.erroresNivel += 1;
                Debug.Log("ERRORE numero: " + VN.erroresNivel);


                if (VN.erroresNivel >= maxErrors)
                {
                    Debug.Log("ERRORE Raggiunti 3 errori. ");

                    VN.erroresNivel = 0;
                    CorazonesVar--;
                    //VN.corazones--;
                    

                    Debug.Log("ERRORE QUANTI SONOOOO" + VN.corazones + "" + CorazonesVar);


                    Debug.Log("ERRORE Corazones rimanenti: " + CorazonesVar);
                    //if (VN.corazones <= 0)
                    if (CorazonesVar <= -3)
                    {
                        if (VN.grabbedObject != null)
                        {
                            VN.grabbedObject.SetActive(false);
                            VN.grabbedObject = null;

                            Debug.Log("Oggetto in mano disattivato per il reset del livello.");
                        }
                        ShowSuperaErrores();

                        Debug.Log("ERRORE GAME OVER: Nessun corazones rimanente.");

                    }
                    else
                    {
                        Show3Errores();
                        repeatCount--;
                        ResetLevelState();

                    }
                }
            }
            else if (new_acierto)
            {
                //Debug.Log("BOXCOLL good one");
                LaunchSuccess(WaveRig.transform.position, WaveRig.transform.rotation);
                new_acierto = false;
                objetosRepuestos += 1;
                objetosRipostiperFrame++;


                if (objetosRipostiperFrame >= NumberProducts)
                {

                    StartCoroutine(DelayedResetAssignedPositions());
                    new_turn = true;
                    time = 0;
                    objetosRipostiperFrame = 0;



                    foreach (Transform randomObject in selectedRandomObjects)
                    {
                        randomObject.gameObject.SetActive(false);
                        Debug.Log($"RANDOM Oggetto '{randomObject.gameObject.name}' disattivato.");
                    }
                    foreach (Transform randomObjectDiff in selectedRandomObjectsDiff)
                    {
                        randomObjectDiff.gameObject.SetActive(false);
                        Debug.Log($"RANDOM Oggetto '{randomObjectDiff.gameObject.name}' disattivato.");
                    }
                }


                if (objetosRepuestos == objetosAReponer)
                {
                    Vector3 targetPosition = Camera.main.transform.position + Camera.main.transform.forward * 0.5f + new Vector3(0f, 0.34f, 0f);
                    Quaternion cameraRotation = Camera.main.transform.rotation;
                    Quaternion yRotation = Quaternion.Euler(0, cameraRotation.eulerAngles.y, 0);

                    canvasFinalNivel.transform.SetPositionAndRotation(targetPosition, yRotation);
                    canvasFinalNivel.SetActive(true);
                    audioSource_win.Play();
                    new_turn = false;


                    //CONTATORE DI LIVELLI

                    if (repeatCount < maxRepeats - 1)
                    {
                        repeatCount++;
                        Invoke("ResetLevelState", 2f);
                        Debug.Log("RIPETIZIONE Ripetizione del livello: " + repeatCount);
                    }
                    else
                    {
                        Debug.Log("RIPETIZIONE Livello completato dopo " + maxRepeats + " ripetizioni.");

                    }

                }
            }

            if (new_turn & time > waiting_time)
            {

                Debug.Log("!!!! Entrato nell'if di new_turn");
                new_turn = false;

                var (shelveNum, childNum, productName, prodottiRimossiShelve2, prodottiRimossiShelve3) = ChooseAndEmptyBox();

                Debug.Log("!!!!" + shelveNum.ToString() + " " + childNum.ToString() + " " + productName);

            }
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
    void ShowSuperaErrores()
    {
        StartCoroutine(ShowSuperaErroresTemporarily());
    }





    IEnumerator ShowSuperaErroresTemporarily()
    {

        Vector3 targetPosition = Camera.main.transform.position + Camera.main.transform.forward * 0.3f + new Vector3(0f, 0.34f, 0f);
        Quaternion cameraRotation = Camera.main.transform.rotation;
        Quaternion yRotation = Quaternion.Euler(0, cameraRotation.eulerAngles.y, 0);

        Supera_Errores.transform.SetPositionAndRotation(targetPosition, yRotation);
        Supera_Errores.SetActive(true);
        audioSource_Supera_Errores.Play();


        yield return new WaitForSeconds(3f);
        Supera_Errores.SetActive(false);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    



    IEnumerator ShowSupera3ErroresTemporarily()
    {
        Vector3 targetPosition = Camera.main.transform.position + Camera.main.transform.forward * 0.3f + new Vector3(0f, 0.34f, 0f);
        Quaternion cameraRotation = Camera.main.transform.rotation;
        Quaternion yRotation = Quaternion.Euler(0, cameraRotation.eulerAngles.y, 0);

        SuperaErrores3.transform.SetPositionAndRotation(targetPosition, yRotation);
        SuperaErrores3.SetActive(true);
        audioSource_SuperaErrores3.Play();

        yield return new WaitForSeconds(3f);
        SuperaErrores3.SetActive(false);
    }

    public void ResetLevelState()
    {   
        Supera_Errores.SetActive(false);
        canvasFinalNivel.SetActive(false);
        objetosRepuestos = 0;
        objetosRipostiperFrame = 0;
        new_turn = true;
        time = 0;
        VN.erroresNivel = 0;

        if (VN.grabbedObject != null)
        {
            VN.grabbedObject.SetActive(false); 
            VN.grabbedObject = null;
            Debug.Log("Oggetto in mano disattivato per il reset del livello.");
        }


        StartCoroutine(DelayedResetAssignedPositions());
       

        foreach (Transform randomObject in selectedRandomObjects)
        {
            randomObject.gameObject.SetActive(false);
            Debug.Log($"Oggetto '{randomObject.gameObject.name}' disattivato per la nuova ripetizione.");
        }

        foreach (Transform randomObjectDiff in selectedRandomObjectsDiff)
        {
            randomObjectDiff.gameObject.SetActive(false);
            Debug.Log($"Oggetto '{randomObjectDiff.gameObject.name}' disattivato per la nuova ripetizione.");
        }
    }


    public int NumberProducts;
    public int NumberProductsRandom;
    public int NumberProductsRandomDiff;
    public void GetShelvesByLevel(int level)
    {
        Debug.LogWarning("!!!! Entrto in GetShelvesByLevel");

       

        // SHELVES

        shelvesByLevel.Add(new List<GameObject> { shelveComida, shelveLibros }); // Livello 1
        shelvesByLevel.Add(new List<GameObject> { shelveComida, shelveLibros }); // Livello 2
        shelvesByLevel.Add(new List<GameObject> { shelveComida, shelveLibros }); // Livello 3
        shelvesByLevel.Add(new List<GameObject> { shelveComida, shelveLibros }); // Livello 4
        shelvesByLevel.Add(new List<GameObject> { shelveComida, shelveLibros, shelveFruta }); // Livello 5
        shelvesByLevel.Add(new List<GameObject> { shelveComida, shelveLibros, shelveFruta }); // Livello 6
        

        Debug.Log("!!!! shelvesByLevel popolato con Count = " + shelvesByLevel.Count);

                // Non e selezionato nessun livello

                if (level < 1 || level > shelvesByLevel.Count)
                {
                    Debug.LogWarning("Livello non valido: " + level);
                    game_shelves = new List<GameObject>(); 
                    return; 
                }
         
                // Aggiorna shelves con livello selezionato

                game_shelves = shelvesByLevel[level - 1]; 
                Debug.LogWarning("Livello Selezionato con: " + game_shelves);
        
                foreach (GameObject shelf in game_shelves)
                {
                    Debug.LogWarning("Quale scaffale: " + shelf.name);
                }

                // PRODOTTI DA RIMETTERE IN ORDINE

                productsByLevel = new List<int> { 1, 1, 2, 2, 3, 2};
                NumberProducts = productsByLevel[level - 1];
                Debug.Log("!!!! Numero di prodotti da scegliere per il livello " + level + ": " + NumberProducts);

                // PRODOTTI RANDOM

                RandomProducts = new List<int> { 0, 0, 1, 2, 3, 4 };
                NumberProductsRandom = RandomProducts[level - 1];

                // PRODOTTI RANDOM NON PRESENTI NEL SUPERMERCATO

                RandomProductsDiff = new List<int> {0, 1, 1, 1, 1 ,2};
                NumberProductsRandomDiff = RandomProductsDiff[level - 1];
            }



            int shelveNumber = 0;
            int small_boxNumber = 0;
            string productName = "";
            public List<List<string>> prodottiRimossiShelve2 = new List<List<string>>();
            public List<List<string>> prodottiRimossiShelve3 = new List<List<string>>();
    
            public (int, int, string, List<List<string>>, List<List<string>>) ChooseAndEmptyBox()
            {

                Debug.Log("!!!! Entrato in ChooseAndEmptyBox");
                Debug.Log("!!!! Valore di objetosRepuestos: " + objetosRepuestos);
       
                   prodottiRimossiShelve2.Clear();
                   prodottiRimossiShelve3.Clear();

                RandomObjectDiff();
                RandomObject();
                for (int i = 0; i < NumberProducts; i++)
                {

                    // SHELVE

                     shelveNumber = UnityEngine.Random.Range(0, game_shelves.Count);  //  ex-> shelveNumber=1
                    GameObject shelve = game_shelves[shelveNumber];
                    Debug.Log("!!!! shelve name: " + shelve.name);


                    //BOX

                    int BOXCount = shelve.transform.childCount; //sceglie un figlio dello shelve. ex: shelve1-> childCount=32

                    Transform randomBOX = null;


                    //controlla che il figlio inizi con BOX (!randomBOX.name.StartsWith("BOX")) e che sia attivo (!randomBOX.gameObject.activeSelf)

                    while (randomBOX == null || !randomBOX.name.StartsWith("BOX") || !randomBOX.gameObject.activeSelf)
                    {
                        int BOXNumber = UnityEngine.Random.Range(0, BOXCount); // scheglie un figlio da (0,32)--> da 0 a 31   ex-->15->BOX_rvine01

                        randomBOX = shelve.transform.GetChild(BOXNumber);  // Si prende una BOX

                    }

                    Debug.Log("!!!! selected BOX : " + randomBOX.name);

                    //BOX--->front

                    Transform front = randomBOX.Find("front");

                    //front --> box_

                    int small_boxCount = front.transform.childCount;  // Numero di box piccoli in front
                     small_boxNumber = UnityEngine.Random.Range(0, small_boxCount);  // Sceglie un box piccolo casuale
                    Transform small_box = front.GetChild(small_boxNumber);  // Ottiene il box piccolo
                    string box_name = small_box.gameObject.name;   // Ottengo il nome del box --> box_rwine01

                    Debug.Log("!!!! box name: " + box_name);


                    // box_ --> products
                    Transform product = small_box.transform.Find("products");


                    if (product != null)
                    {
                        Debug.Log("!!!! Prodotto trovato: " + product.name);



                        if (shelve.name == "ray_store_game2")
                        {
                            List<string> gerarchiaOggetto = new List<string>
                            {
                                randomBOX.name.Replace("(Clone)", ""),
                                front.name,
                                small_box.name,
                                product.name
                            };
                            Debug.Log("!!!! Aggiungo la gerarchia a prodottiRimossiShelve2: " + string.Join(" -> ", gerarchiaOggetto));
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


                            Debug.Log("!!!! Aggiungo la gerarchia a prodottiRimossiShelve3: " + string.Join(" -> ", gerarchiaOggetto3));
                            prodottiRimossiShelve3.Add(gerarchiaOggetto3);
                        }

                        product.gameObject.SetActive(false);
                    }
                    else
                    {
                        Debug.LogError("!!!! Prodotto non trovato in " + small_box.name);
                    }



                    if (randomizeShelfPosition_Shelf2 != null)
                    {
                        Debug.Log("randomizeShelfPosition_Shelf2: " + randomizeShelfPosition_Shelf2.name);

                        RandomizeShelfPositions2 randomizeScript = randomizeShelfPosition_Shelf2.GetComponent<RandomizeShelfPositions2>();

                        if (randomizeScript != null)
                        {
                            Debug.Log("!!! RandomizeShelfPositions2 trovato, passando lista prodottiRimossiShelve2...");
                            randomizeScript.PassaLaLista(prodottiRimossiShelve2);
                        }
                        else
                        {
                            Debug.LogError("!!!! RandomizeShelfPositions2 non trovato su " + randomizeShelfPosition_Shelf2.name);
                        }
                    }
                    else
                    {
                        Debug.LogError("!!!! randomizeShelfPosition_Shelf2 è null");
                    }


                    if (randomizeShelfPosition_Shelf3 != null)
                    {
                        Debug.Log("randomizeShelfPosition_Shelf3: " + randomizeShelfPosition_Shelf3.name);

                        RandomizeShelfPositions3 randomizeScript3 = randomizeShelfPosition_Shelf3.GetComponent<RandomizeShelfPositions3>();

                        if (randomizeScript3 != null)
                        {
                            Debug.Log("!!! RandomizeShelfPositions3 trovato, passando lista prodottiRimossiShelve3...");
                            randomizeScript3.PassaLaLista3(prodottiRimossiShelve3);
                        }
                        else
                        {
                            Debug.LogError("!!!! RandomizeShelfPositions3 non trovato su " + randomizeShelfPosition_Shelf3.name);
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
                    Debug.Log("!!!! reponer Selected product: {productName}");

            
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
                            Debug.Log("!!!! Il prodotto è già attivo. Creando una copia.");
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

                        Debug.Log("!!!! reponer pos: " + selectedPosition);
                        Debug.Log("!!!! reponer prod: " + productToPlace);

                        productToPlace.transform.SetLocalPositionAndRotation(selectedPosition, Quaternion.Euler(product_rot.x, product_rot.y, product_rot.z));
                        productToPlace.gameObject.SetActive(true);

                        Debug.Log("!!!! Esce da ChooseAndEmptyBox");  
                    }
      
                }
        
                return (shelveNumber, small_boxNumber, productName, prodottiRimossiShelve2, prodottiRimossiShelve3);

            }

        public GameObject ProductosFalsos;
        public GameObject ProductosFalsosDiff;
        public List<Transform> selectedRandomObjects = new List<Transform>();
        public List<Transform> selectedRandomObjectsDiff = new List<Transform>();
        public List<Transform> RandomObject()
        {
            Debug.Log("RANDOM Entrato in RandomObject");


            if (NumberProductsRandom == 0)
            {
                Debug.LogWarning("RANDOM Nessun prodotto da selezionare, NumberProductsRandom è 0.");
                return selectedRandomObjects; // Restituisce una lista vuota senza fare altre operazioni
            }

            // PRODOTTI UGUALI

            List<Transform> availableChildren = new List<Transform>();
            selectedRandomObjects.Clear();

            Debug.Log($" RANDOM Numero di figli in 'ProductosFalsos': {ProductosFalsos.transform.childCount}");


            foreach (Transform child in ProductosFalsos.transform)
            {
                if (!child.gameObject.activeSelf)
                {
                    availableChildren.Add(child);
                    Debug.Log($"RANDOM Oggetto disponibile trovato: {child.gameObject.name}");
                }
                else
                {
                    Debug.Log($"RANDOM Oggetto già attivo e ignorato: {child.gameObject.name}");
                }

            }

            for (int i = 0; i < NumberProductsRandom && availableChildren.Count > 0; i++)
            {
                int randomIndex = UnityEngine.Random.Range(0, availableChildren.Count);
                Transform randomChild = availableChildren[randomIndex];


                string randomProductName = randomChild.gameObject.name;

                Debug.Log($"RANDOM Selezionato oggetto: {randomProductName}");
                var (selectedPosition, position_index) = GetRandomAvailablePosition(randomProductName);

                Quaternion originalRotation = randomChild.localRotation;

                Debug.Log($"RANDOM Posizione assegnata per '{randomProductName}': {selectedPosition}");


                randomChild.SetLocalPositionAndRotation(selectedPosition, originalRotation);
                randomChild.gameObject.SetActive(true);

                selectedRandomObjects.Add(randomChild);
                Debug.Log($"RANDOM Oggetto '{randomProductName}' posizionato e attivato");

                availableChildren.RemoveAt(randomIndex);
            }
        
            return selectedRandomObjects;

        }

        public List<Transform> RandomObjectDiff()
        {
            Debug.Log("RANDOM_DIFF Entrato in RandomObjectDiff");

            if (NumberProductsRandomDiff == 0)
            {
                Debug.LogWarning("RANDOM_DIFF Nessun prodotto da selezionare, NumberProductsRandomDiff è 0.");
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
                    Debug.Log($"RANDOM_DIFF Oggetto disponibile trovato: {child.gameObject.name}");
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
                    if(childWithTag != null)
                    {
                        return childWithTag;
                    }
                }
            }

            return null;
        }

        (Vector3, int) GetRandomAvailablePosition(string name)
        {

            Debug.Log("!!!! Posizioni disponibili prima dell'assegnazione: " + string.Join(", ", position_products_assigned.Select(b => b.ToString())));
            List<int> availableIndices = new List<int>();

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

                Debug.Log("!!!! Posizioni disponibili dopo l'assegnazione: " +  string.Join(", ", position_products_assigned.Select(b => b.ToString())));



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




        IEnumerator DelayedResetAssignedPositions()
        {
            
            yield return new WaitForSeconds(5f);
            ResetAssignedPositions();
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
        }

        public void LaunchSuccess(Vector3 posCabeza, Quaternion rotCabeza)
        {
            //GameObject tick = GameObject.Find("Nivel_ok");
            logSaver.SetLogEvent("Lanza_Acierto");
            //Debug.Log("BOXCOLL tiiick");
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
            //Debug.Log("BOXCOLL bwwwaah");
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
            //Debug.Log("BOXCOLL prod " + p.ToString());
            if (p != null)
            {
                //Debug.Log("BOXCOLL prod " + p.name);
                p.parent = null;
                p.gameObject.SetActive(false);
            }
            reset = true;
            VN.erroresNivel = 0;
            VN.reiniciaNivel = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
