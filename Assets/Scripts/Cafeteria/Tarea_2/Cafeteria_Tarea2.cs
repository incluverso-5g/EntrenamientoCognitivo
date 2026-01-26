using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.UIElements;

public class Cafeteria_Tarea2 : MonoBehaviour
{
    // LLamada a otros scripts:
    VariablesComunes_T2 variablesComunes;
    ListaPrefabsNiveles listaPrefabsNiveles;

    // Creación de variables para cada fase y fallos:
    public int fallos;
    public int faseActual;
    public int faseAnterior;

    // Para comprobar si hay un plato en la mesa o si hay bebidas en la barra:
    // private bool platoMesa = false;
    private bool bebidasBarra = false;

    // Variables de los bocadillos: 
    public GameObject bocadillo_M1;
    public GameObject bocadillo_M2;
    public GameObject bocadillo_M3;

    // Lista para los sitios donde dejar las comandas y objetos para ellos:
    private List<GameObject> possibleCollidersM1;
    private List<GameObject> possibleCollidersM2;
    private List<GameObject> possibleCollidersM3;
    public GameObject selectedColliderM1;
    public GameObject selectedColliderM2;
    public GameObject selectedColliderM3;

    // Materiales:
    public Material materialSelect;
    public Material materialTransparent;

    // Comprobamos si hay un collider seleccionado:
    private bool colliderIsSelected = false;

    // Define el color actual del bocadillo visible
    public Color currentBocadilloColor;

    void Start()
    {
        listaPrefabsNiveles = GetComponent<ListaPrefabsNiveles>();
        // Se comienza con cero fallos en la fase 0:
        fallos = 0;
        //faseActual = 0;
        faseAnterior = -1;

        // Extraemos la posición y rotación de la barra del otro script: 
        variablesComunes = GetComponent<VariablesComunes_T2>();

        // Búsqueda de los objetos de juego de los bocadillos:
        bocadillo_M1 = GameObject.Find("Bocadillo_M1");
        bocadillo_M2 = GameObject.Find("Bocadillo_M2");
        bocadillo_M3 = GameObject.Find("Bocadillo_M3");

        // Búsqueda de los objetos de los sitios de las comandas:
        possibleCollidersM1 = FindCollidersByName("ColisionPlatosM1");
        possibleCollidersM2 = FindCollidersByName("ColisionPlatosM2");
        possibleCollidersM3 = FindCollidersByName("ColisionPlatosM3");

        ClearLevel();
    }

