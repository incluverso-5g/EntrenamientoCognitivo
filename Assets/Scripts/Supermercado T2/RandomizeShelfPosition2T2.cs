using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


public class RandomizeShelfPositions2T2 : MonoBehaviour
{
    public List<GameObject> YourProducts2T2; // Prodotti che seleziono 
    public float minDistance;
    private List<Vector3> occupiedPositions = new List<Vector3>(); 
    public GameObject shelves;
    private GameObject[] boxes;
    public static RandomizeShelfPositions2T2 instance2T2;
    public List<GameObject> newClones = new List<GameObject>();
    private Vector3[] TotPositions = new Vector3[32] 
    {  
       // BALDA 1
        new Vector3(1.56f, 1.384f, 1.761f ), // 1.1
        new Vector3(1.56f,1.384f, 1.1985f ),   // 1.2
        new Vector3(1.56f, 1.384f, 0.636f ), // 1.3
        new Vector3(1.56f, 1.384f, 0.0735f ), // 1.4
        new Vector3(1.56f, 1.384f, -0.489f ), // 1.5
        new Vector3(1.56f, 1.384f, -1.0515f), // 1.6
        new Vector3(1.56f,1.384f, -1.614f), // 1.7
        new Vector3(1.56f, 1.384f, -2.1765f), // 1.8
        
       // BALDA 2
       new Vector3(1.56f, 1.064f, -2.1765f), // 2.8
       new Vector3(1.56f,1.064f, -1.614f), // 2.7
       new Vector3(1.56f, 1.064f, -1.0515f), // 2.6
       new Vector3(1.56f, 1.064f, -0.489f ), // 2.5
       new Vector3(1.56f, 1.064f, 0.0735f ), // 2.4
       new Vector3(1.56f,1.064f, 0.636f ), // 2.3
       new Vector3(1.56f,1.064f, 1.1985f ),// 2.2
       new Vector3(1.56f, 1.064f, 1.761f ), // 2.1
       
       // BALDA 3
        new Vector3(1.56f, 0.74f, 1.761f ), // 3.1
        new Vector3(1.56f,0.74f, 1.1985f ),   // 3.2
        new Vector3(1.56f,0.74f, 0.636f ), // 3.3
        new Vector3(1.56f, 0.74f, 0.0735f ), // 3.4
        new Vector3(1.56f, 0.74f, -0.489f ), // 3.5
        new Vector3(1.56f, 0.74f, -1.0515f), // 3.6
        new Vector3(1.56f,0.74f, -1.614f), // 3.7
        new Vector3(1.56f, 0.74f, -2.1765f), // 3.8
        
        // BALDA 4
        new Vector3(1.56f, 0.324f, -2.1765f), // 4.8
        new Vector3(1.56f,0.324f, -1.614f), // 4.7
        new Vector3(1.56f, 0.324f, -1.0515f), // 4.6
        new Vector3(1.56f, 0.324f, -0.489f ), // 4.5
        new Vector3(1.56f, 0.324f, 0.0735f ), // 4.4
        new Vector3(1.56f, 0.324f, 0.636f ), // 4.3
        new Vector3(1.56f,0.324f, 1.1985f ),   // 4.2
        new Vector3(1.56f, 0.324f, 1.761f ), // 4.1

    };

   
    public List<List<GameObject>> allLevelProductsT2 = new List<List<GameObject>>();
    public List<GameObject> Nivel1;
    public List<GameObject> Nivel2;
    public List<GameObject> Nivel3;
    public List<GameObject> Nivel4;
    public List<GameObject> Nivel5;
    public List<GameObject> Nivel6;


    Dictionary<GameObject, GameObject> Diz_randomPositions = new Dictionary<GameObject, GameObject>();//DIZIONARIO RANDOM


    public void GetProductsByLevel2T2(int level)
    {
        allLevelProductsT2.Clear();

        if (Nivel1 != null) allLevelProductsT2.Add(Nivel1);
        if (Nivel2 != null) allLevelProductsT2.Add(Nivel2);
        if (Nivel3 != null) allLevelProductsT2.Add(Nivel3);
        if (Nivel4 != null) allLevelProductsT2.Add(Nivel4);
        if (Nivel5 != null) allLevelProductsT2.Add(Nivel5);
        if (Nivel6 != null) allLevelProductsT2.Add(Nivel6);


        Debug.Log("!!!! allLevelProducts ha " + allLevelProductsT2.Count + " livelli.");

        for (int i = 0; i < allLevelProductsT2.Count; i++)
        {
            Debug.Log("Livello " + (i + 1) + " ha " + allLevelProductsT2[i].Count + " prodotti.");
            foreach (var product in allLevelProductsT2[i])
            {
                Debug.Log("Livello " + (i + 1) + " contiene: " + product.name);
            }
        }

        if (level < 1 || level > allLevelProductsT2.Count)
        {
            Debug.Log("!!!! Chiamato GetProductsByLevel per il livello: " + level);
            Debug.Log("!!!! allLevelProducts.Count: " + allLevelProductsT2.Count);

            Debug.LogWarning("Livello non valido: " + level);

            // Imposta game_shelves a una lista vuota se il livello non è valido
            YourProducts2T2 = new List<GameObject>();
            return;
        }

        YourProducts2T2 = allLevelProductsT2[level - 1]; // Aggiorna game_shelves con la lista degli scaffali per il livello selezionato
        foreach (GameObject obj in YourProducts2T2)
        {
            Debug.LogWarning("Oggetti nello scaffale: " + obj.name);
        }
        UpdateShelf();
    }


