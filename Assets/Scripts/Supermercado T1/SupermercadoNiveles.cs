using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SupermercadoNiveles : MonoBehaviour
{
    public VariablesNiveles variables;
    public RandomizeShelfPositions2 RandomizeShelfPositions2;
    public RandomizeShelfPositions3 RandomizeShelfPositions3;
    public ReponerLogic1 shelves;

    public int level;
    
    public List<int> ListaNiveles;

    public GameObject waveRig;

    private void Awake()
    {
        waveRig = GameObject.Find("Wave Rig");
        variables = waveRig.GetComponent<VariablesNiveles>(); //VariablesNiveles.instance;

        /*if (variables == null)
        {
            Debug.LogError("!!!! VariablesNiveles non esiste.");
        }
        else
        {
            Debug.Log("!!!! VariablesNiveles trovato, nivel_supermercado_T1: " + variables.nivel_supermercado_T1);
        }*/

        ListaNiveles = new List<int> { 1, 2, 3, 4, 5, 6};

        SelectLevel();

        currentRepetitions = repetitionsPerLevel;
    }

    public void SelectLevel()

    {
        Debug.Log("!!!! Valor di nivel_supermercado_T1: " + variables.nivel_supermercado_T1);


        if (variables.nivel_supermercado_T1 >= 1 && variables.nivel_supermercado_T1 <= ListaNiveles.Count)
        {
            level = ListaNiveles[variables.nivel_supermercado_T1 - 1]; // Livello corrispondente
            Debug.Log("!!!! Livello impostato a " + level);
        }
        else
        {
            Debug.LogWarning("!!!! Nessun livello valido selezionato. Valor di nivel_supermercado_T1: " + variables.nivel_supermercado_T1);
            return;
        }

       
        Debug.Log("!!! Livello " + level + " selezionato");

        Debug.Log("!!!! SH3 Chiamata a RandomizeShelfPositions3.GetProductsByLevel con level: " + level);
        RandomizeShelfPositions3.GetProductsByLevel(level);

        Debug.Log("!!!! Chiamata a RandomizeShelfPositions2.GetProductsByLevel con level: " + level);
        RandomizeShelfPositions2.GetProductsByLevel2(level);


        Debug.Log("!!!! Chiamata a shelves.GetShelvesByLevel con level: " + level);
        shelves.GetShelvesByLevel(level);

    }

    public int repetitionsPerLevel = 3; // Numero di ripetizioni per ogni livello
    private int currentRepetitions; // Ripetizioni rimanenti per il livello attuale
    private int currentIndex = 0; // Indice del livello corrente nella lista

    public void Update()
    {
        if (currentRepetitions > 0)
        {
            
            currentRepetitions--;
        }
        else
        {
            //Debug.Log("!!! Livello " + level + " completato con " + repetitionsPerLevel + " ripetizioni.");
            currentRepetitions = repetitionsPerLevel;
            enabled = false; 
        }
    }
}