    void Update()
    {
        //faseActual = variablesComunes.faseActual;

        if (faseAnterior != variablesComunes.faseActual)
        {
            //Debug.Log("Fase anterior distinta a la actual");
            bebidasBarra = false;
            colliderIsSelected = false;

            ResetPhaseColliders();
        }
        else
        {
            bebidasBarra = true;
            colliderIsSelected = true;
        }

        //Debug.Log("Fase actual: " + variablesComunes.faseActual + " Fase anterior: " + faseAnterior);

        if (variablesComunes.repeticiones < variablesComunes.repeticiones_max)
        {
            if (variablesComunes.faseActual < variablesComunes.fases_max)
            {
                //Debug.Log("Si fase actual es menor");
                if (!bebidasBarra && !colliderIsSelected) // Si estamos en fase inicial y no hay bebidas en la barra ni collider activo
                {
                    //Debug.Log("Estamos en fase inicial");
                    if (variablesComunes.currentLevel == 1)
                    {
                        ClearLevel();

                        // Vamos a poner tres bebidas en la barra:
                        //InstantiateDrinksBarra(variablesComunes.prefabCocaCola, variablesComunes.prefabCafe, variablesComunes.prefabFanta);
                        InstantiateDrinksBarra(listaPrefabsNiveles.prefabListN1);
                        
                        // Se activa únicamente un bocadillo de los tres, con un color aleatorio:
                        SetRandomBocadilloVisibility();
                        //setRandomBocadilloColor();

                        SelectRandomCollider(); //Selecciona un collider aleatorio en una mesa

                        // Se ponen las bebidas solicitadas en los bocadillos:
                        InstantiateDrinksBocadillo(variablesComunes.instantiatedDrinksBocadilloM1, bocadillo_M1, listaPrefabsNiveles.prefabListN1);
                        InstantiateDrinksBocadillo(variablesComunes.instantiatedDrinksBocadilloM2, bocadillo_M2, listaPrefabsNiveles.prefabListN1);
                        InstantiateDrinksBocadillo(variablesComunes.instantiatedDrinksBocadilloM3, bocadillo_M3, listaPrefabsNiveles.prefabListN1);

                        // La fase anterior sube a la fase en la que estamos:
                        faseAnterior = variablesComunes.faseActual;
                    }
                    else if (variablesComunes.currentLevel == 2)
                    {
                        ClearLevel();

                        List<GameObject> selectedFoodsandDrinks = variablesComunes.ShuffleListGameObject(listaPrefabsNiveles.prefabListN2);//Creo una lista seleccionando los 6 primeros objetos aleatorios
                        selectedFoodsandDrinks = selectedFoodsandDrinks.Take(6).ToList();

                        InstantiateDrinksBarra(selectedFoodsandDrinks);

                        // Se activan dos bocadillos de los tres, con un color aleatorio:
                        SetRandomBocadilloVisibility();
                        //setRandomBocadilloColor();

                        SelectRandomCollider(); 

                        // Se ponen las bebidas solicitadas en los bocadillos:
                        InstantiateDrinksBocadillo(variablesComunes.instantiatedDrinksBocadilloM1, bocadillo_M1, selectedFoodsandDrinks);
                        InstantiateDrinksBocadillo(variablesComunes.instantiatedDrinksBocadilloM2, bocadillo_M2, selectedFoodsandDrinks);
                        InstantiateDrinksBocadillo(variablesComunes.instantiatedDrinksBocadilloM3, bocadillo_M3, selectedFoodsandDrinks);

                        // La fase anterior sube a la fase en la que estamos:
                        faseAnterior = variablesComunes.faseActual;
                    }
                    else if (variablesComunes.currentLevel == 3)
                    {
                        ClearLevel();

                        List<GameObject> selectedFoodsandDrinks = variablesComunes.ShuffleListGameObject(listaPrefabsNiveles.prefabListN2);//Creo una lista seleccionando los 6 primeros objetos aleatorios
                        selectedFoodsandDrinks = selectedFoodsandDrinks.Take(6).ToList();
                        InstantiateDrinksBarra(selectedFoodsandDrinks);
                        
                        // Activar los tres bocadillos
                        SetBocadillosVisibility(true, true, true);

                        // Asignar comidas y bebidas a cada bocadillo
                        InstantiateDrinksBocadillo(variablesComunes.instantiatedDrinksBocadilloM1, bocadillo_M1, selectedFoodsandDrinks);
                        InstantiateDrinksBocadillo(variablesComunes.instantiatedDrinksBocadilloM2, bocadillo_M2, selectedFoodsandDrinks);
                        InstantiateDrinksBocadillo(variablesComunes.instantiatedDrinksBocadilloM3, bocadillo_M3, selectedFoodsandDrinks);

                        // Establecer colores aleatorios para los bocadillos
                        //setRandomBocadilloColorDif();
                        
                        // Seleccionar colliders para cada bocadillo
                        SelectRandomCollider();

                        faseAnterior = variablesComunes.faseActual;
                    }
                    else if(variablesComunes.currentLevel == 4)
                    {
                        ClearLevel();

                        //List<GameObject> selectedFoodandDrinks = variablesComunes.ShuffleListGameObject(listaPrefabsNiveles.prefabListN4);
                        List<GameObject> selectedFoodsandDrinks = GetRandomPairsPrefabList();
                        //Debug.Log("Selected foods and drinks: " + selectedFoodsandDrinks);

                        InstantiateDrinksBarra(selectedFoodsandDrinks);

                        // Activar los tres bocadillos
                        SetBocadillosVisibility(true, true, true);

                        // Asignar comidas y bebidas a cada bocadillo
                        InstantiateDrinksBocadillo(variablesComunes.instantiatedDrinksBocadilloM1, bocadillo_M1, selectedFoodsandDrinks);
                        InstantiateDrinksBocadillo(variablesComunes.instantiatedDrinksBocadilloM2, bocadillo_M2, selectedFoodsandDrinks);
                        InstantiateDrinksBocadillo(variablesComunes.instantiatedDrinksBocadilloM3, bocadillo_M3, selectedFoodsandDrinks);

                        // Establecer colores aleatorios para los bocadillos
                        //setRandomBocadilloColorDif();

                        // Seleccionar colliders para cada bocadillo
                        SelectRandomCollider();

                        faseAnterior = variablesComunes.faseActual;
                    }
                    else if (variablesComunes.currentLevel == 5)
                    {
                        ClearLevel();

                        //List<GameObject> selectedFoodandDrinks = variablesComunes.ShuffleListGameObject(listaPrefabsNiveles.prefabListN4);
                        List<GameObject> selectedFoodsandDrinks = GetRandomPairsPrefabList();
                        //Debug.Log("Selected foods and drinks: " + selectedFoodsandDrinks);

                        InstantiateDrinksBarra(selectedFoodsandDrinks);

                        // Activar los tres bocadillos
                        SetBocadillosVisibility(true, true, true);

                        // Asignar comidas y bebidas a cada bocadillo
                        InstantiateDrinksBocadillo(variablesComunes.instantiatedDrinksBocadilloM1, bocadillo_M1, selectedFoodsandDrinks);
                        InstantiateDrinksBocadillo(variablesComunes.instantiatedDrinksBocadilloM2, bocadillo_M2, selectedFoodsandDrinks);
                        InstantiateDrinksBocadillo(variablesComunes.instantiatedDrinksBocadilloM3, bocadillo_M3, selectedFoodsandDrinks);

                        // Establecer colores aleatorios para los bocadillos
                        //setRandomBocadilloColorDif();

                        // Seleccionar colliders para cada bocadillo
                        SelectRandomCollider();

                        faseAnterior = variablesComunes.faseActual;
                    }
                    else if (variablesComunes.currentLevel == 6)
                    {
                        ClearLevel();

                        //List<GameObject> selectedFoodandDrinks = variablesComunes.ShuffleListGameObject(listaPrefabsNiveles.prefabListN4);
                        
                        List<GameObject> selectedFoodsandDrinks = GetRandomPair();
                        InstantiateDrinksBarra(selectedFoodsandDrinks);

                        // Activar los tres bocadillos
                        SetBocadillosVisibility(true, true, true);

                        // Asignar comidas y bebidas a cada bocadillo
                        InstantiateDrinksBocadillo(variablesComunes.instantiatedDrinksBocadilloM1, bocadillo_M1, selectedFoodsandDrinks);
                        InstantiateDrinksBocadillo(variablesComunes.instantiatedDrinksBocadilloM2, bocadillo_M2, selectedFoodsandDrinks);
                        InstantiateDrinksBocadillo(variablesComunes.instantiatedDrinksBocadilloM3, bocadillo_M3, selectedFoodsandDrinks);

                        // Establecer colores aleatorios para los bocadillos
                        //setRandomBocadilloColorDif();

                        // Seleccionar colliders para cada bocadillo
                        SelectRandomCollider();

                        faseAnterior = variablesComunes.faseActual;
                    }
                }
            }
        }
    }

