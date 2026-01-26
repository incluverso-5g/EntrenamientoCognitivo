using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VariablesNiveles : MonoBehaviour
{   
    //public static VariablesNiveles instance;
    public int nivel_cafeteria_T1;
    public int nivel_cafeteria_T2;

    public int nivel_supermercado_T1;
    public int nivel_supermercado_T2;

    public bool reiniciaNivel;
    public bool lanzaError;
    public bool lanzaAcierto;

    public bool lanzaInstrucciones;

    public GameObject grabbedObject;

    public int erroresNivel;

    public bool objetosChocan;
    public bool Separadores;

    public int corazones;

    public string videoRelax;

    public bool Reset = false;
    public bool Recoloca = false;

    private void Awake()
    {   
       
         nivel_supermercado_T2 = 4;
         nivel_supermercado_T1 = 5;
        /*
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }*/


        erroresNivel = 0;
        corazones = 3;

        reiniciaNivel = false;
        lanzaError = false;
        lanzaAcierto = false;
        lanzaInstrucciones = false;
        objetosChocan = false;
        Separadores = false;

        nivel_cafeteria_T1 = 6;
        nivel_cafeteria_T2 = 6;
    }

    //private void Update()
    //{
        //Debug.Log("Separadores: " + Separadores);
        //Quiero usar una variable que se llama corazones y no erroresNivel
        //corazones = erroresNivel;

        /*
        print("Nivel caf T1" + nivel_cafeteria_T1);
        print("Nivel caf T2" + nivel_cafeteria_T2);
        print("Nivel sup T1" + nivel_supermercado_T1);
        print("Nivel sup T2" + nivel_supermercado_T2);
        */
    //}
}
