using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class RandomizeShelfPositions3T2 : MonoBehaviour
{
    
    public List<GameObject> YourProducts3; // Prodotti che seleziono 
    public float minDistance3;
    private List<Vector3> occupiedPositions3 = new List<Vector3>(); 
    public GameObject shelves3;
    private GameObject[] boxes3;
    public static RandomizeShelfPositions3T2 instance3T2;
    public List<GameObject> newClones3 = new List<GameObject>();

    private Vector3[] TotPositions3 = new Vector3[16] 
    {  
       // BALDA 1
        
        new Vector3(-0.74f,1.42f, 2.52f ), // 1.1
        new Vector3(-0.165f,1.42f, 2.52f ),   // 1.2
        new Vector3(0.41f,1.42f, 2.52f ), // 1.3
        new Vector3(0.985f,1.42f, 2.52f ), // 1.4
        
        
        // BALDA 2
        new Vector3(0.985f,1.07f, 2.52f ), // 2.4
        new Vector3(0.41f,1.07f, 2.52f ), // 2.3
        new Vector3(-0.165f,1.07f, 2.52f ), // 2.2
        new Vector3(-0.74f,1.07f, 2.52f ), // 2.1
        
       
       // BALDA 3
        new Vector3(-0.74f,0.789f, 2.52f ), // 3.1
       new Vector3(-0.165f,0.789f, 2.52f ),   // 3.2
        new Vector3(0.41f,0.789f, 2.52f ), // 3.3
        new Vector3(0.985f,0.789f, 2.52f ), // 3.4
     

        
        // BALDA 4
        new Vector3(0.985f, 0.33f, 2.52f ), // 4.4
        new Vector3(0.41f, 0.33f, 2.52f ), // 4.3
        new Vector3(-0.165f, 0.33f, 2.52f ),   // 4.2
        new Vector3(-0.74f, 0.33f, 2.52f ), // 4.1
        

    };
    
    public List<List<GameObject>> allLevelProductsT2 = new List<List<GameObject>>();
    public List<GameObject> Nivel1;
    public List<GameObject> Nivel2;
    public List<GameObject> Nivel3;
    public List<GameObject> Nivel4;
    public List<GameObject> Nivel5;
    public List<GameObject> Nivel6;



    Dictionary<GameObject, GameObject> Diz_randomPositions3 = new Dictionary<GameObject, GameObject>();//DIZIONARIO RANDOM
    
    public void GetProductsByLevel3T2(int level)
    {
        allLevelProductsT2.Clear();

        Debug.Log("!!!! Entrato in GetProductsByLevel3T2 con livello: " + level);
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
            //0foreach (var product in allLevelProductsT2[i])
            //{
            //    Debug.Log("Livello " + (i + 1) + " contiene: " + product.name);
            //}
        }

        if (level < 1 || level > allLevelProductsT2.Count)
        {
            Debug.Log("!!!! Chiamato GetProductsByLevel per il livello: " + level);
            Debug.Log("!!!! allLevelProducts.Count: " + allLevelProductsT2.Count);

            Debug.LogWarning("Livello non valido: " + level);

            // Imposta game_shelves a una lista vuota se il livello non è valido
            YourProducts3 = new List<GameObject>(); 
            return; 
        }

        YourProducts3 = allLevelProductsT2[level - 1]; // Aggiorna game_shelves con la lista degli scaffali per il livello selezionato
        //foreach (GameObject obj in YourProducts3)
        //{
        //    Debug.LogWarning("Oggetti nello scaffale: " + obj.name);
        //}
        UpdateShelf3();
    }
    
    public void UpdateShelf3()
    {
        // MIS PRODUCTOS
        if (YourProducts3 != null && YourProducts3.Count > 1)
        {
            // Cerca i tag BOX dentro dello scaffale
            GameObject[] boxes = shelves3.GetComponentsInChildren<Transform>()
                .Where(t => t.CompareTag("BOX"))
                .Select(t => t.gameObject)
                .ToArray();

            foreach (GameObject box in boxes)
            {
                box.SetActive(false); // Disattiva i vecchi prodotti
            }

            RandomizePositionsYourProducts3(YourProducts3); // Riempie tutte le posizioni con i prodotti selezionati
            Debug.Log("!!!! Scaffale aggiornato con i prodotti selezionati.");
        }
        
        else
        {
            Debug.Log("AAA tutto rimane uguale.");
        }
    }


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
void RandomizePositionsYourProducts3(List<GameObject> items)
    {
        List<Vector3> availablePositions3 = new List<Vector3>(TotPositions3);
        occupiedPositions3.Clear();
        Diz_randomPositions3.Clear();
        newClones3.Clear();

        Dictionary<GameObject, int> objectCounts = GenerateRandomDistribution(availablePositions3.Count, items);
        List<GameObject> productsToPlace = GenerateOrderedObjectList(objectCounts);

        // Calcola il numero di copie per ciascun prodotto per riempire tutte le posizioni
        //int baseCount = totalPositions / totalItems; // Copie base per prodotto
        //int remainder = totalPositions % totalItems; // Posizioni in più da distribuire

        // Aggiungi copie dei prodotti in base alle posizioni disponibili
        /*for (int i = 0; i < totalItems; i++)
        {
            for (int j = 0; j < baseCount; j++)
            {
                productsToPlace.Add(items[i]);
            }
            if (i < remainder)
            {
                productsToPlace.Add(items[i]);
            }
        }
        */

        // Mescola i prodotti mantenendo quelli uguali vicini
        //productsToPlace = productsToPlace.OrderBy(p => p.name).ThenBy(x => Random.value).ToList();

        Dictionary<string, int> productCloneCounts = new Dictionary<string, int>(); // Per dare un nome univoco ai cloni

        // Assegna i prodotti alle posizioni
        foreach (GameObject product3 in productsToPlace)
        {
            GameObject newProduct = Instantiate(product3);
            Vector3 newPosition = GetNextAdjacentPosition3(availablePositions3);
            Vector3 originalScale = product3.transform.localScale;

            newProduct.transform.position = newPosition;
            newProduct.transform.SetParent(shelves3.transform);
            newProduct.transform.localScale = originalScale;
            newProduct.transform.rotation = product3.transform.rotation;

            Diz_randomPositions3[newProduct] = newProduct; // Salva il GameObject intero

            newProduct.tag = "ClonedProduct";
            newProduct.SetActive(true);

            // Assegna un nome univoco al clone
            string originalName = product3.name;
            if (!productCloneCounts.ContainsKey(originalName))
            {
                productCloneCounts[originalName] = 1; // Inizializza il contatore
            }

            newProduct.name = $"{originalName} ({productCloneCounts[originalName]})"; // Rinomina il clone
            productCloneCounts[originalName]++; // Incrementa il contatore

            // Crea un clone disattivato per utilizzo futuro
            GameObject clone = Instantiate(newProduct);
            clone.transform.position = newProduct.transform.position;
            clone.transform.rotation = newProduct.transform.rotation;
            clone.transform.localScale = originalScale;
            clone.SetActive(false);
            newClones3.Add(clone);

            occupiedPositions3.Add(newPosition);
        }

        //PrintDictionary3(); // DIZIONARIO
    }

    Vector3 GetNextAdjacentPosition3(List<Vector3> availablePositions)
    {
        if (availablePositions.Count == 0)
        {
            Debug.LogWarning("!!!! Non ci sono più posizioni disponibili!");
            return Vector3.zero;
        }

        // Prendi la prima posizione e rimuovila, così restano adiacenti
        Vector3 chosenPosition = availablePositions[0];
        availablePositions.RemoveAt(0); // Rimuove la posizione usata
        return chosenPosition;
    }


    Vector3 GetRandomAvailablePosition3(List<Vector3> availablePositions)
    {
        if (availablePositions.Count == 0)
        {
            Debug.LogWarning("AAA Non ci sono più posizioni disponibili!");
            return Vector3.zero;
        }

        int randomIndex = Random.Range(0, availablePositions.Count);
        Vector3 chosenPosition = availablePositions[randomIndex];
        availablePositions.RemoveAt(randomIndex); // Rimuove la posizione usata
        return chosenPosition;
    }


    public void RestoreOriginalPositions3T2()
    {
        Debug.Log("!!!! Entrato in RestoreOriginalPositions ");
        Debug.Log($"!!!! Numero di elementi nel dizionario: {Diz_randomPositions3.Count}");


        foreach (Transform child in shelves3.transform)
        {
            if (child.name != "colliders")
            {
                Destroy(child.gameObject);
            }
        }

        List<GameObject> activeClones3 = new List<GameObject>();

        // Crea i cloni attivi a partire dai cloni disattivati
        foreach (GameObject clone3 in newClones3)
        {
            GameObject newClone3 = Instantiate(clone3);
            Vector3 originalScale = clone3.transform.localScale;

            newClone3.transform.position = clone3.transform.position;
            newClone3.transform.rotation = clone3.transform.rotation;
            newClone3.transform.localScale = originalScale;

            newClone3.SetActive(true);
            newClone3.transform.SetParent(shelves3.transform); // Aggiunge il clone come figlio dello scaffale
            newClone3.transform.localScale = originalScale;

            newClone3.name = newClone3.name.Replace("(Clone)", "").Trim();
            activeClones3.Add(newClone3); // Salva il clone attivo nella lista
            Debug.Log($" !!!! PRODOTTO Nuova posizione di {newClone3.name} (attivato): {newClone3.transform.position}");
        }
    }








    /*
    private List<List<string>> prodottiRimossiShelve3;

    public void PassaLaLista3(List<List<string>> prodottiRimossi3)
    {
        prodottiRimossiShelve3 = prodottiRimossi3; // Assegna direttamente la lista delle gerarchie
        Debug.Log("!!!! PassaLaLista3 chiamato con " + prodottiRimossiShelve3.Count + " prodotti.");

        foreach (var gerarchia3 in prodottiRimossiShelve3)
        {
            Debug.Log("!!!! LISTA PASSATA: Prodotto con gerarchia: " + string.Join(" -> ", gerarchia3));
        }
    }


    public GameObject Products;

    public void RestoreOriginalPositions3()
    { 
        Debug.Log("!!!! Entrato in RestoreOriginalPositions ");
        Debug.Log($"!!!! Numero di elementi nel dizionario: {Diz_randomPositions3.Count}");

        // Elimina i figli attuali dello scaffale
        foreach (Transform child in shelves3.transform)
        {
            if (child.name != "colliders")
            {
                Destroy(child.gameObject);
            }
        }

        // Crea una lista per i nuovi cloni attivi
        List<GameObject> activeClones3 = new List<GameObject>();

        // Crea i cloni attivi a partire dai cloni disattivati
        foreach (GameObject clone3 in newClones3)
        {
            GameObject newClone3 = Instantiate(clone3);
            Vector3 originalScale = clone3.transform.localScale;

            newClone3.transform.position = clone3.transform.position;
            newClone3.transform.rotation = clone3.transform.rotation;
            newClone3.transform.localScale = originalScale;

            newClone3.SetActive(true);
            newClone3.transform.SetParent(shelves3.transform); // Aggiunge il clone come figlio dello scaffale
            newClone3.transform.localScale = originalScale;

            newClone3.name = newClone3.name.Replace("(Clone)", "").Trim();
            activeClones3.Add(newClone3); // Salva il clone attivo nella lista
            Debug.Log($" !!!! PRODOTTO Nuova posizione di {newClone3.name} (attivato): {newClone3.transform.position}");
        }

         if (prodottiRimossiShelve3 != null && prodottiRimossiShelve3.Count > 0)
         {
            Debug.Log("!!!! PRODOTTO Inizio controllo sui prodotti attivi in 'products'.");

            // Mappa dei prodotti attivi in 'Products' per nome essenziale
            HashSet<string> activeProductNames = new HashSet<string>();

            foreach (Transform product in Products.transform)
            {
                if (product.gameObject.activeSelf)
                {
                   
                    string productName = product.name.Replace("Product_", "").Trim();
                    activeProductNames.Add(productName);
                    Debug.Log($"PRODOTTO Prodotto attivo trovato: {product.name} (nome essenziale: {productName})");
                }
            }

            // Rimuovi dalla lista `prodottiRimossiShelve3` i prodotti che non sono attivi in `Products`
            for (int i = prodottiRimossiShelve3.Count - 1; i >= 0; i--)
            {
                var gerarchia3 = prodottiRimossiShelve3[i];
                string rawProductName = gerarchia3[2].Replace("BOX_", "").Replace("box_", "").Split(' ')[0];


                Debug.Log($" PRODOTTO Controllo prodotto nella lista: {string.Join(" -> ", gerarchia3)} (nome target: {rawProductName})");

                // Se il prodotto non è attivo in 'Products', rimuovilo dalla lista
                if (!activeProductNames.Contains(rawProductName))
                {
                    Debug.Log($" PRODOTTO Rimozione dalla lista: {string.Join(" -> ", gerarchia3)} (non attivo in 'products').");
                    prodottiRimossiShelve3.RemoveAt(i);
                }
            }

                // Esegui DisattivaProdotto3 per ciascuno nella lista rimasta
                foreach (var gerarchia3 in prodottiRimossiShelve3)
                {
                Debug.Log($" PRODOTTO Eseguo DisattivaProdotto3 per: {string.Join(" -> ", gerarchia3)}");
                DisattivaProdotto3(gerarchia3);
                }
         }
        else
        {
            Debug.Log("!!!! PRODOTTO Nessun prodotto rimosso disponibile.");
        }


    }
        public void DisattivaProdotto3(List<string> gerarchia3)
        {

            Debug.Log("DisattivaProdotto3 chiamato per la gerarchia: " + string.Join(" -> ", gerarchia3));
            string boxName3 = gerarchia3[0];

            //BOX

            Transform boxTransform3 = FindBoxIgnoringClones3(boxName3);


            Debug.Log("PROVA Nome del BOX: " + boxName3);

            if (boxTransform3 == null)
            {
                Debug.LogWarning("PROVA BOX " + boxName3 + " non trovato nella scena.");
                return;
            }

            Transform currentTransform = boxTransform3;
            for (int i = 1; i < gerarchia3.Count; i++)
            {
                string currentName = gerarchia3[i];
                currentTransform = currentTransform.Find(currentName);

                if (currentTransform == null)
                {
                    Debug.LogWarning("PROVA" + currentName + " non trovato in " + currentTransform.name);
                    return;
                }
                Debug.Log("PROVA Trovato: " + currentName + " in " + currentTransform.parent.name);
            }
            currentTransform.gameObject.SetActive(false);
            Debug.Log("PROVA Prodotto disattivato: " + currentTransform.name);


        }

    private Transform FindBoxIgnoringClones3(string boxName3)
    {
        // Trova tutti gli oggetti nella scena
        GameObject[] allBoxes3 = GameObject.FindObjectsOfType<GameObject>();



        Debug.Log($"PROVA Nome del BOX pulito: {boxName3}"); // nome del BOX pulito

        foreach (GameObject box3 in allBoxes3)
        {
            // Rimuovi tutte le occorrenze di "(Clone)" dal nome del box
            string cleanBoxInScene3 = box3.name.Replace("(Clone)", "").Trim();


            Debug.Log($"PROVA Trovato BOX: {box3.name} (pulito: {cleanBoxInScene3})");

            // Confronta i nomi puliti
            if (cleanBoxInScene3 == boxName3)
            {
                Debug.Log($"PROVA Trovato BOX corrispondente: {box3.name}");
                return box3.transform; // Restituisci il primo BOX trovato
            }
        }


        Debug.LogWarning("PROVA Nessun BOX trovato che corrisponde a " + boxName3);
        return null;
    }

    public void PrintDictionary3()
    {
        if (Diz_randomPositions3.Count == 0)
        {
            Debug.Log("!!!! Dizionario random vuoto");
        }
        else
        {
            foreach (KeyValuePair<GameObject, GameObject> entry3 in Diz_randomPositions3)
            {
                GameObject savedObject3 = entry3.Value; // Ottieni il GameObject salvato

                Debug.Log("!!!! Prodotto: " + entry3.Key.name
                + " - Posizione: " + savedObject3.transform.position
                + " - Rotazione: " + savedObject3.transform.rotation.eulerAngles
                + " - Scala: " + savedObject3.transform.localScale);
            }
        }
    }*/
} 











































/*void Start()
        {
            RandomizePositions();
        }
        void RandomizePositions()
        {
            // Crea una lista delle posizioni disponibili perche e piu comodo invece di usare gli array

            List<Vector3> availablePositions = new List<Vector3>(TotPositions);
            // Posiziona i cubi nelle posizioni disponibili
            foreach (GameObject cube in cubes)
            {
                Vector3 newPosition = GetRandomAvailablePosition
                (availablePositions);
                cube.transform.position = newPosition;
                occupiedPositions.Add(newPosition); // Aggiunge la posizione  alla lista delle occupate



            }
        }
        // Ottiene una posizione casuale disponibile e la rimuove dalla lista  delle posizioni disponibili

        Vector3 GetRandomAvailablePosition(List<Vector3> availablePositions)
        {
            if (availablePositions.Count == 0)
            {
                Debug.LogWarning("Non ci sono più posizioni disponibili!");
                return Vector3.zero;
            }
            int randomIndex = Random.Range(0, availablePositions.Count);
            Vector3 chosenPosition = availablePositions[randomIndex];
            availablePositions.RemoveAt(randomIndex); // Rimuove la posizione usata

      return chosenPosition;
        }
     */



