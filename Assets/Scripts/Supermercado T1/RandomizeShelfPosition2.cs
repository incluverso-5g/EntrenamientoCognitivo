using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;



public class RandomizeShelfPositions2 : MonoBehaviour
{
	public List<GameObject> YourProducts; 
	public float minDistance;
	private List<Vector3> occupiedPositions = new List<Vector3>(); 
	public GameObject shelves;
	private GameObject[] boxes;
	public static RandomizeShelfPositions2 instance;
	//private VariablesNiveles VN;

   // public Transform currentDynamicChild;


	public List<GameObject> newClones = new List<GameObject>();
	private Vector3[] TotPositions = new Vector3[32] 
	{  
	  // BALDA 1
		new Vector3(1.56f, 1.394f, 1.761f ), // 1.1
		new Vector3(1.56f,1.394f, 1.1985f ),   // 1.2
		new Vector3(1.56f, 1.394f, 0.636f ), // 1.3
		new Vector3(1.56f, 1.394f, 0.0735f ), // 1.4
		new Vector3(1.56f, 1.394f, -0.489f ), // 1.5
		new Vector3(1.56f, 1.394f, -1.0515f), // 1.6
		new Vector3(1.56f,1.394f, -1.614f), // 1.7
		new Vector3(1.56f, 1.394f, -2.1765f), // 1.8
		
	   // BALDA 2
	   new Vector3(1.56f, 1.074f, -2.1765f), // 2.8
	   new Vector3(1.56f,1.074f, -1.614f), // 2.7
	   new Vector3(1.56f, 1.074f, -1.0515f), // 2.6
	   new Vector3(1.56f, 1.074f, -0.489f ), // 2.5
	   new Vector3(1.56f, 1.074f, 0.0735f ), // 2.4
	   new Vector3(1.56f,1.074f, 0.636f ), // 2.3
	   new Vector3(1.56f,1.074f, 1.1985f ),// 2.2
	   new Vector3(1.56f, 1.074f, 1.761f ), // 2.1
	   
	   // BALDA 3
		new Vector3(1.56f, 0.84f, 1.761f ), // 3.1
		new Vector3(1.56f,0.84f, 1.1985f ),   // 3.2
		new Vector3(1.56f,0.84f, 0.636f ), // 3.3
		new Vector3(1.56f, 0.84f, 0.0735f ), // 3.4
		new Vector3(1.56f, 0.84f, -0.489f ), // 3.5
		new Vector3(1.56f, 0.84f, -1.0515f), // 3.6
		new Vector3(1.56f,0.84f, -1.614f), // 3.7
		new Vector3(1.56f, 0.84f, -2.1765f), // 3.8
		
		// BALDA 4
		new Vector3(1.56f, 0.334f, -2.1765f), // 4.8
		new Vector3(1.56f,0.334f, -1.614f), // 4.7
		new Vector3(1.56f, 0.334f, -1.0515f), // 4.6
		new Vector3(1.56f, 0.334f, -0.489f ), // 4.5
		new Vector3(1.56f, 0.334f, 0.0735f ), // 4.4
		new Vector3(1.56f, 0.334f, 0.636f ), // 4.3
		new Vector3(1.56f,0.334f, 1.1985f ),   // 4.2
		new Vector3(1.56f, 0.334f, 1.761f ), // 4.1
	   
	};

	




	public List<List<GameObject>> allLevelProducts = new List<List<GameObject>>();
	public List<GameObject> Nivel1;
	public List<GameObject> Nivel2;
	public List<GameObject> Nivel3;
	public List<GameObject> Nivel4;
	public List<GameObject> Nivel5;
	public List<GameObject> Nivel6;
	

	Dictionary<GameObject, GameObject> Diz_randomPositions = new Dictionary<GameObject, GameObject>();//DIZIONARIO RANDOM

	void Awake()
	{
		/*
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
		*/
		UpdateShelf();

	}

