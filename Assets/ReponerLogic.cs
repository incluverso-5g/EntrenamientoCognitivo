using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI.Table;

public class ReponerLogic : MonoBehaviour
{
    private GrabObject grabObj;// = GetComponent<GrabObject>();
    public GameObject products;

    private Vector3 position_fruit_box = new Vector3(-27.763f, 1.1989f, 0.731f);

    private List<string> fruits = new List<string> { "perry", "orange", "apple", "pepper_red", "onion", "artichoke", "avocado", "tomato"};

    private Vector3 prod_rot = new Vector3(270, 0, 0);

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

    private List<bool> position_fruits_assigned = new List<bool> { false, false, false, false, false, false};
    private List<bool> position_products_assigned = new List<bool> { false, false, false, false, false, false};

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

    public bool use_all = false;

    private List<GameObject> game_shelves = new List<GameObject>();
    private List<GameObject> all_shelves = new List<GameObject>();
    public int objetosAReponer = 15;
    private int objetosRepuestos = 0;
    public int objetosAlaVez = 1;
    //public bool new_product;
    public int fallos = 0;
    public bool new_fallo = false;
    public bool new_acierto = false;
    public GameObject tick;
    public AudioSource audioSource_ok;
    public GameObject cross;
    public AudioSource audioSource_error;

    public GameObject canvasInstrucciones;
    public GameObject canvasFinalNivel;
    public AudioSource audioSource_win;
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
    private void Awake()
    {
        //canvasFinalNivel = GameObject.Find("Nivel_finalizado");
        //canvasFinalNivel.SetActive(false);
        //canvasInstrucciones.SetActive(true);
        ResetAssignedPositions();
    }

    // Start is called before the first frame update
    void Start()
    {
        all_shelves = new List<GameObject> { shelveFruta, shelveComida, shelveLibros };
        if (use_all)
        {
            game_shelves = new List<GameObject> { shelveFruta, shelveComida, shelveLibros };
        }
        else
        {
            game_shelves = new List<GameObject> {shelveComida};
        }

        WaveRig = GameObject.Find("Wave Rig");
        grabObj = WaveRig.GetComponent<GrabObject>();

        canvasFinalNivel = GameObject.Find("Nivel_finalizado");
        audioSource_win = canvasFinalNivel.GetComponentInChildren<AudioSource>();
        canvasFinalNivel.SetActive(false);

        for (int i = 0; i< WaveRig.transform.childCount; i++)
        {
            if (WaveRig.transform.GetChild(i).name== "WaveRightController")
            {
                RightController = WaveRig.transform.GetChild(i);
                //Debug.Log("BOXCOLL right contr " + RightController.name);
            }
        }

        VN = WaveRig.GetComponent<VariablesNiveles>();

        tick = GameObject.Find("Nivel_ok");
        audioSource_ok = tick.GetComponentInChildren<AudioSource>();
        tick.SetActive(false);


        cross = GameObject.Find("Nivel_erroneo");
        audioSource_error = cross.GetComponentInChildren<AudioSource>();
        cross.SetActive(false);

        logSaver = WaveRig.GetComponent<LogSaver>();

        collide_products = !VN.objetosChocan;
    }
    // Update is called once per frame
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
                Debug.Log("Logging " + VN.objetosChocan);
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