    public void ClearLevel()
    {
        DeactivatePlates(variablesComunes.instantiatedDrinksBarra);
        DeactivatePlates(variablesComunes.instantiatedDrinksBocadilloM1);
        DeactivatePlates(variablesComunes.instantiatedDrinksBocadilloM2);
        DeactivatePlates(variablesComunes.instantiatedDrinksBocadilloM3);
        variablesComunes.instantiatedDrinksBocadilloM1.Clear();
        variablesComunes.instantiatedDrinksBocadilloM2.Clear();
        variablesComunes.instantiatedDrinksBocadilloM3.Clear();
    }

    public List<GameObject> GetRandomPairsPrefabList()
    {
        List<GameObject> firstPair = listaPrefabsNiveles.similarPairsN4[Random.Range(0, listaPrefabsNiveles.similarPairsN4.Count)];
        List<List<GameObject>> remainingPairs = new List<List<GameObject>>(listaPrefabsNiveles.similarPairsN4);
        remainingPairs.Remove(firstPair);
        List<GameObject> secondPair = remainingPairs[Random.Range(0, remainingPairs.Count)];

        List<GameObject> selectedPairs = new List<GameObject>();
        selectedPairs.AddRange(firstPair);
        selectedPairs.AddRange(secondPair);

        List<GameObject> remainingPrefabs = new List<GameObject>(listaPrefabsNiveles.prefabListN4);
        foreach (var prefab in selectedPairs)
        {
            remainingPrefabs.Remove(prefab);
        }

        remainingPrefabs = variablesComunes.ShuffleListGameObject(remainingPrefabs);

        int additionalPrefabsNeeded = Mathf.Max(0, 6 - selectedPairs.Count);
        List<GameObject> randomPrefabs = remainingPrefabs.GetRange(0, Mathf.Min(additionalPrefabsNeeded, remainingPrefabs.Count));

        List<GameObject> finalList = new List<GameObject>(selectedPairs);
        finalList.AddRange(randomPrefabs);

        return variablesComunes.ShuffleListGameObject(finalList);
    }