	public void GetProductsByLevel2(int level)
	{

		if (Nivel1 != null) allLevelProducts.Add(Nivel1);
		if (Nivel2 != null) allLevelProducts.Add(Nivel2);
		if (Nivel3 != null) allLevelProducts.Add(Nivel3);
		if (Nivel4 != null) allLevelProducts.Add(Nivel4);
		if (Nivel5 != null) allLevelProducts.Add(Nivel5);
		if (Nivel6 != null) allLevelProducts.Add(Nivel6);
	   

		Debug.Log("!!!! SH2 allLevelProducts ha " + allLevelProducts.Count + " livelli.");
		/*
		for (int i = 0; i < allLevelProducts.Count; i++)
		{
			Debug.Log("SH2 Livello " + (i + 1) + " ha " + allLevelProducts[i].Count + " prodotti.");
			foreach (var product in allLevelProducts[i])
			{
				Debug.Log("SH2 Livello " + (i + 1) + " contiene: " + product.name);
			}
		}
		*/


		if (level < 1 || level > allLevelProducts.Count)
		{
			Debug.Log("!!!! SH2 Chiamato GetProductsByLevel per il livello: " + level);
			Debug.Log("!!!! SH2 allLevelProducts.Count: " + allLevelProducts.Count);

			Debug.LogWarning("!!!! SH2 Livello non valido: " + level);
			YourProducts = new List<GameObject>(); // Imposta game_shelves a una lista vuota se il livello non è valido
			return;
		}

		YourProducts = allLevelProducts[level - 1]; // Aggiorna game_shelves con la lista degli scaffali per il livello selezionato
		//foreach (GameObject obj in YourProducts)
		//{
        //{
		//	Debug.LogWarning("!!!! SH2 Oggetti nello scaffale: " + obj.name);
		//}
		
	}


	void Update()
	{
	
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
	   
		else
		{
			//PrintOriginalPositionDictionary();
			Debug.Log("AAA tutto rimane uguale.");
		}
	}




