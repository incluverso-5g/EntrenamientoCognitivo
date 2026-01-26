using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nivel_1 : MonoBehaviour
{
    public VariablesComunes variablesComunes;
    public int currentLevel;

    private GameObject colisionPlatos;

    public int fallos_nivel_1;

    private GameObject cambioFase;
    public bool cambioNivel;

    private int faseActual;
    private int faseAnterior;

    private bool platoInstantiated = false;

    private Vector3[] posMesa1;
    private Vector3[] rotMesa1;
    private Vector4[] posMesa2;
    private Vector4[] rotMesa2;
    private Vector3[] posMesa3;
    private Vector3[] rotMesa3;

    private Dictionary<string, MesaData> mesaData;

    public GameObject prefabPlatoSucio;

    public GameObject prefabPlatoPollo;
    public GameObject prefabPlatoTostadas;
    public GameObject prefabPlato2;

    private List<GameObject> instantiatedPlatosSucios = new List<GameObject>();
    private List<GameObject> instantiatedPlatosComida = new List<GameObject>();

    Prefabs_Comida_Bebida prefabsIntialization;

    void Start()
    {
        variablesComunes = GetComponent<VariablesComunes>();
        prefabsIntialization = GetComponent<Prefabs_Comida_Bebida>();

        fallos_nivel_1 = 0;

        faseActual = variablesComunes.faseActual;
        faseAnterior = -1;

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

        if (faseAnterior != faseActual)
        {
            platoInstantiated = false;
        }
        else
        {
            platoInstantiated = true;
        }


        if (currentLevel == 1)
        {
            faseActual = variablesComunes.faseActual;

            if (faseActual == 0 && !platoInstantiated)
            {
                ShuffleArrays();

                MesaDictionary(posMesa1[0], rotMesa1[0], posMesa2[0], rotMesa2[0], posMesa3[0], rotMesa3[0]);

                List<string> positionKeys = new List<string>(mesaData.Keys);
                Shuffle(positionKeys);

                MesaData mesaDataSelected1 = mesaData[positionKeys[0]];
                InstantiatePlates(variablesComunes.SelectRandomPlate(prefabsIntialization.prefabPlatoPizza, prefabsIntialization.prefabPlatoPollo, prefabsIntialization.prefabPlatoTostadas), instantiatedPlatosComida, mesaDataSelected1.Position, mesaDataSelected1.Rotation);

                MesaData mesaDataSelected2 = mesaData[positionKeys[1]];
                InstantiatePlates(prefabPlatoSucio, instantiatedPlatosSucios, mesaDataSelected2.Position, mesaDataSelected2.Rotation);

                faseAnterior = faseActual;
            }
            else if (faseActual == 1 && !platoInstantiated)
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
                    InstantiatePlates(variablesComunes.SelectRandomPlate(prefabsIntialization.prefabPlatoPizza, prefabsIntialization.prefabPlatoPollo, prefabsIntialization.prefabPlatoTostadas), instantiatedPlatosComida, mesa1Data.Position, mesa1Data.Rotation); // Instantiate the first plate (Pollo)

                    MesaData mesa2Data = mesaData[positionKeys[1]];
                    InstantiatePlates(prefabPlatoSucio, instantiatedPlatosSucios, mesa2Data.Position, mesa2Data.Rotation); // Instantiate the second plate (Sucio)
                }

                // Update the phase at the end
                faseAnterior = faseActual;

            }
            else if (faseActual == 2 && !platoInstantiated)
            {
                // Deactivate plates of Comida and clear the instantiatedPlatosSucios list
                DeactivatePlates(instantiatedPlatosComida);
                instantiatedPlatosSucios.Clear();

                ShuffleArrays();

                MesaDictionary(posMesa1[0], rotMesa1[0], posMesa2[0], rotMesa2[0], posMesa3[0], rotMesa3[0]);
                List<string> positionKeys = new List<string>(mesaData.Keys);
                Shuffle(positionKeys); // Shuffle the keys once

                for (int i = 0; i < 3; i++)  // Assuming you have 3 platos to instantiate
                {
                    MesaDictionary(posMesa1[i], rotMesa1[i], posMesa2[i], rotMesa2[i], posMesa3[i], rotMesa3[i]); // Use the correct positions and rotations for each "plato"

                    MesaData mesa1Data = mesaData[positionKeys[0]];
                    InstantiatePlates(variablesComunes.SelectRandomPlate(prefabsIntialization.prefabPlatoPizza, prefabsIntialization.prefabPlatoPollo, prefabsIntialization.prefabPlatoTostadas), instantiatedPlatosComida, mesa1Data.Position, mesa1Data.Rotation); // Instantiate the first plato (clean or pollo plate)

                    MesaData mesa2Data = mesaData[positionKeys[1]];
                    InstantiatePlates(prefabPlatoSucio, instantiatedPlatosSucios, mesa2Data.Position, mesa2Data.Rotation); // Instantiate the second plato (dirty plate)

                    if (i != 1)  // Only Plato 2 skips the third plate instantiation
                    {
                        MesaData mesa3Data = mesaData[positionKeys[2]];
                        InstantiatePlates(variablesComunes.SelectRandomPlate(prefabsIntialization.prefabPlatoPizza, prefabsIntialization.prefabPlatoPollo, prefabsIntialization.prefabPlatoTostadas), instantiatedPlatosComida, mesa3Data.Position, mesa3Data.Rotation);
                    }
                }

                faseAnterior = faseActual;  // Update the fase at the end

            }
            else if (faseActual == 3 && !platoInstantiated)
            {         
                DeactivatePlates(instantiatedPlatosComida); //Desactivo los platos de Comida y borro de la lista - Solo borro de la lista los platos sucios (Ya estan desactivados)
                instantiatedPlatosSucios.Clear();

                ShuffleArrays();

                MesaDictionary(posMesa1[0], rotMesa1[0], posMesa2[0], rotMesa2[0], posMesa3[0], rotMesa3[0]);
                List<string> positionKeys = new List<string>(mesaData.Keys);
                Shuffle(positionKeys); // Shuffle the keys only once, and use them in the loop

                for (int i = 0; i < 3; i++)
                {
                    MesaDictionary(posMesa1[i], rotMesa1[i], posMesa2[i], rotMesa2[i], posMesa3[i], rotMesa3[i]);

                    if (i < 2)
                    {
                        // Instantiating clean and dirty plates for Plato 1 and Plato 2
                        MesaData mesa1Data = mesaData[positionKeys[0]];
                        InstantiatePlates(variablesComunes.SelectRandomPlate(prefabsIntialization.prefabPlatoPizza, prefabsIntialization.prefabPlatoPollo, prefabsIntialization.prefabPlatoTostadas), instantiatedPlatosComida, mesa1Data.Position, mesa1Data.Rotation);

                        MesaData mesa2Data = mesaData[positionKeys[1]];
                        InstantiatePlates(prefabPlatoSucio, instantiatedPlatosSucios, mesa2Data.Position, mesa2Data.Rotation);

                        MesaData mesa3Data = mesaData[positionKeys[2]];
                        InstantiatePlates(prefabPlatoSucio, instantiatedPlatosSucios, mesa3Data.Position, mesa3Data.Rotation);
                    }
                    else if (i == 2)
                    {
                        // Instantiating dirty plates for Plato 3
                        MesaData mesa2Data = mesaData[positionKeys[1]];
                        InstantiatePlates(prefabPlatoSucio, instantiatedPlatosSucios, mesa2Data.Position, mesa2Data.Rotation);
                    }
                }

                faseAnterior = faseActual;
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

    public void ShuffleArrays(){
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

    private void DeactivatePlates(List <GameObject> instantiatedPlates)
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