    public List<GameObject> GetRandomPair()
    {
        List<GameObject> selectedPair = listaPrefabsNiveles.similarPairsN6[Random.Range(0, listaPrefabsNiveles.similarPairsN6.Count)];

        List<GameObject> remainingPrefabs = new List<GameObject>(listaPrefabsNiveles.prefabListN6);
        foreach (var prefab in selectedPair)
        {
            remainingPrefabs.Remove(prefab);
        }

        remainingPrefabs = variablesComunes.ShuffleListGameObject(remainingPrefabs);

        int additionalPrefabsNeeded = Mathf.Max(0, 6 - selectedPair.Count);
        List<GameObject> randomPrefabs = remainingPrefabs.GetRange(0, Mathf.Min(additionalPrefabsNeeded, remainingPrefabs.Count));

        List<GameObject> finalList = new List<GameObject>(selectedPair);
        finalList.AddRange(randomPrefabs);

        return variablesComunes.ShuffleListGameObject(finalList);
    }


    private void SelectRandomCollider()
    {
        //Reiniciamos los valores
        selectedColliderM1 = null;
        selectedColliderM2 = null;
        selectedColliderM3 = null;
        DeactivateColliders(possibleCollidersM1);
        DeactivateColliders(possibleCollidersM2);
        DeactivateColliders(possibleCollidersM3);

        // Se selecciona un collider aleatorio: 
        if (bocadillo_M1.activeSelf)
        {
            selectedColliderM1 = SelectRandomColliderColor(possibleCollidersM1, bocadillo_M1);
            Debug.Log("Selected collider: " + selectedColliderM1.name);
        }
        if (bocadillo_M2.activeSelf)
        {
            selectedColliderM2 = SelectRandomColliderColor(possibleCollidersM2, bocadillo_M2);
            Debug.Log("Selected collider: " + selectedColliderM2.name);
        }
        if (bocadillo_M3.activeSelf)
        {
            selectedColliderM3 = SelectRandomColliderColor(possibleCollidersM3, bocadillo_M3);
            Debug.Log("Selected collider: " + selectedColliderM3.name);
        }
    }

    private void InstantiateDrinksBarra(List<GameObject> prefabs)
    {
        variablesComunes.ShuffleList(variablesComunes.posBarra);

        for (int i = 0; i < Mathf.Min(prefabs.Count, variablesComunes.posBarra.Count); i++)
        {
            Vector3 prefabRotation = prefabs[i].transform.rotation.eulerAngles;

            //GameObject bebida = variablesComunes.CreateNewPlateRandomList(listaPrefabsNiveles.prefabPlatoHamburguesa, variablesComunes.posBarra[i], prefabRotation);
            GameObject bebida = variablesComunes.CreateNewPlateRandomList(prefabs[i], variablesComunes.posBarra[i], prefabRotation);
            variablesComunes.instantiatedDrinksBarra.Add(bebida);

            // Guardar posición y rotación inicial
            if (!variablesComunes.objetosPosicionesIniciales.ContainsKey(bebida))
            {
                variablesComunes.objetosPosicionesIniciales[bebida] = (variablesComunes.posBarra[i], Quaternion.Euler(prefabRotation));
            }
        }
    }

