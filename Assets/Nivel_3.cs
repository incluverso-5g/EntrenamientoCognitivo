using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nivel_3 : MonoBehaviour
{
    public VariablesComunes variablesComunes;
    public int currentLevel;

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

    Prefabs_Comida_Bebida prefabsIntialization;

    private List<GameObject> instantiatedPlatosSucios = new List<GameObject>();
    private List<GameObject> instantiatedPlatosComida = new List<GameObject>();

    void Start()
    {
        variablesComunes = GetComponent<VariablesComunes>();
        prefabsIntialization = GetComponent<Prefabs_Comida_Bebida>();

        faseActual = variablesComunes.faseActual;
        faseAnterior = -1;

        posMesa1 = variablesComunes.posMesa1;
        rotMesa1 = variablesComunes.rotMesa1;
        posMesa2 = variablesComunes.posMesa2;
        rotMesa2 = variablesComunes.rotMesa2;
        posMesa3 = variablesComunes.posMesa3;
        rotMesa3 = variablesComunes.rotMesa3;
    }

    void Update()
    {
        currentLevel = variablesComunes.currentLevel;

        if (faseAnterior != faseActual)
        {
            platoInstantiated = false;
        }
        else
        {
            platoInstantiated = true;
        }

        if (currentLevel == 3)
        {
            faseActual = variablesComunes.faseActual;

            if (faseActual == 0 && !platoInstantiated)
            {
                DeactivatePlates(instantiatedPlatosComida); //Desactivo los platos de Comida y borro de la lista - Solo borro de la lista los platos sucios (Ya estan desactivados)
                instantiatedPlatosSucios.Clear();

                ShuffleArrays();

                MesaDictionary(posMesa1[0], rotMesa1[0], posMesa2[0], rotMesa2[0], posMesa3[0], rotMesa3[0]);

                List<string> positionKeys = new List<string>(mesaData.Keys);
                Shuffle(positionKeys);

                MesaData mesaDataSelected1 = mesaData[positionKeys[0]]; //Plato sucio mesa 1
                InstantiatePlates(variablesComunes.SelectRandomPlate(prefabsIntialization.prefabPlatoPollo, prefabsIntialization.prefabPlatoTostadas, prefabsIntialization.prefabPlatoPizza), instantiatedPlatosComida, mesaDataSelected1.Position, mesaDataSelected1.Rotation);

                MesaDictionary(posMesa1[1], rotMesa1[1], posMesa2[1], rotMesa2[1], posMesa3[1], rotMesa3[1]);
                MesaData mesaDataSelected2 = mesaData[positionKeys[0]]; //Plato limpio mesa 1
                InstantiatePlates(prefabsIntialization.prefabPlatoSucio, instantiatedPlatosSucios, mesaDataSelected2.Position, mesaDataSelected2.Rotation);

                faseAnterior = faseActual;
            }
            else if (faseActual == 1 && !platoInstantiated)
            {
                DeactivatePlates(instantiatedPlatosComida); //Desactivo los platos de Comida y borro de la lista - Solo borro de la lista los platos sucios (Ya estan desactivados)
                instantiatedPlatosSucios.Clear();

                ShuffleArrays();

                MesaDictionary(posMesa1[0], rotMesa1[0], posMesa2[0], rotMesa2[0], posMesa3[0], rotMesa3[0]);

                List<string> positionKeys = new List<string>(mesaData.Keys);
                Shuffle(positionKeys);

                for (int i = 0; i < 2; i++)
                {
                    MesaDictionary(posMesa1[0], rotMesa1[0], posMesa2[0], rotMesa2[0], posMesa3[0], rotMesa3[0]);
                    MesaData mesaDataSelected1 = mesaData[positionKeys[i]]; //Plato sucio mesas 1 y 2
                    InstantiatePlates(variablesComunes.SelectRandomPlate(prefabsIntialization.prefabPlatoPollo, prefabsIntialization.prefabPlatoTostadas, prefabsIntialization.prefabPlatoPizza), instantiatedPlatosComida, mesaDataSelected1.Position, mesaDataSelected1.Rotation);

                    MesaDictionary(posMesa1[1], rotMesa1[1], posMesa2[1], rotMesa2[1], posMesa3[1], rotMesa3[1]);
                    MesaData mesaDataSelected2 = mesaData[positionKeys[i]]; //Plato limpio mesas 1 y 2
                    InstantiatePlates(prefabsIntialization.prefabPlatoSucio, instantiatedPlatosSucios, mesaDataSelected2.Position, mesaDataSelected2.Rotation);
                }

                faseAnterior = faseActual;
            }
            else if (faseActual == 2 && !platoInstantiated)
            {
                DeactivatePlates(instantiatedPlatosComida); //Desactivo los platos de Comida y borro de la lista - Solo borro de la lista los platos sucios (Ya estan desactivados)
                instantiatedPlatosSucios.Clear();

                ShuffleArrays();

                MesaDictionary(posMesa1[0], rotMesa1[0], posMesa2[0], rotMesa2[0], posMesa3[0], rotMesa3[0]);

                List<string> positionKeys = new List<string>(mesaData.Keys);
                Shuffle(positionKeys);

                for (int i = 0; i < 3; i++)
                {
                    MesaDictionary(posMesa1[0], rotMesa1[0], posMesa2[0], rotMesa2[0], posMesa3[0], rotMesa3[0]);
                    MesaData mesaDataSelected1 = mesaData[positionKeys[i]]; //Plato sucio mesas 1 y 2
                    InstantiatePlates(variablesComunes.SelectRandomPlate(prefabsIntialization.prefabPlatoPollo, prefabsIntialization.prefabPlatoTostadas, prefabsIntialization.prefabPlatoPizza), instantiatedPlatosComida, mesaDataSelected1.Position, mesaDataSelected1.Rotation);

                    MesaDictionary(posMesa1[1], rotMesa1[1], posMesa2[1], rotMesa2[1], posMesa3[1], rotMesa3[1]);
                    MesaData mesaDataSelected2 = mesaData[positionKeys[i]]; //Plato limpio mesas 1 y 2
                    InstantiatePlates(prefabsIntialization.prefabPlatoSucio, instantiatedPlatosSucios, mesaDataSelected2.Position, mesaDataSelected2.Rotation);

                    if (i == 2)
                    {
                        MesaDictionary(posMesa1[2], rotMesa1[2], posMesa2[2], rotMesa2[2], posMesa3[2], rotMesa3[2]);
                        MesaData mesaDataSelected3 = mesaData[positionKeys[i]]; //Plato sucio mesas 1 y 2
                        InstantiatePlates(prefabsIntialization.prefabPlatoSucio, instantiatedPlatosComida, mesaDataSelected3.Position, mesaDataSelected3.Rotation);
                    }
                }

                faseAnterior = faseActual;
            }
            else if (faseActual == 3 && !platoInstantiated)
            {
                DeactivatePlates(instantiatedPlatosComida); //Desactivo los platos de Comida y borro de la lista - Solo borro de la lista los platos sucios (Ya estan desactivados)
                instantiatedPlatosSucios.Clear();

                ShuffleArrays();

                MesaDictionary(posMesa1[0], rotMesa1[0], posMesa2[0], rotMesa2[0], posMesa3[0], rotMesa3[0]);

                List<string> positionKeys = new List<string>(mesaData.Keys);
                Shuffle(positionKeys);

                for (int i = 0; i < 3; i++)
                {
                    MesaDictionary(posMesa1[0], rotMesa1[0], posMesa2[0], rotMesa2[0], posMesa3[0], rotMesa3[0]);
                    MesaData mesaDataSelected1 = mesaData[positionKeys[i]]; //Plato sucio mesas 1 y 2
                    InstantiatePlates(variablesComunes.SelectRandomPlate(prefabsIntialization.prefabPlatoPollo, prefabsIntialization.prefabPlatoTostadas, prefabsIntialization.prefabPlatoPizza), instantiatedPlatosComida, mesaDataSelected1.Position, mesaDataSelected1.Rotation);

                    MesaDictionary(posMesa1[1], rotMesa1[1], posMesa2[1], rotMesa2[1], posMesa3[1], rotMesa3[1]);
                    MesaData mesaDataSelected2 = mesaData[positionKeys[i]]; //Plato limpio mesas 1 y 2
                    InstantiatePlates(prefabsIntialization.prefabPlatoSucio, instantiatedPlatosSucios, mesaDataSelected2.Position, mesaDataSelected2.Rotation);

                    MesaDictionary(posMesa1[2], rotMesa1[2], posMesa2[2], rotMesa2[2], posMesa3[2], rotMesa3[2]);
                    MesaData mesaDataSelected3 = mesaData[positionKeys[i]]; //Plato sucio mesas 1 y 2
                    InstantiatePlates(prefabsIntialization.prefabPlatoSucio, instantiatedPlatosComida, mesaDataSelected3.Position, mesaDataSelected3.Rotation);
                }

                faseAnterior = faseActual;
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
