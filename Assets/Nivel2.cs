using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nivel2 : MonoBehaviour
{
    public VariablesComunes variablesComunes;
    public int currentLevel;

    private GameObject colisionPlatos;

    public int fallos_nivel_1;

    public bool cambioNivel;

    //private int faseActual;
    //private int faseAnterior;

    private bool platoInstantiated = false;

    private Vector3[] posMesa1;
    private Vector3[] rotMesa1;
    private Vector4[] posMesa2;
    private Vector4[] rotMesa2;
    private Vector3[] posMesa3;
    private Vector3[] rotMesa3;

    private Dictionary<string, MesaData> mesaData;

    public GameObject prefabPlatoSucio;

    private List<GameObject> instantiatedPlatosSucios = new List<GameObject>();
    private List<GameObject> instantiatedPlatosComida = new List<GameObject>();

    Prefabs_Comida_Bebida prefabsIntialization;

    void Start()
    {
        variablesComunes = GetComponent<VariablesComunes>();
        prefabsIntialization = GetComponent<Prefabs_Comida_Bebida>();

        fallos_nivel_1 = 0;

        //faseActual = variablesComunes.faseActual;
        //faseAnterior = ;

        posMesa1 = variablesComunes.posMesa1;
        rotMesa1 = variablesComunes.rotMesa1;
        posMesa2 = variablesComunes.posMesa2;
        rotMesa2 = variablesComunes.rotMesa2;
        posMesa3 = variablesComunes.posMesa3;
        rotMesa3 = variablesComunes.rotMesa3;

        colisionPlatos = GameObject.Find("ColisionPlatos");

        //MesaDictionary();
        //faseActual = colisionPlatos.GetComponent<CambioFaseNivel1>().faseActual;
    }

    void Update()
    {
        //Debug.Log("Entra fase 0");
        currentLevel = variablesComunes.currentLevel;
        //faseActual = variablesComunes.faseActual;

        if (currentLevel == 1)
        {
            //faseActual = variablesComunes.faseActual;

            if (variablesComunes.repeticiones < variablesComunes.repeticiones_max)
            {
                if (variablesComunes.faseActual < variablesComunes.fases_max)
                {
                    if (variablesComunes.faseAnterior != variablesComunes.faseActual)
                    {
                        platoInstantiated = false;
                    }
                    else
                    {
                        platoInstantiated = true;
                    }


                    if (!platoInstantiated)
                    {
                        // Deactivate "platos" (plates) of Comida and clear the instantiatedPlatosSucios list
                        DeactivatePlates(instantiatedPlatosComida);
                        instantiatedPlatosSucios.Clear();

                        ShuffleArrays();

                        MesaDictionary(posMesa1[0], rotMesa1[0], posMesa2[0], rotMesa2[0], posMesa3[0], rotMesa3[0]);
                        List<string> positionKeys = new List<string>(mesaData.Keys);
                        Shuffle(positionKeys);  // Shuffle the position keys once before the loop

                        for (int i = 0; i < 2; i++)
                        {
                            MesaDictionary(posMesa1[i], rotMesa1[i], posMesa2[i], rotMesa2[i], posMesa3[i], rotMesa3[i]);

                            MesaData mesa1Data = mesaData[positionKeys[0]];
                            InstantiatePlates(prefabsIntialization.prefabPlatoPollo, instantiatedPlatosComida, mesa1Data.Position, mesa1Data.Rotation); // Instantiate the first plate (Pollo)

                            MesaData mesa2Data = mesaData[positionKeys[1]];
                            InstantiatePlates(prefabPlatoSucio, instantiatedPlatosSucios, mesa2Data.Position, mesa2Data.Rotation); // Instantiate the second plate (Sucio)
                        }

                        platoInstantiated = true;

                        // Update the phase at the end
                        variablesComunes.faseAnterior = variablesComunes.faseActual;
                    }
                }
            }
        }
        else
        {
            if (platoInstantiated)
            {
                DeactivatePlates(instantiatedPlatosComida);
                DeactivatePlates(instantiatedPlatosSucios);
            }
        }

    }

    public void ShuffleArrays()
    {
        variablesComunes.ShuffleArraysV3(posMesa1, rotMesa1);
        variablesComunes.ShuffleArraysV4(posMesa2, rotMesa2);
        variablesComunes.ShuffleArraysV3(posMesa3, rotMesa3);
    }

    public class MesaData
    {
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public int Value { get; set; }
    }

    public void MesaDictionary(Vector3 posMesa1, Vector3 rotMesa1, Vector4 posMesa2, Vector4 rotMesa2, Vector3 posMesa3, Vector3 rotMesa3)
    {
        mesaData = new Dictionary<string, MesaData>();

        mesaData.Add("Mesa1", new MesaData { Position = posMesa1, Rotation = rotMesa1, Value = 1 });
        mesaData.Add("Mesa2", new MesaData { Position = posMesa2, Rotation = rotMesa2, Value = 2 });
        mesaData.Add("Mesa3", new MesaData { Position = posMesa3, Rotation = rotMesa3, Value = 3 });
    }

    private void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            int k = Random.Range(0, n--);
            T temp = list[n];
            list[n] = list[k];
            list[k] = temp;
        }
    }

    private void InstantiatePlates(GameObject prefab, List<GameObject> instantiatedPlates, Vector3 posMesa, Vector3 rotMesa)
    {
        GameObject plate = variablesComunes.CreateNewPlateRandomList(prefab, posMesa, rotMesa);
        instantiatedPlates.Add(plate);
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