    // Variables para el contador aleatorio:
    private int randomIndex;
    private int lastRandomIndex;

    private void SetRandomBocadilloVisibility()
    {
        // Desactivar todos los bocadillos al inicio
        SetBocadillosVisibility(false, false, false);

        // Generar un nuevo índice aleatorio diferente al último:
        do
        {
            randomIndex = Random.Range(0, 3);
        }
        while (randomIndex == lastRandomIndex);

        // Actualizar el último índice para la próxima llamada
        lastRandomIndex = randomIndex;

        // Generar dos índices aleatorios diferentes, el primero se usará en fase 1, y en la fase dos los dos índices a la vez
        int secondIndex;
        do
        {
            secondIndex = Random.Range(0, 3);
        } while (secondIndex == lastRandomIndex);
        
        if(variablesComunes.currentLevel == 1)
        {
            if (randomIndex == 0)
            {
                bocadillo_M1.SetActive(true);
            }
            else if (randomIndex == 1)
            {
                bocadillo_M2.SetActive(true);
            }
            else if (randomIndex == 2)
            {
                bocadillo_M3.SetActive(true);
            }
        }
        if (variablesComunes.currentLevel == 2)
        {
            // Activar los bocadillos correspondientes
            if (lastRandomIndex == 0 || secondIndex == 0)
            {
                bocadillo_M1.SetActive(true);
            }
            if (lastRandomIndex == 1 || secondIndex == 1)
            {
                bocadillo_M2.SetActive(true);
            }
            if (lastRandomIndex == 2 || secondIndex == 2)
            {
                bocadillo_M3.SetActive(true);
            }
        }
    }

