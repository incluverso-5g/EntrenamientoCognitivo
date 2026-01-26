using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SupermercadoT2Niveles : MonoBehaviour
{
    public VariablesNiveles variables;
    public RandomizeShelfPositions2T2 RandomizeShelfPositions2T2;
    public RandomizeShelfPositions3T2 RandomizeShelfPositions3T2;
    public PedidoLogic shelves;

    public int level;

    public List<int> ListaNiveles;

    public GameObject waveRig;

    private void Awake()
    {
        waveRig = GameObject.Find("Wave Rig");
        variables = waveRig.GetComponent<VariablesNiveles>(); //VariablesNiveles.instance;

        //variables = VariablesNiveles.instance;

        /*if (variables == null)
        {
            Debug.LogError("!!!! VariablesNiveles non esiste.");
        }
        else
        {
            Debug.Log("!!!! VariablesNiveles trovato, nivel_supermercado_T2: " + variables.nivel_supermercado_T2);
        }*/

        ListaNiveles = new List<int> { 1, 2, 3, 4, 5, 6 };

        SelectLevel();

        currentRepetitions = repetitionsPerLevel;
    }

    public void SelectLevel()

    {
       /* if (RandomizeShelfPositions3T2 != null)
        {
            Debug.Log("!!!! RandomizeShelfPositions3T2 trovato, chiamata a GetProductsByLevel3T2.");
            RandomizeShelfPositions3T2.GetProductsByLevel3T2(level);
        }
        else
        {
            Debug.LogError(" !!!!RandomizeShelfPositions3T2 non è assegnato!");
        }*/



        Debug.Log("!!!! Valor di nivel_supermercado_T2: " + variables.nivel_supermercado_T2);


        if (variables.nivel_supermercado_T2 >= 1 && variables.nivel_supermercado_T2 <= ListaNiveles.Count)
        {
            level = ListaNiveles[variables.nivel_supermercado_T2 - 1]; // Livello corrispondente
            Debug.Log("!!!! Livello impostato a " + level);
        }
        else
        {
            Debug.LogWarning("!!!! Nessun livello valido selezionato. Valor di nivel_supermercado_T2: " + variables.nivel_supermercado_T1);
            return;
        }


        Debug.Log("!!! Livello " + level + " selezionato");

        Debug.Log("!!!! Chiamata a RandomizeShelfPositions3T2.GetProductsByLevel con level: " + level);
        RandomizeShelfPositions3T2.GetProductsByLevel3T2(level);

        Debug.Log("!!!! Chiamata a RandomizeShelfPositions2T2.GetProductsByLevel con level: " + level);
        RandomizeShelfPositions2T2.GetProductsByLevel2T2(level);


        StartCoroutine(DelayedCall(3f));
       
    }

    private IEnumerator DelayedCall(float delay)
    {
        Debug.Log("!!!! Chiamata a shelves.GetShelvesByLevelT2 con level: " + level);
        yield return new WaitForSeconds(delay);
        shelves.GetShelvesByLevelT2(level);
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
            Debug.Log("!!! Livello T2 " + level + " completato con " + repetitionsPerLevel + " ripetizioni.");
            currentRepetitions = repetitionsPerLevel;
            enabled = false;
        }
    }
}
