using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nivel1 : MonoBehaviour
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

        if (variablesComunes.repeticiones < variablesComunes.repeticiones_max)
        {
            if (variablesComunes.faseActual < variablesComunes.fases_max)
            {
                if (variablesComunes.faseAnterior != variablesComunes.faseActual)
                {
                    platoInstantiated = false;
                    ShuffleArrays();
                }
                else
                {
                    platoInstantiated = true;
                }


                if (!platoInstantiated)
                {
                    if(currentLevel == 1)
                    {
                        // Deactivate "platos" (plates) of Comida and clear the instantiatedPlatosSucios list
                        DeactivatePlates(variablesComunes.instantiatedPlatosComida);
                        variablesComunes.instantiatedPlatosComida.Clear();
                        variablesComunes.instantiatedPlatosSucios.Clear();

                        variablesComunes.savedPositionsPlatosComida.Clear();
                        variablesComunes.savedRotationsPlatosComida.Clear();
                        variablesComunes.savedPositionsPlatosSucios.Clear();
                        variablesComunes.savedRotationsPlatosSucios.Clear();

                        GameObject prefabComidaRandom = variablesComunes.SelectRandomPlate_6(prefabsIntialization.prefabPlatoPizza, prefabsIntialization.prefabPlatoPollo, prefabsIntialization.prefabPlatoDesayuno, prefabsIntialization.prefabPlatoTarta, prefabsIntialization.prefabPlatoTostadas, prefabsIntialization.prefabPlatoHamburguesa);

                        ShuffleArrays();

                        MesaDictionary(posMesa1[0], rotMesa1[0], posMesa2[0], rotMesa2[0], posMesa3[0], rotMesa3[0]);
                        List<string> positionKeys = new List<string>(mesaData.Keys);
                        Shuffle(positionKeys);  // Shuffle the position keys once before the loop

                        for (int i = 0; i < 2; i++)
                        {
                            MesaDictionary(posMesa1[i], rotMesa1[i], posMesa2[i], rotMesa2[i], posMesa3[i], rotMesa3[i]);

                            MesaData mesa1Data = mesaData[positionKeys[0]];
                            InstantiatePlates(prefabComidaRandom, variablesComunes.instantiatedPlatosComida, mesa1Data.Position, mesa1Data.Rotation); // Instantiate the first plate (Pollo)

                            MesaData mesa2Data = mesaData[positionKeys[1]];
                            InstantiatePlates(prefabPlatoSucio, variablesComunes.instantiatedPlatosSucios, mesa2Data.Position, mesa2Data.Rotation); // Instantiate the second plate (Sucio)
                        }

                        platoInstantiated = true;

                        // Update the phase at the end
                        variablesComunes.faseAnterior = variablesComunes.faseActual;
                    }
                    else if (currentLevel == 2)
                    {
                        // Deactivate "platos" (plates) of Comida and clear the instantiatedPlatosSucios list
                        DeactivatePlates(variablesComunes.instantiatedPlatosComida);
                        variablesComunes.instantiatedPlatosComida.Clear();
                        variablesComunes.instantiatedPlatosSucios.Clear();

                        variablesComunes.savedPositionsPlatosComida.Clear();
                        variablesComunes.savedRotationsPlatosComida.Clear();
                        variablesComunes.savedPositionsPlatosSucios.Clear();
                        variablesComunes.savedRotationsPlatosSucios.Clear();

                        ShuffleArrays();

                        MesaDictionary(posMesa1[0], rotMesa1[0], posMesa2[0], rotMesa2[0], posMesa3[0], rotMesa3[0]);
                        List<string> positionKeys = new List<string>(mesaData.Keys);
                        Shuffle(positionKeys); // Shuffle the keys once

                        for (int i = 0; i < 3; i++)  // Assuming you have 3 platos to instantiate
                        {
                            MesaDictionary(posMesa1[i], rotMesa1[i], posMesa2[i], rotMesa2[i], posMesa3[i], rotMesa3[i]); // Use the correct positions and rotations for each "plato"

                            MesaData mesa1Data = mesaData[positionKeys[0]];
                            InstantiatePlates(variablesComunes.SelectRandomPlate(prefabsIntialization.prefabPlatoPizza, prefabsIntialization.prefabPlatoPollo, prefabsIntialization.prefabPlatoDesayuno), variablesComunes.instantiatedPlatosComida, mesa1Data.Position, mesa1Data.Rotation); // Instantiate the first plato (clean or pollo plate)

                            MesaData mesa2Data = mesaData[positionKeys[1]];
                            InstantiatePlates(prefabPlatoSucio, variablesComunes.instantiatedPlatosSucios, mesa2Data.Position, mesa2Data.Rotation); // Instantiate the second plato (dirty plate)

                            if (i != 1)  // Only Plato 2 skips the third plate instantiation
                            {
                                MesaData mesa3Data = mesaData[positionKeys[2]];
                                InstantiatePlates(variablesComunes.SelectRandomPlate(prefabsIntialization.prefabPlatoPizza, prefabsIntialization.prefabPlatoPollo, prefabsIntialization.prefabPlatoDesayuno), variablesComunes.instantiatedPlatosComida, mesa3Data.Position, mesa3Data.Rotation);
                            }
                        }

                        platoInstantiated = true;

                        // Update the phase at the end
                        variablesComunes.faseAnterior = variablesComunes.faseActual;
                    }
                    else if (currentLevel == 3)
                    {
                        DeactivatePlates(variablesComunes.instantiatedPlatosComida); //Desactivo los platos de Comida y borro de la lista - Solo borro de la lista los platos sucios (Ya estan desactivados)
                        variablesComunes.instantiatedPlatosComida.Clear();
                        variablesComunes.instantiatedPlatosSucios.Clear();

                        variablesComunes.savedPositionsPlatosComida.Clear();
                        variablesComunes.savedRotationsPlatosComida.Clear();
                        variablesComunes.savedPositionsPlatosSucios.Clear();
                        variablesComunes.savedRotationsPlatosSucios.Clear();

                        ShuffleArrays();

                        MesaDictionary(posMesa1[0], rotMesa1[0], posMesa2[0], rotMesa2[0], posMesa3[0], rotMesa3[0]);
                        List<string> positionKeys = new List<string>(mesaData.Keys);
                        Shuffle(positionKeys); // Shuffle the keys only once, and use them in the loop

                        for (int i = 0; i < 3; i++)
                        {
                            MesaDictionary(posMesa1[i], rotMesa1[i], posMesa2[i], rotMesa2[i], posMesa3[i], rotMesa3[i]);

                            if (i < 3)
                            {
                                // Instantiating clean and dirty plates for Plato 1 and Plato 2
                                MesaData mesa1Data = mesaData[positionKeys[0]];
                                InstantiatePlates(variablesComunes.SelectRandomPlate_6(prefabsIntialization.prefabPlatoPizza, prefabsIntialization.prefabPlatoPollo, prefabsIntialization.prefabPlatoDesayuno, prefabsIntialization.prefabPlatoTarta, prefabsIntialization.prefabPlatoTostadas, prefabsIntialization.prefabPlatoHamburguesa), variablesComunes.instantiatedPlatosComida, mesa1Data.Position, mesa1Data.Rotation);

                                MesaData mesa2Data = mesaData[positionKeys[1]];
                                InstantiatePlates(prefabPlatoSucio, variablesComunes.instantiatedPlatosSucios, mesa2Data.Position, mesa2Data.Rotation);

                                MesaData mesa3Data = mesaData[positionKeys[2]];
                                InstantiatePlates(prefabPlatoSucio, variablesComunes.instantiatedPlatosSucios, mesa3Data.Position, mesa3Data.Rotation);
                            }
                        }

                        platoInstantiated = true;

                        // Update the phase at the end
                        variablesComunes.faseAnterior = variablesComunes.faseActual;
                    }
                    else if (currentLevel == 4)
                    {
                        DeactivatePlates(variablesComunes.instantiatedPlatosComida); //Desactivo los platos de Comida y borro de la lista - Solo borro de la lista los platos sucios (Ya estan desactivados)
                        variablesComunes.instantiatedPlatosComida.Clear();
                        variablesComunes.instantiatedPlatosSucios.Clear();

                        variablesComunes.savedPositionsPlatosComida.Clear();
                        variablesComunes.savedRotationsPlatosComida.Clear();
                        variablesComunes.savedPositionsPlatosSucios.Clear();
                        variablesComunes.savedRotationsPlatosSucios.Clear();

                        ShuffleArrays();

                        MesaDictionary(posMesa1[0], rotMesa1[0], posMesa2[0], rotMesa2[0], posMesa3[0], rotMesa3[0]);

                        List<string> positionKeys = new List<string>(mesaData.Keys);
                        Shuffle(positionKeys);

                        for (int i = 0; i < 3; i++)
                        {
                            MesaDictionary(posMesa1[0], rotMesa1[0], posMesa2[0], rotMesa2[0], posMesa3[0], rotMesa3[0]);
                            MesaData mesaDataSelected1 = mesaData[positionKeys[i]]; //Plato sucio mesas 1 y 2
                            InstantiatePlates(variablesComunes.SelectRandomPlate_6(prefabsIntialization.prefabPlatoPizza, prefabsIntialization.prefabPlatoPollo, prefabsIntialization.prefabPlatoDesayuno, prefabsIntialization.prefabPlatoTarta, prefabsIntialization.prefabPlatoTostadas, prefabsIntialization.prefabPlatoHamburguesa), variablesComunes.instantiatedPlatosComida, mesaDataSelected1.Position, mesaDataSelected1.Rotation);

                            MesaDictionary(posMesa1[1], rotMesa1[1], posMesa2[1], rotMesa2[1], posMesa3[1], rotMesa3[1]);
                            MesaData mesaDataSelected2 = mesaData[positionKeys[i]]; //Plato limpio mesas 1 y 2
                            InstantiatePlates(prefabsIntialization.prefabPlatoSucio, variablesComunes.instantiatedPlatosSucios, mesaDataSelected2.Position, mesaDataSelected2.Rotation);

                            MesaDictionary(posMesa1[2], rotMesa1[2], posMesa2[2], rotMesa2[2], posMesa3[2], rotMesa3[2]);
                            MesaData mesaDataSelected3 = mesaData[positionKeys[i]]; //Plato sucio mesas 1 y 2
                            InstantiatePlates(prefabsIntialization.prefabPlatoSucio, variablesComunes.instantiatedPlatosComida, mesaDataSelected3.Position, mesaDataSelected3.Rotation);
                        }

                        platoInstantiated = true;

                        // Update the phase at the end
                        variablesComunes.faseAnterior = variablesComunes.faseActual;
                    }
                    else if (currentLevel == 5)
                    {
                        DeactivatePlates(variablesComunes.instantiatedPlatosComida); //Desactivo los platos de Comida y borro de la lista - Solo borro de la lista los platos sucios (Ya estan desactivados)
                        variablesComunes.instantiatedPlatosComida.Clear();
                        variablesComunes.instantiatedPlatosSucios.Clear();

                        variablesComunes.savedPositionsPlatosComida.Clear();
                        variablesComunes.savedRotationsPlatosComida.Clear();
                        variablesComunes.savedPositionsPlatosSucios.Clear();
                        variablesComunes.savedRotationsPlatosSucios.Clear();

                        ShuffleArrays();

                        MesaDictionary(posMesa1[0], rotMesa1[0], posMesa2[0], rotMesa2[0], posMesa3[0], rotMesa3[0]);

                        List<string> positionKeys = new List<string>(mesaData.Keys);
                        Shuffle(positionKeys);

                        for (int i = 0; i < 3; i++)
                        {
                            MesaDictionary(posMesa1[0], rotMesa1[0], posMesa2[0], rotMesa2[0], posMesa3[0], rotMesa3[0]);
                            MesaData mesaDataSelected1 = mesaData[positionKeys[i]]; //Plato sucio mesas 1 y 2
                            InstantiatePlates(variablesComunes.SelectRandomPlate_6(prefabsIntialization.prefabPlatoPizza, prefabsIntialization.prefabPlatoPollo, prefabsIntialization.prefabPlatoDesayuno, prefabsIntialization.prefabPlatoTarta, prefabsIntialization.prefabPlatoTostadas, prefabsIntialization.prefabPlatoHamburguesa), variablesComunes.instantiatedPlatosComida, mesaDataSelected1.Position, mesaDataSelected1.Rotation);

                            MesaDictionary(posMesa1[1], rotMesa1[1], posMesa2[1], rotMesa2[1], posMesa3[1], rotMesa3[1]);
                            MesaData mesaDataSelected2 = mesaData[positionKeys[i]]; //Plato limpio mesas 1 y 2
                            InstantiatePlates(prefabsIntialization.prefabPlatoSucioAlitas, variablesComunes.instantiatedPlatosSucios, mesaDataSelected2.Position, mesaDataSelected2.Rotation);

                            MesaDictionary(posMesa1[2], rotMesa1[2], posMesa2[2], rotMesa2[2], posMesa3[2], rotMesa3[2]);
                            MesaData mesaDataSelected3 = mesaData[positionKeys[i]]; //Plato sucio mesas 1 y 2
                            InstantiatePlates(prefabsIntialization.prefabPlatoSucioAlitas, variablesComunes.instantiatedPlatosComida, mesaDataSelected3.Position, mesaDataSelected3.Rotation);
                        }

                        platoInstantiated = true;

                        // Update the phase at the end
                        variablesComunes.faseAnterior = variablesComunes.faseActual;
                    }
                    else if (currentLevel == 6)
                    {
                        DeactivatePlates(variablesComunes.instantiatedPlatosComida); //Desactivo los platos de Comida y borro de la lista - Solo borro de la lista los platos sucios (Ya estan desactivados)
                        variablesComunes.instantiatedPlatosComida.Clear();
                        variablesComunes.instantiatedPlatosSucios.Clear();

                        variablesComunes.savedPositionsPlatosComida.Clear();
                        variablesComunes.savedRotationsPlatosComida.Clear();
                        variablesComunes.savedPositionsPlatosSucios.Clear();
                        variablesComunes.savedRotationsPlatosSucios.Clear();

                        ShuffleArrays();

                        MesaDictionary(posMesa1[0], rotMesa1[0], posMesa2[0], rotMesa2[0], posMesa3[0], rotMesa3[0]);

                        List<string> positionKeys = new List<string>(mesaData.Keys);
                        Shuffle(positionKeys);

                        for (int i = 0; i < 3; i++)
                        {
                            MesaDictionary(posMesa1[0], rotMesa1[0], posMesa2[0], rotMesa2[0], posMesa3[0], rotMesa3[0]);
                            MesaData mesaDataSelected1 = mesaData[positionKeys[i]]; //Plato sucio mesas 1 y 2
                            InstantiatePlates(variablesComunes.SelectRandomPlate_6(prefabsIntialization.prefabPlatoPizza, prefabsIntialization.prefabPlatoPollo, prefabsIntialization.prefabPlatoDesayuno, prefabsIntialization.prefabPlatoTarta, prefabsIntialization.prefabPlatoTostadas, prefabsIntialization.prefabPlatoHamburguesa), variablesComunes.instantiatedPlatosComida, mesaDataSelected1.Position, mesaDataSelected1.Rotation);

                            MesaDictionary(posMesa1[1], rotMesa1[1], posMesa2[1], rotMesa2[1], posMesa3[1], rotMesa3[1]);
                            MesaData mesaDataSelected2 = mesaData[positionKeys[i]]; //Plato limpio mesas 1 y 2
                            InstantiatePlates(variablesComunes.SelectRandomPlate_List(prefabsIntialization.listaPrefabPlatoSucioN6), variablesComunes.instantiatedPlatosSucios, mesaDataSelected2.Position, mesaDataSelected2.Rotation);

                            MesaDictionary(posMesa1[2], rotMesa1[2], posMesa2[2], rotMesa2[2], posMesa3[2], rotMesa3[2]);
                            MesaData mesaDataSelected3 = mesaData[positionKeys[i]]; //Plato sucio mesas 1 y 2
                            InstantiatePlates(variablesComunes.SelectRandomPlate_List(prefabsIntialization.listaPrefabPlatoSucioN6), variablesComunes.instantiatedPlatosComida, mesaDataSelected3.Position, mesaDataSelected3.Rotation);
                        }

                        platoInstantiated = true;

                        // Update the phase at the end
                        variablesComunes.faseAnterior = variablesComunes.faseActual;
                    }
                    else
                    {
                        if (platoInstantiated)
                        {
                            DeactivatePlates(variablesComunes.instantiatedPlatosComida);
                            DeactivatePlates(variablesComunes.instantiatedPlatosSucios);
                        }
                    }
                }
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
        if (instantiatedPlates == variablesComunes.instantiatedPlatosComida)
        {
            variablesComunes.savedPositionsPlatosComida.Add(new Vector3(posMesa.x, posMesa.y, posMesa.z));
            variablesComunes.savedRotationsPlatosComida.Add(new Vector3(0, rotMesa.y, prefab.transform.rotation.z)); //Quaternion.Euler(0, rotMesa.y, prefab.transform.rotation.z)
        }
        else if (instantiatedPlates == variablesComunes.instantiatedPlatosSucios)
        {
            //Debug.Log("Prefab name: " + prefab.name);
            if (prefab.name == "plato_sucio")
            {
                variablesComunes.savedPositionsPlatosSucios.Add(new Vector3(posMesa.x, posMesa.y, posMesa.z));
                variablesComunes.savedRotationsPlatosSucios.Add(new Vector3(rotMesa.x, rotMesa.y, rotMesa.z));

                //Debug.Log("XDDDDDD");

            }
            else if (prefab.name.Contains("plato_sucio_fruta") | prefab.name.Contains("plato_sucio_alitas") | prefab.name.Contains("plato_sucio_pizza"))
            {
                float rot_plato_y = rotMesa.z + 180;

                variablesComunes.savedPositionsPlatosSucios.Add(new Vector3(posMesa.x, posMesa.y, posMesa.z));
                variablesComunes.savedRotationsPlatosSucios.Add(new Vector3(0, rot_plato_y, prefab.transform.rotation.z));
            }
        }

        GameObject plate = variablesComunes.CreateNewPlateRandomList(prefab, posMesa, rotMesa);
        instantiatedPlates.Add(plate);
        //Guardamos posiciones
        //savedPositions.Add(new Vector3(posMesa.x, posMesa.y, posMesa.z));
        //savedRotations.Add(new Vector3(rotMesa.x, rotMesa.y, rotMesa.z));        
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