	// MIS PRODUCTOS
	void RandomizePositionsYourProducts(List<GameObject> items2)
	{
		List<Vector3> availablePositions2 = new List<Vector3>(TotPositions);
		occupiedPositions.Clear();
		Diz_randomPositions.Clear();


		Dictionary<string, List<GameObject>> groupedProducts2 = new Dictionary<string, List<GameObject>>();
		List<GameObject> productsToPlace2 = new List<GameObject>();

		int totalPositions2 = availablePositions2.Count;
		int totalItems2 = items2.Count;

		// Calcola il numero di copie per ciascun prodotto per riempire tutte le posizioni
		int baseCount2 = totalPositions2 / totalItems2; 
		int remainder2 = totalPositions2 % totalItems2; 

		// Aggiungi copie dei prodotti in base alle posizioni disponibili
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

		// Mescola i prodotti mantenendo quelli uguali vicini
		productsToPlace2 = productsToPlace2.OrderBy(p => p.name).ThenBy(x => Random.value).ToList();

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

		PrintDictionary(); // DIZIONARIO
	}
	Vector3 GetNextAdjacentPosition2(List<Vector3> availablePositions2)
	{
		if (availablePositions2.Count == 0)
		{
			Debug.LogWarning("!!!! Non ci sono più posizioni disponibili!");
			return Vector3.zero;
		}

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



	private List<List<string>> prodottiRimossiShelve2;


	public void PassaLaLista(List<List<string>> prodottiRimossi)
	{
		prodottiRimossiShelve2 = prodottiRimossi; // Assegna direttamente la lista delle gerarchie
		Debug.Log("!!!! PassaLaLista chiamato con " + prodottiRimossiShelve2.Count + " prodotti.");

		//foreach (var gerarchia in prodottiRimossiShelve2)
		//{
		//	Debug.Log("!!!! LISTA PASSATA: Prodotto con gerarchia: " + string.Join(" -> ", gerarchia));
		//}
	}
	
	public Transform dynamicChild;
	public GameObject Products2;

	public void RestoreOriginalPositions()
	{
		StartCoroutine(RestoreOriginalPositions2Async());

		/*
		Debug.Log("!!!! Entrato in RestoreOriginalPositions ");
		Debug.Log($"!!!! Numero di elementi nel dizionario: {Diz_randomPositions.Count}");


		Debug.Log($"[IIII RandomizeShelfPositions2] dynamicChild passato: {dynamicChild?.name}");

	

		// Elimina i figli attuali dello scaffale
		foreach (Transform child in shelves.transform)
		{
			if (child.name != "colliders")
			{
				Destroy(child.gameObject);
			}
		}

		// Crea una lista per i nuovi cloni attivi
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
			//Debug.Log($" !!!! PRODOTTO Nuova posizione di {newClone2.name} (attivato): {newClone2.transform.position}");
		}

		if (prodottiRimossiShelve2 != null && prodottiRimossiShelve2.Count > 0)
		{
			Debug.Log("!!!! PRODOTTO Inizio controllo sui prodotti attivi in 'products'.");

			// Mappa dei prodotti attivi in 'Products' per nome essenziale
			HashSet<string> activeProductNames = new HashSet<string>();

			if (dynamicChild != null && dynamicChild.gameObject.activeSelf)
			{

				string dynamicProductName = dynamicChild.name.Replace("Product_", "").Trim();
				activeProductNames.Add(dynamicProductName);
				Debug.Log($"IIII 2 PRODOTTO Oggetto dinamico aggiunto alla lista: {dynamicChild.name} (nome essenziale: {dynamicProductName})");

			}
			else
			{
				Debug.Log("IIII 2 Nessun oggetto dinamico attualmente afferrato.");
			}


			foreach (Transform product in Products2.transform)
			{
				if (product.gameObject.activeSelf)
				{

					string productName = product.name.Replace("Product_", "").Trim();
					activeProductNames.Add(productName);
					//Debug.Log($"PRODOTTO Prodotto attivo trovato: {product.name} (nome essenziale: {productName})");
				}
			}
		   

			// Rimuovi dalla lista `prodottiRimossiShelve3` i prodotti che non sono attivi in `Products`
			for (int i = prodottiRimossiShelve2.Count - 1; i >= 0; i--)
			{
				var gerarchia3 = prodottiRimossiShelve2[i];
				string rawProductName = gerarchia3[2].Replace("BOX_", "").Replace("box_", "").Split(' ')[0];


				//Debug.Log($" PRODOTTO Controllo prodotto nella lista: {string.Join(" -> ", gerarchia3)} (nome target: {rawProductName})");

				// Se il prodotto non è attivo in 'Products', rimuovilo dalla lista
				if (!activeProductNames.Contains(rawProductName))
				{
					//Debug.Log($" PRODOTTO Rimozione dalla lista: {string.Join(" -> ", gerarchia3)} (non attivo in 'products').");
					prodottiRimossiShelve2.RemoveAt(i);
				}
			}

			
			foreach (var gerarchia in prodottiRimossiShelve2)
			{
				//Debug.Log($" PRODOTTO Eseguo DisattivaProdotto3 per: {string.Join(" -> ", gerarchia)}");
				DisattivaProdotto(gerarchia);
			}
		}
		else
		{
			Debug.Log("!!!! PRODOTTO Nessun prodotto rimosso disponibile.");
		}
		*/
	}

	public IEnumerator RestoreOriginalPositions2Async()
	{
		Debug.Log("!!!! Entrato in RestoreOriginalPositions ");
		//Debug.Log($"!!!! Numero di elementi nel dizionario: {Diz_randomPositions.Count}");
		//Debug.Log($"[IIII RandomizeShelfPositions2] dynamicChild passato: {dynamicChild?.name}");


		// Elimina i figli attuali dello scaffale
		foreach (Transform child in shelves.transform)
		{
			if (child.name != "colliders")
			{
				Destroy(child.gameObject);
			}
		}
		yield return null;

		// Crea una lista per i nuovi cloni attivi
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
										  //Debug.Log($" !!!! PRODOTTO Nuova posizione di {newClone2.name} (attivato): {newClone2.transform.position}");
		}
		yield return null;

		if (prodottiRimossiShelve2 != null && prodottiRimossiShelve2.Count > 0)
		{
			Debug.Log("!!!! PRODOTTO Inizio controllo sui prodotti attivi in 'products'.");

			// Mappa dei prodotti attivi in 'Products' per nome essenziale
			HashSet<string> activeProductNames = new HashSet<string>();

			if (dynamicChild != null && dynamicChild.gameObject.activeSelf)
			{

				string dynamicProductName = dynamicChild.name.Replace("Product_", "").Trim();
				activeProductNames.Add(dynamicProductName);
				Debug.Log($"IIII 2 PRODOTTO Oggetto dinamico aggiunto alla lista: {dynamicChild.name} (nome essenziale: {dynamicProductName})");

			}
			else
			{
				Debug.Log("IIII 2 Nessun oggetto dinamico attualmente afferrato.");
			}


			foreach (Transform product in Products2.transform)
			{
				if (product.gameObject.activeSelf)
				{

					string productName = product.name.Replace("Product_", "").Trim();
					activeProductNames.Add(productName);
					//Debug.Log($"PRODOTTO Prodotto attivo trovato: {product.name} (nome essenziale: {productName})");
				}
			}

			yield return null;
			// Rimuovi dalla lista `prodottiRimossiShelve3` i prodotti che non sono attivi in `Products`
			for (int i = prodottiRimossiShelve2.Count - 1; i >= 0; i--)
			{
				var gerarchia3 = prodottiRimossiShelve2[i];
				string rawProductName = gerarchia3[2].Replace("BOX_", "").Replace("box_", "").Split(' ')[0];


				//Debug.Log($" PRODOTTO Controllo prodotto nella lista: {string.Join(" -> ", gerarchia3)} (nome target: {rawProductName})");

				// Se il prodotto non è attivo in 'Products', rimuovilo dalla lista
				if (!activeProductNames.Contains(rawProductName))
				{
					//Debug.Log($" PRODOTTO Rimozione dalla lista: {string.Join(" -> ", gerarchia3)} (non attivo in 'products').");
					prodottiRimossiShelve2.RemoveAt(i);
				}
			}
			yield return null;

			foreach (var gerarchia in prodottiRimossiShelve2)
			{
				//Debug.Log($" PRODOTTO Eseguo DisattivaProdotto3 per: {string.Join(" -> ", gerarchia)}");
				DisattivaProdotto(gerarchia);
			}
			yield return null;
		}
		else
		{
			Debug.Log("!!!! PRODOTTO Nessun prodotto rimosso disponibile.");
		}
	}


