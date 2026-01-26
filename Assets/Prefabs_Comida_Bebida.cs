using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prefabs_Comida_Bebida : MonoBehaviour
{
    public GameObject prefabPlatoSucio;
    public GameObject prefabPlatoSucioPizza;
    public GameObject prefabPlatoSucioAlitas;
    public GameObject prefabPlatoSucioFruta;

    public List<GameObject> listaPrefabPlatoSucioN5;
    public List<GameObject> listaPrefabPlatoSucioN6;

    public GameObject prefabPlatoPollo;
    public GameObject prefabPlatoTostadas;
    public GameObject prefabPlatoPizza;
    public GameObject prefabPlatoTarta;
    public GameObject prefabPlatoDesayuno;
    public GameObject prefabPlatoHamburguesa;

    private void Start()
    {
        listaPrefabPlatoSucioN5.Add(prefabPlatoSucio);
        listaPrefabPlatoSucioN5.Add(prefabPlatoSucioFruta);

        listaPrefabPlatoSucioN6.Add(prefabPlatoSucio);
        listaPrefabPlatoSucioN6.Add(prefabPlatoSucioFruta);
        listaPrefabPlatoSucioN6.Add(prefabPlatoSucioPizza);
        listaPrefabPlatoSucioN6.Add(prefabPlatoSucioAlitas);
    }
}