    private List<GameObject> FindCollidersByName(string nameSubstring)
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        List<GameObject> colliders = new List<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains(nameSubstring))
            {
                colliders.Add(obj);
            }
        }

        return colliders;
    }

    void ResetPhaseColliders()
    {
        ReactivateColliders(possibleCollidersM1);
        ReactivateColliders(possibleCollidersM2);
        ReactivateColliders(possibleCollidersM3);
    }

    private void ReactivateColliders(List<GameObject> colliders)
    {
        foreach (GameObject collider in colliders)
        {
            Renderer renderer = collider.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = materialSelect; // Default material
            }
            collider.SetActive(true); // Ensure active state
        }
    }



    private void DeactivateColliders(List<GameObject> possibleColliders)
    {
        foreach (GameObject collider in possibleColliders)
        {
            Renderer rendererComponent = collider.GetComponent<Renderer>();
            if (rendererComponent != null)
            {
                rendererComponent.material = materialTransparent;
            }
        }
    }

    private GameObject SelectRandomColliderColor(List<GameObject> possibleColliders, GameObject bocadilloMesa)
    {
        GameObject selectCollider = null;

        if (possibleColliders.Count > 0)
        {
            int randomIndex = Random.Range(0, possibleColliders.Count);
            selectCollider = possibleColliders[randomIndex];

            // Usar el color del bocadillo visible
            Color bocadilloColor = bocadilloMesa.GetComponent<Renderer>().material.color;


            foreach (GameObject collider in possibleColliders)
            {
                Renderer rendererComponent = collider.GetComponent<Renderer>();
                if (rendererComponent != null)
                {
                    if (collider == selectCollider)
                    {
                        // Aplicar el color del bocadillo al collider
                        rendererComponent.material.color = bocadilloColor;
                    }
                    else
                    {
                        rendererComponent.material = materialTransparent;
                    }
                }
            }
        }

        return selectCollider;
    }

    private void setRandomBocadilloColor()
    {
        // Lista de colores disponibles
        Color[] colors = { Color.red, Color.green, Color.blue };

        // Seleccionar dos colores distintos
        List<Color> availableColors = colors.ToList();
        Color firstColor = availableColors[Random.Range(0, availableColors.Count)];
        availableColors.Remove(firstColor);
        Color secondColor = availableColors[Random.Range(0, availableColors.Count)];

        // Asignar colores a los bocadillos visibles
        bool firstColorAssigned = false;
        currentBocadilloColor = firstColor;

        if (bocadillo_M1.activeSelf)
        {
            bocadillo_M1.GetComponent<Renderer>().material.color = firstColor;
            firstColorAssigned = true;
        }

        if (bocadillo_M2.activeSelf)
        {
            bocadillo_M2.GetComponent<Renderer>().material.color = firstColorAssigned ? secondColor : firstColor;
            firstColorAssigned = true;
        }

        if (bocadillo_M3.activeSelf)
        {
            bocadillo_M3.GetComponent<Renderer>().material.color = firstColorAssigned ? secondColor : firstColor;
        }
    }

    private void setRandomBocadilloColorDif()
    {
        // Lista de colores disponibles
        Color[] colores = { Color.red, Color.green, Color.blue };

        // Barajar los colores para asignar aleatoriamente
        List<Color> shuffledColors = colores.OrderBy(c => Random.value).ToList();

        // Asignar colores únicos a cada bocadillo
        if (bocadillo_M1 != null)
        {
            bocadillo_M1.GetComponent<Renderer>().material.color = shuffledColors[0];
        }
        if (bocadillo_M2 != null)
        {
            bocadillo_M2.GetComponent<Renderer>().material.color = shuffledColors[1];
        }
        if (bocadillo_M3 != null)
        {
            bocadillo_M3.GetComponent<Renderer>().material.color = shuffledColors[2];
        }
    }
    
    private void SetBocadillosVisibility(bool m1, bool m2, bool m3)
    {
        bocadillo_M1.SetActive(m1);
        bocadillo_M2.SetActive(m2);
        bocadillo_M3.SetActive(m3);
    }

    private void InstantiateDrinksBocadillo(List<GameObject> instantiadedMesa, GameObject bocadillo, List<GameObject> prefabs)
    {
        if (variablesComunes.currentLevel <= 4)
        {
            int randomIndex = Random.Range(0, prefabs.Count);

            GameObject bebidaInstanciada = variablesComunes.CreateNewPlateRandomList(prefabs[randomIndex], Vector3.zero, Vector3.zero);
            //GameObject bebidaInstanciada = variablesComunes.CreateNewPlateRandomList(listaPrefabsNiveles.prefabPlatoHamburguesa, Vector3.zero, Vector3.zero);
            SetupFoodOrDrinkL5(bebidaInstanciada, bocadillo, Vector3.zero);
            instantiadedMesa.Add(bebidaInstanciada);

        }
        else if (variablesComunes.currentLevel >= 5)
        {
            int firstIndex = Random.Range(0, prefabs.Count);
            int secondIndex;

            do
            {
                secondIndex = Random.Range(0, prefabs.Count);
            } while (secondIndex == firstIndex);

            // Instantiate the first item
            GameObject firstItem = variablesComunes.CreateNewPlateRandomList(prefabs[firstIndex], Vector3.zero, Vector3.zero);
            //GameObject firstItem = variablesComunes.CreateNewPlateRandomList(listaPrefabsNiveles.prefabPlatoPizza, Vector3.zero, Vector3.zero);
            SetupFoodOrDrinkL5(firstItem, bocadillo, new Vector3(-1.5f, 0, 0)); // Izquierda

            // Instantiate the second item
            GameObject secondItem = variablesComunes.CreateNewPlateRandomList(prefabs[secondIndex], Vector3.zero, Vector3.zero);
            //GameObject secondItem = variablesComunes.CreateNewPlateRandomList(listaPrefabsNiveles.prefabPlatoPizza3, Vector3.zero, Vector3.zero);
            SetupFoodOrDrinkL5(secondItem, bocadillo, new Vector3(1.5f, 0, 0)); // Derecha

            // Add both items to the instantiated list
            instantiadedMesa.Add(firstItem);
            instantiadedMesa.Add(secondItem);
        }
    }

    

    private void SetupFoodOrDrinkL5(GameObject item, GameObject bocadillo, Vector3 localPosition)
    {
        if (item == null) return;

        Rigidbody rigidBodyDrink = item.GetComponent<Rigidbody>();
        if (rigidBodyDrink != null)
            rigidBodyDrink.isKinematic = true;

        item.transform.SetParent(bocadillo.transform, false);

        Vector3 parentScale = bocadillo.transform.lossyScale;
        item.transform.localScale = new Vector3(1 / parentScale.x, 1 / parentScale.y, 1 / parentScale.z);

        item.transform.localPosition = localPosition;
        item.transform.localRotation = Quaternion.identity;

        if (item.name.Contains("Burger"))
        {
            item.transform.localRotation = Quaternion.Euler(90, 0, 0);
            item.transform.localPosition += new Vector3(0, -0.1f, -0.5f);
        }
        else if (item.name.Contains("Taza_Cafe"))
        {
            item.transform.localScale *= 3;
            item.transform.localPosition += new Vector3(0, -0.1f, -0.5f);
            item.transform.localRotation = Quaternion.Euler(90, 0, 0);
        }
        else if (item.name.Contains("cola") || item.name.Contains("orange"))
        {
            item.transform.localRotation = Quaternion.Euler(90, 0, 0);
            item.transform.localPosition += new Vector3(0, 0.05f, -0.8f);
        }
        else if (item.name.Contains("Chicken"))
        {
            item.transform.localRotation = Quaternion.Euler(-180, 0, 0);
            item.transform.localPosition += new Vector3(0, 0.02f, -0.1f);
            item.transform.localScale *= 2;
        }
        else if (item.name.Contains("Tarta") || item.name.Contains("Toast") || item.name.Contains("Burger"))
        {
            item.transform.localRotation = Quaternion.Euler(-180, 0, 0);
            item.transform.localPosition += new Vector3(0, 0.0f, -0.1f);
        }
        else if (item.name.Contains("Plate_Pizza"))
        {
            item.transform.localRotation = Quaternion.Euler(-180, 0, 0);
            item.transform.localPosition += new Vector3(0, 0.0f, -0.1f);
            item.transform.localScale /= 1.5f;
        }
        else if (item.name.Contains("Plate2_Pizza") || item.name.Contains("Plate3_Pizza") || item.name.Contains("Plate4_Pizza"))
        {
            // Adjust position relative to the passed localPosition
            item.transform.localRotation = Quaternion.Euler(-180, 0, 0);
            item.transform.localPosition += new Vector3(-0.2f, 0, -0.06f); 
            item.transform.localScale = new Vector3(1 / (parentScale.x * 180), 1 / (parentScale.y * 180), 1 / (parentScale.z * 180));
        }
        else if (item.name.Contains("Zumo"))
        {
            item.transform.localScale = new Vector3(1 / (parentScale.x * 1.5f), 1 / (parentScale.y * 1.5f), 1 / (parentScale.z * 1.5f));
            item.transform.localRotation = Quaternion.Euler(90, 0, 0);
            item.transform.localPosition += new Vector3(0, 0.05f, -0.6f);
        }
        else if (item.name.Contains("Pasta"))
        {
            item.transform.localRotation = Quaternion.Euler(180, 0, 0);
            item.transform.localPosition += new Vector3(0, -0.218f, -0.085f);
        }
        else if (item.name.Contains("water_b3"))
        {
            item.transform.localRotation = Quaternion.Euler(0, 0, -180);
            item.transform.localPosition += new Vector3(0, -0.1f, -0.7f);
            item.transform.localScale /= 100f;
        }
        else if (item.name.Contains("Cerveza"))
        {
            item.transform.localRotation = Quaternion.Euler(90, 0, 0);
            item.transform.localPosition += new Vector3(0, 0.05f, -0.8f);
            item.transform.localScale *= 1.5f;
        }
        else if (item.name.Contains("fiji"))
        {
            item.transform.localRotation = Quaternion.Euler(90, 0, 0);
            item.transform.localPosition += new Vector3(0, 0.05f, -0.8f);
            item.transform.localScale *= 0.60f;
        }
        else
        {
            item.transform.localPosition += new Vector3(0, 0.05f, -0.6f);
        }

        // Desactivar todos los colliders del objeto para que no se pueda agarrar
        Collider[] colliders = item.GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }
    }

    private void DeactivatePlates(List<GameObject> instantiatedPlates)
    {
        foreach (GameObject plate in instantiatedPlates)
        {
            if (plate != null)
            {
                plate.SetActive(false);
            }
        }
        instantiatedPlates.Clear();
    }
}

