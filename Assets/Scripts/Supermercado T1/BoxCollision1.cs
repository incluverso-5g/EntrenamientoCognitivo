using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class BoxCollision1 : MonoBehaviour
{
    private GameObject reponer;
    private Regex pattern = new Regex(@"box_(.+?)( \((\d+)\)| (\d+))?$");

    public float waitTime = 1f; // Time to wait in secondss
    private Coroutine objectCollisionWait = null;
    private bool can_collide = true;
    private class CollisionEntry
    {
        public Collider collider;
        public float startTime;
    }

    private Dictionary<Collider, bool> box_collision = new Dictionary<Collider, bool>();


    public GameObject Productos;
    public GameObject ProductosFalsos;
    public GameObject ProductosFalsosDiff;



    void Start()
    {
        reponer = GameObject.Find("ReponerLogic1");

        foreach (Transform child in Productos.transform)
        {
            originalParents[child.gameObject] = Productos.transform;
        }
        foreach (Transform child in ProductosFalsos.transform)
        {
            originalParents[child.gameObject] = ProductosFalsos.transform;
        }
        foreach (Transform child in ProductosFalsosDiff.transform)
        {
            originalParents[child.gameObject] = ProductosFalsosDiff.transform;
        }
    }

    void addCollision(Collider other)
    {
        //UnityEngine.Debug.Log("BOXCOLL first collision with " + other.transform.parent.ToString());
        if (!box_collision.ContainsKey(other))
        {
            box_collision.Add(other, true);
        }
        else
        {
            box_collision[other] = true;
        }
        
        //Print dict to check it
        //string str = "";
        //foreach(var key in box_collision.Keys)
        //{
        //    str += key.transform.parent.ToString() + ";";
        //}
        //UnityEngine.Debug.Log("BOXCOLL dict with " + str);
        StartCoroutine(CheckCollisionAfterWait(other));
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("box_collider"))
        {
            Transform box = other.gameObject.transform.parent;
            string collided_box = box.gameObject.name;
            //UnityEngine.Debug.Log(gameObject.name + " BOXCOLL Collision detected with " + collided_box);

            Match match = pattern.Match(collided_box);
            string collided_productName = match.Groups[1].Value;

            if (gameObject.name.Contains(collided_productName))
            {
                //UnityEngine.Debug.Log("Collision with box with same product name");
                if (!box.gameObject.transform.Find("products").gameObject.activeSelf)
                {
                    //UnityEngine.Debug.Log("Collision with box with same product name and no products");
                    Transform box_prod = box.gameObject.transform.Find("products");
                    gameObject.SetActive(false);
                    box_prod.gameObject.SetActive(true);
                    New_acierto();
                }
                else
                {
                    addCollision(other);
                }
            }
            else
            {
                addCollision(other);
            }

        }
        else if (other.gameObject.CompareTag("box"))
        {
            string collided_object = other.gameObject.name;
            //UnityEngine.Debug.Log(gameObject.name + " BOXCOLL Collision detected with " + collided_object);
            Match match = pattern.Match(collided_object);
            string collided_productName = match.Groups[1].Value;

            if (gameObject.name.Contains(collided_productName))
            {
                if (!other.gameObject.transform.Find("products").gameObject.activeSelf)
                {
                    Transform box_prod = other.gameObject.transform.Find("products");
                    gameObject.SetActive(false);
                    box_prod.gameObject.SetActive(true);
                    New_acierto();
                }
                else
                {
                    addCollision(other);
                }
            }
            else
            {
                addCollision(other);
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (box_collision.TryGetValue(other, out bool colliding))
        {
            box_collision[other] = false;
            //UnityEngine.Debug.Log("BOXCOLL not colliding anymore with " + other.transform.parent.ToString());
        }
    }

    private Dictionary<GameObject, Transform> originalParents = new Dictionary<GameObject, Transform>();


  

    private void OnCollisionEnter(Collision collision)
    {
        UnityEngine.Debug.Log($"Collisione rilevata con: {collision.gameObject.name} (parent: {collision.transform.parent?.name})");

        if (collision.gameObject.name.Contains("Product"))
        {
           
            if (originalParents.TryGetValue(collision.gameObject, out Transform originalParent))
            {
                
                UnityEngine.Debug.Log($"Collisione Genitore originale di {collision.gameObject.name}: {originalParent?.name}");

                
                if ((originalParent.name == Productos.name || originalParent.name == ProductosFalsos.name || originalParent.name == ProductosFalsosDiff.name)
                    && collision.gameObject.name.StartsWith("Product"))
                {
                    UnityEngine.Debug.Log("Collisione ignorata con: " + collision.gameObject.name);
                    return; 
                }
                else
                {
                    UnityEngine.Debug.Log($"Collisione Il genitore originale di {collision.gameObject.name} non è tra quelli da ignorare.");
                }
            }
            else
            {
                UnityEngine.Debug.LogWarning($"Collisione Genitore originale non trovato per {collision.gameObject.name}. Collisione non ignorata.");
            }



            if (can_collide)
            {
                can_collide = false;
                if (objectCollisionWait != null)
                {
                    StopCoroutine(objectCollisionWait);
                }
                objectCollisionWait = StartCoroutine(WaitForNextCollision());
            }
        }
    }


    private void New_error()
    {
        //UnityEngine.Debug.Log("BOXCOLL ERROR FLAGGED");
        reponer.GetComponent<ReponerLogic1>().new_fallo = true;
    }

    
    private void New_acierto()
    {
        //UnityEngine.Debug.Log("BOXCOLL SUCCESS FLAGGED");
        reponer.GetComponent<ReponerLogic1>().new_acierto = true;
    }


    private IEnumerator WaitForNextCollision()
    {
        New_error();
        can_collide = false;
        yield return new WaitForSecondsRealtime(2f);
        can_collide = true;
    }


    private IEnumerator CheckCollisionAfterWait(Collider other)
    {
        // Wait for the specified number of seconds
        yield return new WaitForSecondsRealtime(waitTime);

        // After the wait, check if the object is still colliding
        if (box_collision.TryGetValue(other, out bool colliding))
        {
            if (colliding)
            {
                New_error();
                //UnityEngine.Debug.Log("BOXCOLL error with " + other.transform.parent.ToString());
                foreach (Collider key in box_collision.Keys.ToList() )
                {
                    box_collision[key] = false;
                }
                //UnityEngine.Debug.Log("BOXCOLL set keys to false");
            }
        }
    }

}
