using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuncionesNoUtilizo : MonoBehaviour
{
    public void CreateNewPlate(GameObject prefab, int posicion, Vector3[] posMesa, Vector3[] rotMesa)
    {
        GameObject newPlato;

        if (prefab.name == "plato_sucio")
        {
            newPlato = Instantiate(prefab, posMesa[posicion], Quaternion.Euler(rotMesa[posicion]));
            BoxCollider boxCollider = newPlato.AddComponent<BoxCollider>();
            boxCollider.size = new Vector3(0.2344088f, 0.2298172f, 0.02085057f);
            boxCollider.center = new Vector3(-0.002295926f, 0f, 0.01229661f);

            Rigidbody rigidBody = newPlato.AddComponent<Rigidbody>();

            newPlato.tag = "Plate";
        }
        else
        {
            Instantiate(prefab, posMesa[posicion], Quaternion.Euler(0, rotMesa[posicion].y, rotMesa[posicion].z));
        }

        //return newPlato;
    }

    public void CreateNewPlateVector4(GameObject prefab, int posicion, Vector4[] posMesa, Vector4[] rotMesa)
    {
        GameObject newPlato;

        if (prefab.name == "plato_sucio")
        {
            newPlato = Instantiate(prefab, posMesa[posicion], Quaternion.Euler(rotMesa[posicion]));
            BoxCollider boxCollider = newPlato.AddComponent<BoxCollider>();
            boxCollider.size = new Vector3(0.2344088f, 0.2298172f, 0.02085057f);
            boxCollider.center = new Vector3(-0.002295926f, 0f, 0.01229661f);

            Rigidbody rigidBody = newPlato.AddComponent<Rigidbody>();

            newPlato.tag = "Plate";
        }
        else
        {
            Instantiate(prefab, posMesa[posicion], Quaternion.Euler(0, rotMesa[posicion].y, rotMesa[posicion].z));
        }

        /*
        GameObject newPlato = Instantiate(prefab, posMesa[posicion], Quaternion.Euler(rotMesa[posicion]));

        BoxCollider boxCollider = newPlato.AddComponent<BoxCollider>();
        boxCollider.size = new Vector3(0.2344088f, 0.2298172f, 0.02085057f);
        boxCollider.center = new Vector3(-0.002295926f, 0f, 0.01229661f);

        Rigidbody rigidBody = newPlato.AddComponent<Rigidbody>();

        newPlato.tag = "Plate";
        //return newPlato;
        */
    }

    public Vector3[] ShuffleArrayV3(Vector3[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            Vector3 temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
        return array;
    }

    public Vector4[] ShuffleArrayV4(Vector4[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            Vector3 temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
        return array;
    }
}