/*
 private void SetupFoodOrDrink(GameObject bebidaInstanciada, GameObject bocadillo)
    {
        Rigidbody rigidBodyDrink = bebidaInstanciada.GetComponent<Rigidbody>();
        rigidBodyDrink.isKinematic = true;

        bebidaInstanciada.transform.SetParent(bocadillo.transform, false);

        Vector3 parentScale = bocadillo.transform.lossyScale;
        bebidaInstanciada.transform.localScale = new Vector3(1 / parentScale.x, 1 / parentScale.y, 1 / parentScale.z);

        if (bebidaInstanciada.name.Contains("Taza_Cafe"))
        {
            bebidaInstanciada.transform.localScale = bebidaInstanciada.transform.localScale * 3;
            bebidaInstanciada.transform.localPosition = new Vector3(0, 0, -0.2f);
            bebidaInstanciada.transform.localRotation = Quaternion.Euler(90, 0, 0);
        }
        else
        {
            bebidaInstanciada.transform.localPosition = new Vector3(0, 0, -0.75f);
        }

        if (bebidaInstanciada.name.Contains("cola") || bebidaInstanciada.name.Contains("orange") || bebidaInstanciada.name.Contains("Cerveza"))
        {
            bebidaInstanciada.transform.localRotation = Quaternion.Euler(90, 0, 0);
        }
        if (bebidaInstanciada.name.Contains("Chicken"))
        {
            bebidaInstanciada.transform.localRotation = Quaternion.Euler(-180, 0, 0);
            bebidaInstanciada.transform.localPosition = new Vector3(0, 0, -0.1f);
            bebidaInstanciada.transform.localScale = bebidaInstanciada.transform.localScale * 2;
        }
        if (bebidaInstanciada.name.Contains("Plate_Pizza") || bebidaInstanciada.name.Contains("Tarta") || bebidaInstanciada.name.Contains("Toast") || bebidaInstanciada.name.Contains("Burger"))
        {
            bebidaInstanciada.transform.localRotation = Quaternion.Euler(-180, 0, 0);
            bebidaInstanciada.transform.localPosition = new Vector3(0, 0, -0.1f);
        }

        if (bebidaInstanciada.name.Contains("Plate2_Pizza"))
        {
            bebidaInstanciada.transform.localScale = new Vector3(1 / (parentScale.x * 160), 1 / (parentScale.y * 160), 1 / (parentScale.z * 160)); bebidaInstanciada.transform.localRotation = Quaternion.Euler(-180, 0, 0);
            bebidaInstanciada.transform.localPosition = new Vector3(0, 0, -0.06f);
        }

        if (bebidaInstanciada.name.Contains("Zumo"))
        {
            bebidaInstanciada.transform.localScale = new Vector3(1 / (parentScale.x * (1.5f)), 1 / (parentScale.y * (1.5f)), 1 / (parentScale.z * (1.5f))); bebidaInstanciada.transform.localRotation = Quaternion.Euler(-180, 0, 0);
            bebidaInstanciada.transform.localRotation = Quaternion.Euler(90, 0, 0);
            //bebidaInstanciada.transform.localScale = new Vector3(1 / (parentScale.x * (1.1f)), 1 / (parentScale.y * (1.1f)), 1 / (parentScale.z * (1.1f))); bebidaInstanciada.transform.localRotation = Quaternion.Euler(-180, 0, 0);
            // bebidaInstanciada.transform.localPosition = new Vector3(0, 0, -0.1f);
        }

        if (bebidaInstanciada.name.Contains("Pasta"))
        {
            bebidaInstanciada.transform.localRotation = Quaternion.Euler(180, 0, 0);
            bebidaInstanciada.transform.localPosition = new Vector3(0, -0.118f, -0.085f);
        }
    }
*/
