using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nivel_Entrenamiento: MonoBehaviour
{
    VariablesComunes variablesComunes;

    private GameObject colisionPlatos;

    private Vector3[] posMesa1;
    private Vector3[] rotMesa1;
    private Vector4[] posMesa2;
    private Vector4[] rotMesa2;
    private Vector3[] posMesa3;
    private Vector3[] rotMesa3;

    //private int faseActual;
    private int faseAnterior = -1;
    private bool platoInstantiated = false;

    public GameObject prefabPlatoSucio;

    void Start()
    {
        variablesComunes = GetComponent<VariablesComunes>();

        //colisionPlatos = GameObject.Find("ColisionPlatos");
        //faseActual = colisionPlatos.GetComponent<CambioFaseEntrenamiento>().faseActual;
        
        posMesa1 = variablesComunes.posMesa1;
        rotMesa1 = variablesComunes.rotMesa1;
        posMesa2 = variablesComunes.posMesa2;
        rotMesa2 = variablesComunes.rotMesa2;
        posMesa3 = variablesComunes.posMesa3;
        rotMesa3 = variablesComunes.rotMesa3;
    }

    void Update()
    {

        if (faseAnterior != variablesComunes.faseActual)
        {
            platoInstantiated = false;
        }
        else
        {
            platoInstantiated = true;
        }

        if (variablesComunes.faseActual == 0 && !platoInstantiated)
        {
            //1 plato Mesa1
            variablesComunes.ShuffleArraysV3(posMesa1, rotMesa1);
            variablesComunes.CreateNewPlateRandomList(prefabPlatoSucio, posMesa1[0], rotMesa1[0]);

            faseAnterior = variablesComunes.faseActual;
        }
        else if (variablesComunes.faseActual == 1 && !platoInstantiated)
        {
            //2 platos Mesa1

            variablesComunes.ShuffleArraysV3(posMesa1, rotMesa1);
            variablesComunes.CreateNewPlateRandomList(prefabPlatoSucio, posMesa1[0], rotMesa1[0]);
            variablesComunes.CreateNewPlateRandomList(prefabPlatoSucio, posMesa1[1], rotMesa1[1]);

            platoInstantiated = true;
            faseAnterior = variablesComunes.faseActual;
        }
        else if (variablesComunes.faseActual == 2 && !platoInstantiated)
        {
            //1 plato Mesa1, 3 platos Mesa2

            variablesComunes.ShuffleArraysV3(posMesa1, rotMesa1);
            variablesComunes.CreateNewPlateRandomList(prefabPlatoSucio, posMesa1[0], rotMesa1[0]);

            variablesComunes.ShuffleArraysV4(posMesa2, rotMesa2);
            variablesComunes.CreateNewPlateRandomList(prefabPlatoSucio, posMesa2[0], rotMesa2[0]);
            variablesComunes.CreateNewPlateRandomList(prefabPlatoSucio, posMesa2[1], rotMesa2[1]);
            variablesComunes.CreateNewPlateRandomList(prefabPlatoSucio, posMesa2[2], rotMesa2[2]);

            platoInstantiated = true;
            faseAnterior = variablesComunes.faseActual;
        }
        else if (variablesComunes.faseActual == 3 && !platoInstantiated)
        {
            // 2 platos Mesa1, 3 platos Mesa2, 3 platos Mesa3

            variablesComunes.ShuffleArraysV3(posMesa1, rotMesa1);
            variablesComunes.CreateNewPlateRandomList(prefabPlatoSucio, posMesa1[0], rotMesa1[0]);
            variablesComunes.CreateNewPlateRandomList(prefabPlatoSucio, posMesa1[1], rotMesa1[1]);

            variablesComunes.ShuffleArraysV4(posMesa2, rotMesa2);
            variablesComunes.CreateNewPlateRandomList(prefabPlatoSucio, posMesa2[0], rotMesa2[0]);
            variablesComunes.CreateNewPlateRandomList(prefabPlatoSucio, posMesa2[1], rotMesa2[1]);
            variablesComunes.CreateNewPlateRandomList(prefabPlatoSucio, posMesa2[2], rotMesa2[2]);

            variablesComunes.ShuffleArraysV3(posMesa3, rotMesa3);
            variablesComunes.CreateNewPlateRandomList(prefabPlatoSucio, posMesa3[0], rotMesa3[0]);
            variablesComunes.CreateNewPlateRandomList(prefabPlatoSucio, posMesa3[1], rotMesa3[1]);
            variablesComunes.CreateNewPlateRandomList(prefabPlatoSucio, posMesa3[2], rotMesa3[2]);

            platoInstantiated = true;
            faseAnterior = variablesComunes.faseActual;
        }
        else
        {
            platoInstantiated = false;
        }
    }
}