    void Start()
    {
       
    }

    public void UpdateShelf()
    {
        // MIS PRODUCTOS
        if (YourProducts2T2 != null && YourProducts2T2.Count > 1)
        {
            // Cerca i tag BOX dentro dello scaffale
            GameObject[] boxes = shelves.GetComponentsInChildren<Transform>()
                .Where(t => t.CompareTag("BOX"))
                .Select(t => t.gameObject)
                .ToArray();

            foreach (GameObject box in boxes)
            {
                box.SetActive(false); // Disattiva i vecchi prodotti
            }

            RandomizePositionsYourProducts(YourProducts2T2); // Riempie tutte le posizioni con i prodotti selezionati
            Debug.Log("AAA Scaffale aggiornato con i prodotti selezionati.");
        }
       
        else
        {
            //PrintOriginalPositionDictionary();
            Debug.Log("AAA tutto rimane uguale.");
        }
    }

    private Dictionary<GameObject, Vector3> originalScales = new Dictionary<GameObject, Vector3>();

    private Dictionary<GameObject, int> GenerateRandomDistribution(int totalSlots, List<GameObject> uniqueObjects)
    {
        int J = uniqueObjects.Count;
        Dictionary<GameObject, int> distribution = new Dictionary<GameObject, int>();

        // Generate J-1 random partition points (between 1 and totalSlots-1)
        List<int> partitionPoints = new List<int>();
        for (int i = 0; i < J - 1; i++)
        {
            partitionPoints.Add(Random.Range(1, totalSlots));
        }
        partitionPoints.Sort(); // Sort to create non-overlapping sections

        // Convert partition points into segment sizes
        List<int> repetitions = new List<int>();
        int prev = 0;
        foreach (int point in partitionPoints)
        {
            repetitions.Add(point - prev);
            prev = point;
        }
        repetitions.Add(totalSlots - prev); // Add the final segment

        // Assign repetitions to unique objects
        for (int i = 0; i < J; i++)
        {
            distribution[uniqueObjects[i]] = repetitions[i];
        }

        return distribution;
    }