	public void DisattivaProdotto(List<string> gerarchia)
	{
		string boxName = gerarchia[0];

		//BOX
		
		Transform boxTransform = FindBoxIgnoringClones(boxName);


		Debug.Log("PROVA Nome del BOX: " + boxName);

		if (boxTransform == null)
		{
			Debug.LogWarning("PROVA BOX " + boxName + " non trovato nella scena.");
			return; 
		}

		Transform currentTransform = boxTransform;
		for (int i = 1; i < gerarchia.Count; i++)
		{
			string currentName = gerarchia[i];
			currentTransform = currentTransform.Find(currentName);

			if (currentTransform == null)
			{
				//Debug.LogWarning("PROVA" + currentName + " non trovato in " + currentTransform.name);
				return;
			}
			//Debug.Log("PROVA Trovato: " + currentName + " in " + currentTransform.parent.name);
		}
		currentTransform.gameObject.SetActive(false);
		Debug.Log("PROVA Prodotto disattivato: " + currentTransform.name);  
	}

	private Transform FindBoxIgnoringClones(string boxName)
	{
		// Trova tutti gli oggetti nella scena
		GameObject[] allBoxes = GameObject.FindObjectsOfType<GameObject>();

		

		Debug.Log($"PROVA Nome del BOX pulito: {boxName}"); // nome del BOX pulito

		foreach (GameObject box in allBoxes)
		{
			// Rimuovi tutte le occorrenze di "(Clone)" dal nome del box
			string cleanBoxInScene = box.name.Replace("(Clone)", "").Trim();

			
			//Debug.Log($"PROVA Trovato BOX: {box.name} (pulito: {cleanBoxInScene})");

			// Confronta i nomi puliti
			if (cleanBoxInScene == boxName)
			{
				//Debug.Log($"PROVA Trovato BOX corrispondente: {box.name}"); 
				return box.transform; // Restituisci il primo BOX trovato
			}
		}

		
		Debug.LogWarning("PROVA Nessun BOX trovato che corrisponde a " + boxName);
		return null;
	}




	public void PrintDictionary()
		{
			if (Diz_randomPositions.Count == 0)
			{
				Debug.Log("!!!! Dizionario random vuoto");
			}
			else
			{
				foreach (KeyValuePair<GameObject, GameObject> entry in Diz_randomPositions)
				{
					GameObject savedObject = entry.Value; 

					//Debug.Log("!!!! Prodotto: " + entry.Key.name
					//+ " - Posizione: " + savedObject.transform.position
					//+ " - Rotazione: " + savedObject.transform.rotation.eulerAngles
					//+ " - Scala: " + savedObject.transform.localScale);
				}
			}
	}
}














