        if(VN.reiniciaNivel == true)
        {
            RestartScene();
            VN.reiniciaNivel = false;
        }
        if (!reset)
        {
            time += Time.deltaTime;
            if (new_fallo)
            {
                //Debug.Log("BOXCOLL that was an error");
                LaunchError(WaveRig.transform.position, WaveRig.transform.rotation);
                new_fallo = false;
                //fallos += 1;
                VN.erroresNivel += 1; // fallos;
            }
            else if (new_acierto)
            {
                //Debug.Log("BOXCOLL good one");
                new_acierto = false;
                objetosRepuestos += 1;
                if (objetosRepuestos < objetosAReponer)
                {
                    new_turn = true;
                    time = 0;
                    LaunchSuccess(WaveRig.transform.position, WaveRig.transform.rotation);
                }
                else
                {
                    Vector3 targetPosition = Camera.main.transform.position + Camera.main.transform.forward * 0.5f + new Vector3(0f, 0.34f, 0f);
                    Quaternion cameraRotation = Camera.main.transform.rotation;
                    Quaternion yRotation = Quaternion.Euler(0, cameraRotation.eulerAngles.y, 0);
                    //Quaternion yRotation = Quaternion.Euler(0, rotCabeza.eulerAngles.y, 0);
                    canvasFinalNivel.transform.SetPositionAndRotation(targetPosition, yRotation); //new Vector3(posCabeza.x, posCabeza.y + 0.1f, posCabeza.z + 0.5f), yRotation);
                    canvasFinalNivel.SetActive(true);
                    audioSource_win.Play();
                }
            }
            
            //Restart level after 3 errors
            /*
            if (VN.erroresNivel >= 3)
            {
                RestartScene();
            }
            */

            if (new_turn & time > waiting_time)
            {
                new_turn = false;
                ResetAssignedPositions();
                var (shelveNum, childNum, productName) = ChooseAndEmptyBox();
                Debug.Log(shelveNum.ToString() + " " + childNum.ToString() + " " + productName);
            }
        }
    }

    public (int, int, string) ChooseAndEmptyBox()
    {
        int shelveNumber = UnityEngine.Random.Range(0, game_shelves.Count);
        //Debug.Log("reponer shelve number: " + shelveNumber.ToString());
        GameObject shelve = game_shelves[shelveNumber];
        
        //Transform front = shelve.transform.GetChild(0);  //transform.Find("front");
        //Debug.Log(shelve.name);
        //Debug.Log(front.name);
        
        int childCount = shelve.transform.childCount;
        int childNumber = UnityEngine.Random.Range(0, childCount);

        Transform ChildTransform = shelve.transform.GetChild(childNumber);
        Transform productsTransform = ChildTransform.transform.Find("products");
        if (productsTransform != null)
        {
            productsTransform.gameObject.SetActive(false);
        }
        string childName = ChildTransform.gameObject.name;
        //Debug.Log("reponer child number & name: " + childNumber.ToString() + " " + childName);
        string productName = "";
        Match match = pattern.Match(childName);

        productName = match.Groups[1].Value;
        //Debug.Log($"reponer Selected product: {productName}");

        List<int> res = new List<int> {shelveNumber, childNumber };

        Transform productToPlace = products.transform.Find("Product_" + productName);

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

        //Debug.Log("reponer rot: " + product_rot);
        //Debug.Log("reponer pos: " + selectedPosition);
        //Debug.Log("reponer prod: " + productToPlace);
        productToPlace.transform.SetLocalPositionAndRotation(selectedPosition, Quaternion.Euler(product_rot.x, product_rot.y, product_rot.z));
        productToPlace.gameObject.SetActive(true);

        return (shelveNumber, childNumber, productName);
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
        //Debug.Log("reponer GetRandomAvailablePosition");
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
                Debug.LogError("No available positions.");
                throw new InvalidOperationException("No available positions.");
            }
            int randomIndex = availableIndices[UnityEngine.Random.Range(0, availableIndices.Count)];

            // Mark the position as taken
            position_fruits_assigned[randomIndex] = true;
            return (position_fruits[randomIndex], randomIndex);
        }
        else
        {
            for (int i = 0; i < position_products_assigned.Count; i++)
            {
                //Debug.Log("reponer int i " + i.ToString());
                if (!position_products_assigned[i])
                {
                    availableIndices.Add(i);
                }
            }
            if (availableIndices.Count == 0)
            {
                Debug.LogError("No available positions.");
                throw new InvalidOperationException("No available positions.");
            }
            //Debug.Log("reponer available indices " + availableIndices.Count.ToString());
            int randomIndex = availableIndices[UnityEngine.Random.Range(0, availableIndices.Count)];
            //Debug.Log("reponer pos index " + randomIndex.ToString() + " pos" + positions_products[randomIndex].ToString());
            // Mark the position as taken
            position_products_assigned[randomIndex] = true;
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
        for(int i=0; i < position_fruits_assigned.Count; i++)
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
        //Debug.Log("BOXCOLL bwwwaah");
        //GameObject cross = GameObject.Find("Nivel_erroneo");
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
        //Go through all boxes and products in front
        foreach (GameObject shelve in all_shelves)
        {
            foreach (Transform box in shelve.transform)
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
        //Go through all boxes and products in back
        foreach (GameObject shelve in all_shelves)
        {
            Transform back = shelve.transform.parent.Find("back");
            if (back != null)
            {
                foreach (Transform box in back)
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
