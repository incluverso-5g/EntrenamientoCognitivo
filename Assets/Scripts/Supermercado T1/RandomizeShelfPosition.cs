using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomizeShelfPositions : MonoBehaviour
{
    public GameObject[] products; // Prodotti della scaffale che posso selezionare per cambiarli
    public List<GameObject> YourProducts; // Prodotti che seleziono nuovi
    public float minDistance;
    private List<Vector3> occupiedPositions = new List<Vector3>(); // Lista delle posizioni occupate
    public GameObject shelves;
    private GameObject[] boxes;

    private Vector3[] TotPositions = new Vector3[32] // corretto il nome dell'array da "Tot Positions" a "TotPositions" perché gli spazi non sono permessi nei nomi delle variabili
    {  
       // BALDA 1
        
        new Vector3(7.13f, 1.5f, -1.6f ), // 1.1
        new Vector3(6.55f, 1.5f, -1.6f),   // 1.2
        new Vector3(5.97f, 1.5f, -1.6f), // 1.3
        new Vector3(5.39f, 1.5f, -1.6f), // 1.4
        new Vector3(4.81f, 1.5f, -1.6f), // 1.5
        new Vector3(4.23f, 1.5f, -1.6f), // 1.6
        new Vector3(3.65f, 1.5f, -1.6f), // 1.7
        new Vector3(3.07f, 1.5f, -1.6f), // 1.8
        
        // BALDA 2
        new Vector3(7.13f, 1.13f, -1.6f),  // 2.1
        new Vector3(6.55f, 1.13f, -1.6f),   //  2.2
        new Vector3(5.97f, 1.13f, -1.6f), // 2.3
        new Vector3(5.39f, 1.13f, -1.6f), // 2.4
        new Vector3(4.81f, 1.13f, -1.6f), // 2.5
        new Vector3(4.23f, 1.13f, -1.6f), // 2.6
        new Vector3(3.65f, 1.13f, -1.6f), // 2.7
        new Vector3(3.07f, 1.13f, -1.6f), // 2.8 

        
       // BALDA 3
        new Vector3(7.13f,0.82f, -1.6f),  // 3.1
        new Vector3(6.55f,0.82f, -1.6f),   //  3.2
        new Vector3(5.97f,0.82f, -1.6f), // 3.3
        new Vector3(5.39f,0.82f, -1.6f), // 3.4
        new Vector3(4.81f,0.82f, -1.6f), // 3.5
        new Vector3(4.23f,0.82f, -1.6f), // 3.6
        new Vector3(3.65f,0.82f, -1.6f), // 3.7
        new Vector3(3.07f,0.82f, -1.6f), // 3.8 

        
        // BALDA 4
        new Vector3(7.13f, 0.41f, -1.6f),  // 4.1
        new Vector3(6.55f, 0.41f, -1.6f),   //  4.2
        new Vector3(5.97f, 0.41f, -1.6f), // 4.3
        new Vector3(5.39f, 0.41f, -1.6f), // 4.4
        new Vector3(4.81f,0.41f, -1.6f), // 4.5
        new Vector3(4.23f,0.41f, -1.6f), // 4.6
        new Vector3(3.65f,0.41f, -1.6f), // 4.7
        new Vector3(3.07f, 0.41f, -1.6f) // 4.8
       
       
    };

    void Awake()
    {
        UpdateShelf();
    }

    public void UpdateShelf()
    {
        // MIS PRODUCTOS
        if (YourProducts != null && YourProducts.Count > 1)
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

            RandomizePositionsYourProducts(YourProducts); // Riempie tutte le posizioni con i prodotti selezionati
            Debug.Log("AAA Scaffale aggiornato con i prodotti selezionati.");
        }
        // PRODUCTOS ORIGINALES
        else if (products != null && products.Length > 1)
        {
            RandomizePositions(products); // Se non ci sono prodotti selezionati, randomizza i prodotti esistenti nello scaffale
            Debug.Log("AAA Prodotti esistenti randomizzati.");
        }
        else
        {
            Debug.Log("AAA tutto rimane uguale.");
        }
    }
    // PRODUCTOS ORIGINALES
    void RandomizePositions(IEnumerable<GameObject> items)
    {
        List<Vector3> availablePositions = new List<Vector3>(TotPositions); // lista delle posizioni disponibili

        foreach (GameObject product in products)
        {
            Vector3 newPosition = GetRandomAvailablePosition(availablePositions);
            product.transform.position = newPosition;
            // Debug.Log("AAA Posizione aggiornata per " + product.name + ": " + newPosition);
            occupiedPositions.Add(newPosition); // Aggiunge la posizione alla lista delle occupate
        }
    }
    // MIS PRODUCTOS
    void RandomizePositionsYourProducts(List<GameObject> items)
    {
        List<Vector3> availablePositions = new List<Vector3>(TotPositions);
        occupiedPositions.Clear();

        int totalPositions = availablePositions.Count;
        int totalItems = YourProducts.Count;


        // DIVISIONES DE PRODUCTOS
        int baseCount = totalPositions / totalItems; // Quante istanze per prodotto
        int remainder = totalPositions % totalItems; // Rimanente da distribuire

        List<GameObject> productsToPlace = new List<GameObject>();

        
        // AÑADE COPIA DE PRODUCTOS EN LA LISTA
        for (int i = 0; i < totalItems; i++)
        {
            for (int j = 0; j < baseCount; j++)
            {
                productsToPlace.Add(items[i]);
            }
            // Aggiunge il resto in modo equo
            if (i < remainder)
            {
                productsToPlace.Add(items[i]);
            }
        }

       
        productsToPlace = productsToPlace.OrderBy(x => Random.value).ToList();  // distribuzione casuale




        // PONE LOS PRODUCTOS
        foreach (GameObject product in productsToPlace)
        {
                GameObject newProduct = Instantiate(product); // Crea una nuova istanza del prodotto
                Vector3 newPosition = GetRandomAvailablePosition(availablePositions);
                newProduct.transform.localPosition = newPosition;

            newProduct.transform.SetParent(shelves.transform);


            newProduct.transform.localScale = shelves.transform.localScale; 
            newProduct.transform.rotation = shelves.transform.rotation;
            newProduct.SetActive(true);

            occupiedPositions.Add(newPosition);
            
        }
    }

    Vector3 GetRandomAvailablePosition(List<Vector3> availablePositions)
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