    private List<GameObject> GenerateOrderedObjectList(Dictionary<GameObject, int> objectCounts)
    {
        // Step 1: Shuffle the unique objects
        List<GameObject> shuffledObjects = new List<GameObject>(objectCounts.Keys);
        Shuffle(shuffledObjects);

        // Step 2: Reconstruct the ordered list with copies grouped
        List<GameObject> orderedObjects = new List<GameObject>();
        foreach (GameObject obj in shuffledObjects)
        {
            int count = objectCounts[obj];
            for (int i = 0; i < count; i++)
            {
                orderedObjects.Add(obj);
            }
        }
        return orderedObjects;
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    // MIS PRODUCTOS
    void RandomizePositionsYourProducts(List<GameObject> items2)
    {
        List<Vector3> availablePositions2 = new List<Vector3>(TotPositions);
        occupiedPositions.Clear();
        Diz_randomPositions.Clear();

        Dictionary<GameObject, int> objectCounts = GenerateRandomDistribution(availablePositions2.Count, items2);
        List<GameObject> productsToPlace2 = GenerateOrderedObjectList(objectCounts);

        // Calcola il numero di copie per ciascun prodotto per riempire tutte le posizioni
        //int baseCount2 = totalPositions2 / totalItems2; // Copie base per prodotto
        //int remainder2 = totalPositions2 % totalItems2; // Posizioni in più da distribuire

        // Aggiungi copie dei prodotti in base alle posizioni disponibili
        /*
        for (int i = 0; i < totalItems2; i++)
        {
            for (int j = 0; j < baseCount2; j++)
            {
                productsToPlace2.Add(items2[i]);
            }
            if (i < remainder2)
            {
                productsToPlace2.Add(items2[i]);
            }
        }
        */

        //Dictionary<GameObject, Vector3> Diz_cloneScales = new Dictionary<GameObject, Vector3>();

        // Mescola i prodotti mantenendo quelli uguali vicini
        //productsToPlace2 = productsToPlace2.OrderBy(p => p.name).ThenBy(x => Random.value).ToList();

        Dictionary<string, int> productCloneCounts2 = new Dictionary<string, int>(); // Per dare un nome univoco ai cloni

        // Assegna i prodotti alle posizioni
        foreach (GameObject product2 in productsToPlace2)
        {
            GameObject newProduct2 = Instantiate(product2);
            Vector3 newPosition2 = GetNextAdjacentPosition2(availablePositions2);
            Vector3 originalScale2 = product2.transform.localScale;

            newProduct2.transform.position = newPosition2;
            newProduct2.transform.SetParent(shelves.transform);
            newProduct2.transform.localScale = originalScale2;
            newProduct2.transform.rotation = product2.transform.rotation;


            if (!originalScales.ContainsKey(newProduct2))
            {
                originalScales[newProduct2] = originalScale2;
            }

            Diz_randomPositions[newProduct2] = newProduct2; // Salva il GameObject intero

            newProduct2.tag = "ClonedProduct";
            newProduct2.SetActive(true);

            // Assegna un nome univoco al clone
            string originalName2 = product2.name;
            if (!productCloneCounts2.ContainsKey(originalName2))
            {
                productCloneCounts2[originalName2] = 1; // Inizializza il contatore
            }

            newProduct2.name = $"{originalName2} ({productCloneCounts2[originalName2]})"; // Rinomina il clone
            productCloneCounts2[originalName2]++; // Incrementa il contatore

            // Crea un clone disattivato per utilizzo futuro
            GameObject clone = Instantiate(newProduct2);
            clone.transform.position = newProduct2.transform.position;
            clone.transform.rotation = newProduct2.transform.rotation;
            clone.transform.localScale = originalScale2;
            clone.SetActive(false);
            newClones.Add(clone);

            occupiedPositions.Add(newPosition2);
        }

        //PrintDictionary(); // DIZIONARIO
    }

    Vector3 GetNextAdjacentPosition2(List<Vector3> availablePositions2)
    {
        if (availablePositions2.Count == 0)
        {
            Debug.LogWarning("!!!! Non ci sono più posizioni disponibili!");
            return Vector3.zero;
        }

        // Prendi la prima posizione e rimuovila, così restano adiacenti
        Vector3 chosenPosition2 = availablePositions2[0];
        availablePositions2.RemoveAt(0); // Rimuove la posizione usata
        return chosenPosition2;
    }


    Vector3 GetRandomAvailablePosition(List<Vector3> availablePositions)
    {
        if (availablePositions.Count == 0)
        {
            Debug.LogWarning("!!!! Non ci sono più posizioni disponibili!");
            return Vector3.zero;
        }

        int randomIndex = Random.Range(0, availablePositions.Count);
        Vector3 chosenPosition = availablePositions[randomIndex];
        availablePositions.RemoveAt(randomIndex); // Rimuove la posizione usata
        return chosenPosition;
    }

    public void RestoreOriginalPositions2T2()
    {
        Debug.Log("!!!! Entrato in RestoreOriginalPositions ");
        Debug.Log($"!!!! Numero di elementi nel dizionario: {Diz_randomPositions.Count}");

        foreach (Transform child in shelves.transform)
        {
            if (child.name != "colliders")
            {
                Destroy(child.gameObject);
            }
        }

        List<GameObject> activeClones2 = new List<GameObject>();

        // Crea i cloni attivi a partire dai cloni disattivati
        foreach (GameObject clone2 in newClones)
        {
            GameObject newClone2 = Instantiate(clone2);
            Vector3 originalScale = clone2.transform.localScale;

            newClone2.transform.position = clone2.transform.position;
            newClone2.transform.rotation = clone2.transform.rotation;
            newClone2.transform.localScale = originalScale;

            newClone2.SetActive(true);
            newClone2.transform.SetParent(shelves.transform); // Aggiunge il clone come figlio dello scaffale
            newClone2.transform.localScale = originalScale;

            newClone2.name = newClone2.name.Replace("(Clone)", "").Trim();
            activeClones2.Add(newClone2); // Salva il clone attivo nella lista
            Debug.Log($" !!!! PRODOTTO Nuova posizione di {newClone2.name} (attivato): {newClone2.transform.position}");
        }
    }


    /*
    public void RestoreOriginalPositions2T2()
    {
        Debug.Log("!!!! Entrato in RestoreOriginalPositions ");
        Debug.Log($"!!!! Numero di elementi nel dizionario: {Diz_randomPositions.Count}");
       

        // Elimina i figli attuali dello scaffale
        foreach (Transform child in shelves.transform)
        {
            if (child.name != "colliders")
            {
                Destroy(child.gameObject);
            }
        }

        List<GameObject> activeClones2 = new List<GameObject>();

        // Crea i cloni attivi a partire dai cloni disattivati
        foreach (GameObject clone2 in newClones)
        {
            GameObject newClone2 = Instantiate(clone2);
            //Vector3 originalScale = clone2.transform.localScale;

            newClone2.transform.position = clone2.transform.position;
            newClone2.transform.rotation = clone2.transform.rotation;
            newClone2.transform.localScale = clone2.transform.localScale;

            newClone2.SetActive(true);
            newClone2.transform.SetParent(shelves.transform); // Aggiunge il clone come figlio dello scaffale
            newClone2.name = newClone2.name.Replace("(Clone)", "").Trim();
            activeClones2.Add(newClone2); // Salva il clone attivo nella lista

            Debug.Log($" !!!! PRODOTTO Nuova posizione di {newClone2.name} (attivato): {newClone2.transform.position}");
        }
    }
    */
}














































