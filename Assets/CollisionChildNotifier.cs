using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionChildNotifier : MonoBehaviour
{
    private CheckCollisionNiveles_Tarea2 parentScript;

    void Start()
    {
        parentScript = GetComponentInParent<CheckCollisionNiveles_Tarea2>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (parentScript != null)
        {
            parentScript.NotifyCollision(gameObject, collision);
        }
    }
}
