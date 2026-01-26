using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariablesComunes1_MesasJuntas : MonoBehaviour
{
    public int currentLevel;

    public Vector3[] posMesa1 = new Vector3[] { new Vector3(1.234f, 0.801f, 3.028f), new Vector3(1.533f, 0.801f, 3.222f), new Vector3(0.853f, 0.801f, 3.190f) };
    public Vector3[] rotMesa1 = new Vector3[] { new Vector3(-90, 0, 0), new Vector3(-90, -90, 0), new Vector3(-90, 90, 0) };

    public Vector4[] posMesa2 = new Vector4[] { new Vector3(0.121f, 0.801f, 1.418f), new Vector3(0.393f, 0.801f, 1.113f), new Vector3(0.74f, 0.801f, 1.416f), new Vector3(1.359f, 0.801f, -0.524f) };
    public Vector4[] rotMesa2 = new Vector4[] { new Vector3(-90, 90, 0), new Vector3(-90, 0, 0), new Vector3(-90, -90, 0), new Vector3(-90, 180, 0) };

    public Vector3[] posMesa3 = new Vector3[] { new Vector3(1.022f, 0.801f, -0.701f), new Vector3(1.44f, 0.801f, -0.792f), new Vector3(0.783f, 0.801f, -0.765f) };
    public Vector3[] rotMesa3 = new Vector3[] { new Vector3(-90, 90, 0), new Vector3(-90, 0, 0), new Vector3(-90, 180, 0) };

    public bool baja_nivel_entrenamiento;

    private GameObject WaveRig;
    LogSaver logSaver;

    private void Start()
    {
        currentLevel = 0;
        baja_nivel_entrenamiento = false;

        WaveRig = GameObject.Find("Wave Rig");
        logSaver = WaveRig.GetComponent<LogSaver>();
        logSaver.SetLogEvent("Nivel_" + currentLevel);
        
    }

    public GameObject CreateNewPlateRandomList(GameObject prefab, Vector3 posMesa, Vector3 rotMesa)
    {
        GameObject newPlato;

        if (prefab.name == "plato_sucio")
        {
            newPlato = Instantiate(prefab, posMesa, Quaternion.Euler(rotMesa));
            BoxCollider boxCollider = newPlato.AddComponent<BoxCollider>();
            boxCollider.size = new Vector3(0.2344088f, 0.2298172f, 0.02085057f);
            boxCollider.center = new Vector3(-0.002295926f, 0f, 0.01229661f);

            Rigidbody rigidBody = newPlato.AddComponent<Rigidbody>();

            newPlato.tag = "Plate";
        }
        else
        {
            newPlato = Instantiate(prefab, posMesa, Quaternion.Euler(0, rotMesa.y, rotMesa.z));
        }

        return newPlato;
    }

    public void ShuffleArraysV3(Vector3[] array1, Vector3[] array2)
    {
        for (int i = array1.Length - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);

            Vector3 temp1 = array1[i];
            array1[i] = array1[j];
            array1[j] = temp1;

            Vector3 temp2 = array2[i];
            array2[i] = array2[j];
            array2[j] = temp2;
        }
    }

    public void ShuffleArraysV4(Vector4[] array1, Vector4[] array2)
    {
        for (int i = array1.Length - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);

            Vector3 temp1 = array1[i];
            array1[i] = array1[j];
            array1[j] = temp1;

            Vector3 temp2 = array2[i];
            array2[i] = array2[j];
            array2[j] = temp2;
        }
    }
}
