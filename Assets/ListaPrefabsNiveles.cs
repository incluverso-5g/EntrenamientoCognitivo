using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListaPrefabsNiveles : MonoBehaviour
{
    public GameObject prefabFanta;
    public GameObject prefabCola;
    public GameObject prefabCafe;
    public GameObject prefabAgua;
    public GameObject prefabVino;
    public GameObject prefabCerveza;

    public GameObject prefabPlatoPollo;
    public GameObject prefabPlatoPizza;
    public GameObject prefabPlatoTarta;
    public GameObject prefabPlatoDesayuno;
    public GameObject prefabPlatoHamburguesa;

    public GameObject prefabPlatoHamburguesa2;
    public GameObject prefabPlatoTarta2;
    public GameObject prefabPlatoPizza2;
    public GameObject prefabPlatoAlbondigas;

    public GameObject prefabPlatoHamburguesa3;
    public GameObject prefabPlatoHamburguesa4;
    public GameObject prefabPlatoTarta3;
    public GameObject prefabPlatoTarta4;
    public GameObject prefabPlatoPizza3;
    public GameObject prefabPlatoPizza4;

    public GameObject prefabVino2;
    public GameObject prefabAgua2;
    public GameObject prefabZumo;
    public GameObject prefabZumo2;

    public GameObject prefabVino3;
    public GameObject prefabVino4;
    public GameObject prefabAgua3;
    public GameObject prefabAgua4;
    public GameObject prefabZumo3;
    public GameObject prefabZumo4;

    public List<GameObject> prefabListN1 = new List<GameObject>();
    public List<GameObject> prefabListN2 = new List<GameObject>();
    public List<GameObject> prefabListN3 = new List<GameObject>();
    public List<GameObject> prefabListN4 = new List<GameObject>();
    public List<GameObject> prefabListN5 = new List<GameObject>();
    public List<GameObject> prefabListN6 = new List<GameObject>();

    public List<List<GameObject>> similarPairsN4 = new List<List<GameObject>>();
    public List<List<GameObject>> similarPairsN6 = new List<List<GameObject>>();


    private void Start()
    {
        //Add to N1 list
        prefabListN1.AddRange(new List<GameObject> { prefabFanta, prefabCola, prefabCafe, prefabAgua, prefabVino, prefabCerveza });
        prefabListN2.AddRange(new List<GameObject> { prefabFanta, prefabCola, prefabCafe, prefabAgua, prefabVino, prefabCerveza, prefabPlatoPollo, prefabPlatoPizza, prefabPlatoTarta, prefabPlatoDesayuno, prefabPlatoHamburguesa });
        prefabListN4.AddRange(new List<GameObject> { prefabFanta, prefabCola, prefabCafe, prefabAgua, prefabVino, prefabCerveza,
                                                     prefabVino2, prefabAgua2, prefabZumo, prefabZumo2,
                                                     prefabPlatoPollo, prefabPlatoPizza, prefabPlatoTarta, prefabPlatoDesayuno, prefabPlatoHamburguesa,
                                                     prefabPlatoHamburguesa2, prefabPlatoPizza2, prefabPlatoTarta2, prefabPlatoAlbondigas });

        // Define similar pairs
        similarPairsN4.Add(new List<GameObject> { prefabVino, prefabVino2 });
        similarPairsN4.Add(new List<GameObject> { prefabAgua, prefabAgua2 });
        similarPairsN4.Add(new List<GameObject> { prefabZumo, prefabZumo2 });
        similarPairsN4.Add(new List<GameObject> { prefabPlatoHamburguesa, prefabPlatoHamburguesa2 });
        similarPairsN4.Add(new List<GameObject> { prefabPlatoPizza, prefabPlatoPizza2 });
        similarPairsN4.Add(new List<GameObject> { prefabPlatoTarta, prefabPlatoTarta2 });

        prefabListN6.AddRange(new List<GameObject> { prefabFanta, prefabCola, prefabCafe, prefabAgua, prefabVino, prefabCerveza, //Niveles 1-3
                                                     prefabVino2, prefabAgua2, prefabZumo, prefabZumo2, //Niveles 4-5
                                                     prefabVino3, prefabVino4, prefabAgua3, prefabAgua4, prefabZumo3, prefabZumo4, //Nivel 6
                                                     prefabPlatoPollo, prefabPlatoPizza, prefabPlatoTarta, prefabPlatoDesayuno, prefabPlatoHamburguesa, //Niveles 1-3
                                                     prefabPlatoHamburguesa2, prefabPlatoPizza2, prefabPlatoTarta2, prefabPlatoAlbondigas, //Niveles 4-5
                                                     prefabPlatoPizza3, prefabPlatoPizza4, prefabPlatoTarta3, prefabPlatoTarta4, prefabPlatoHamburguesa3, prefabPlatoHamburguesa4}); //Nivel 6

        similarPairsN6.Add(new List<GameObject> { prefabVino, prefabVino2, prefabVino3, prefabVino4 });
        similarPairsN6.Add(new List<GameObject> { prefabAgua, prefabAgua2, prefabAgua3, prefabAgua4 });
        similarPairsN6.Add(new List<GameObject> { prefabZumo, prefabZumo2, prefabZumo3, prefabZumo4 });
        similarPairsN6.Add(new List<GameObject> { prefabPlatoHamburguesa, prefabPlatoHamburguesa2, prefabPlatoHamburguesa3, prefabPlatoHamburguesa4 });
        similarPairsN6.Add(new List<GameObject> { prefabPlatoPizza, prefabPlatoPizza2, prefabPlatoPizza3, prefabPlatoPizza4 });
        similarPairsN6.Add(new List<GameObject> { prefabPlatoTarta, prefabPlatoTarta2, prefabPlatoTarta3, prefabPlatoTarta4 });
    }

    public GameObject FindPrefabByName(string name)
    {
        if (name.Contains("Cafe"))
        {
            return prefabCafe;
        }
        else if (name.Contains("cola"))
        {
            return prefabCola;
        }
        else if (name.Contains("soda_orange"))
        {
            return prefabFanta;
        }
        else if (name.Contains("Cerveza"))
        {
            return prefabCerveza;
        }
        else if (name.Contains("Agua"))
        {
            return prefabAgua;
        }
        else if (name.Contains("water_b2"))
        {
            return prefabAgua2;
        }
        else if (name.Contains("water_b3"))
        {
            return prefabAgua3;
        }
        else if (name.Contains("fiji_water"))
        {
            return prefabAgua4;
        }
        else if (name.Contains("rvine03"))
        {
            return prefabVino;
        }
        else if (name.Contains("wvine02"))
        {
            return prefabVino2;
        }
        else if (name.Contains("wvine03"))
        {
            return prefabVino3;
        }
        else if (name.Contains("rvine02"))
        {
            return prefabVino4;
        }
        else if (name.Contains("Zumo_Tomate"))
        {
            return prefabZumo;
        }
        else if (name.Contains("Zumo_Naranja"))
        {
            return prefabZumo2;
        }
        else if (name.Contains("Zumo_Manzana"))
        {
            return prefabZumo3;
        }
        else if (name.Contains("Zumo_Mora"))
        {
            return prefabZumo4;
        }
        else if (name.Contains("Chicken"))
        {
            return prefabPlatoPollo;
        }
        else if (name.Contains("Plate_Pizza"))
        {
            return prefabPlatoPizza;
        }
        else if (name.Contains("Plate2_Pizza"))
        {
            return prefabPlatoPizza2;
        }
        else if (name.Contains("Plate3_Pizza"))
        {
            return prefabPlatoPizza3;
        }
        else if (name.Contains("Plate4_Pizza"))
        {
            return prefabPlatoPizza4;
        }
        else if (name.Contains("PlateTarta"))
        {
            return prefabPlatoTarta;
        }
        else if (name.Contains("Plate2Tarta"))
        {
            return prefabPlatoTarta2;
        }
        else if (name.Contains("Plate3Tarta"))
        {
            return prefabPlatoTarta3;
        }
        else if (name.Contains("Plate4Tarta"))
        {
            return prefabPlatoTarta4;
        }
        else if (name.Contains("Toast"))
        {
            return prefabPlatoDesayuno;
        }
        else if (name.Contains("PlateBurger"))
        {
            return prefabPlatoHamburguesa;
        }
        else if (name.Contains("PlateMacBurger"))
        {
            return prefabPlatoHamburguesa2;
        }
        else if (name.Contains("PlateFishBurger"))
        {
            return prefabPlatoHamburguesa3;
        }
        else if (name.Contains("PlateDonnutBurger"))
        {
            return prefabPlatoHamburguesa4;
        }
        else
        {
            return null;
        }
    }
}
