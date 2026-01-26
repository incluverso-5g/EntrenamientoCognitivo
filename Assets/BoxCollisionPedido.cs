using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class BoxCollisionPedido : MonoBehaviour
{
    private GameObject pedir;
    private Regex pattern = new Regex(@"box_(.+?)( \((\d+)\)| (\d+))?$");
    private Regex estanteria_product_pattern = new Regex(@"[Pp]roduct_(\w+)(?: \(\d+\))?");
    private Regex bocadillo_product_pattern = new Regex(@"[Pp]roduct_(.+?)(?:\(Clone\))?$");
    public float waitTime = 0.1f; // Time to wait in secondss
    private Coroutine objectCollisionWait = null;
    private bool can_collide = true;
    private bool first_upd = true;
    private GameObject bocadillo;

    private class CollisionEntry
    {
        public Collider collider;
        public float startTime;
    }

    private Dictionary<Collider, bool> box_collision = new Dictionary<Collider, bool>();
    private Dictionary<int, GameObject> correctlyCollidedDict = new Dictionary<int, GameObject>();
    private List<int> correctlyCollidedObjects = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        pedir = GameObject.Find("PedidoLogic");
    }

    private void Update()
    {
        if (first_upd)
        {
            first_upd = false;
            Transform pedido = this.transform.parent.parent;
            for (int i = 0; i < pedido.childCount; i++)
            {
                string bocadilloName = pedido.GetChild(i).name;

                // Controlla se il nome corrisponde a uno dei bocadillo previsti
                if (bocadilloName == "Bocadillo" ||
                    bocadilloName == "BocadilloLeftSuper" ||
                    bocadilloName == "BocadilloCenterSuper")
                {
                    bocadillo = pedido.GetChild(i).gameObject;
                    UnityEngine.Debug.Log($"Bocadillo trovato: {bocadilloName} per la scatola '{gameObject.name}'");
                    break;
                }
            }

            if (bocadillo == null)
            {
                UnityEngine.Debug.LogError($"Bocadillo non trovato per la scatola '{gameObject.name}'");
            }
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
        //UnityEngine.Debug.Log("BOXCOLL dict with " + str);
        StartCoroutine(CheckCollisionAfterWait(other));
    }

    private void OnTriggerEnter(Collider other)
    {
        //UnityEngine.Debug.Log(gameObject.name + " BOXCOLL Collision detected with " + other.gameObject.name);
        if (other.gameObject.CompareTag("DynamicObject") && !correctlyCollidedObjects.Contains(other.gameObject.GetInstanceID()))
        {
            //UnityEngine.Debug.Log(" BOXCOLL Collision detected with DynamicObj");
            string colliding_product = other.gameObject.name;
            UnityEngine.Debug.Log(gameObject.name + " BOXCOLL Collision detected with " + colliding_product + " which is dynamicObject="+ other.CompareTag("DynamicObject"));
            Match match = estanteria_product_pattern.Match(colliding_product);
            
            string colliding_productName = match.Groups[1].Value;
            UnityEngine.Debug.Log(gameObject.name + " BOXCOLL Collision detected with REGEX " + colliding_productName);
            bool corr = false;

            for (int i = 0; i < bocadillo.transform.childCount; i++)
            {
                string ped_prod = bocadillo.transform.GetChild(i).name;
                if (bocadillo.transform.GetChild(i).GameObject().activeSelf)
                {
                    Match match2 = bocadillo_product_pattern.Match(ped_prod);
                    string ped_prodName = match2.Groups[1].Value;
                    //UnityEngine.Debug.Log("bocad prod " + ped_prod);
                    if (ped_prodName == colliding_productName)
                    {
                        UnityEngine.Debug.Log("match between " + ped_prodName + " and " + colliding_productName);
                        corr = true;
                        bocadillo.transform.GetChild(i).GameObject().SetActive(false);
                        correctlyCollidedObjects.Add(other.gameObject.GetInstanceID());
                        correctlyCollidedDict.Add(other.gameObject.GetInstanceID(), other.gameObject);
                        //other.GameObject().SetActive(false);
                        StartCoroutine(AddProductAsChild(other));
                        New_acierto();    
                        break;
                    }
                }      
            }
            ComprobaPedidoCompleto();
            if (!corr)
            {
                UnityEngine.Debug.Log("No match at all with " + colliding_productName);
                Destroy(other.gameObject, 2f);
                addCollision(other);
            }
        }
        else
        {
            UnityEngine.Debug.Log("NOT counting collision with " + other.gameObject.name);
        }
    }

    private IEnumerator AddProductAsChild(Collider product)
    {
        yield return new WaitForSecondsRealtime(0.5f);
        product.transform.SetParent(this.transform);
    }

    private void OnTriggerExit(Collider other)
    {
        if (box_collision.TryGetValue(other, out bool colliding))
        {
            box_collision[other] = false;
            //UnityEngine.Debug.Log("BOXCOLL not colliding anymore with " + other.transform.parent.ToString());
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Contains("Product"))
        {
            //UnityEngine.Debug.Log("BOXCOLL oncollisionEnter with " + collision.gameObject.name);
            if (can_collide)
            {
                //UnityEngine.Debug.Log("BOXCOLL oncollisionEnter can collide with " + collision.gameObject.name);
                can_collide = false;
                if (objectCollisionWait != null)
                {
                    StopCoroutine(objectCollisionWait);
                }
                objectCollisionWait = StartCoroutine(WaitForNextCollision());
            }
        }
    }

  
    private void ComprobaPedidoCompleto()
    {
        UnityEngine.Debug.Log($"[ComprobaPedidoCompleto] Checking bocadillo '{bocadillo.name}'...");

        int activeProds = 0;
        for (int i = 0; i < bocadillo.transform.childCount; i++)
        {
            var child = bocadillo.transform.GetChild(i);
            if (child.gameObject.activeSelf && !child.name.Contains("Canvas"))
            {
                activeProds++;
                UnityEngine.Debug.Log($"[ComprobaPedidoCompleto] Active product: {child.name}");
            }
        }

        UnityEngine.Debug.Log($"[ComprobaPedidoCompleto] Active products count: {activeProds}");

        if (activeProds == 0)
        {
            UnityEngine.Debug.Log($"[ComprobaPedidoCompleto] Bocadillo '{bocadillo.name}' is EMPTY.");
            pedir.GetComponent<PedidoLogic>().new_pedido_completado = true;

            ClearActiveChildrenDynamicObjects(this.transform);
            //StartCoroutine(DeactivatePedidoAfterDelay(1.2f));
            //UnityEngine.Debug.Log($"[ComprobaPedidoCompleto] Pedido '{pedido.name}' DISATTIVATO.");
        }
    }

    private IEnumerator DeactivatePedidoAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        GameObject pedido = this.transform.parent.parent.gameObject;
        pedido.SetActive(false);
    }


    private void ClearActiveChildrenDynamicObjects(Transform parent)
    {
        foreach(int id in correctlyCollidedObjects)
        {
            GameObject go;
            if(correctlyCollidedDict.TryGetValue(id, out go))
            {
                UnityEngine.Debug.Log($"DESTROY GameObject with name {go.name}");
                Destroy(go, 0.7f);
                correctlyCollidedDict.Remove(id);
            }
        }

        //GameObject[] allobjects = GameObject.FindGameObjectsWithTag("DynamicObject");
        //foreach(GameObject go in allobjects)
        //{
        //    if (correctlyCollidedObjects.Contains(go.GetInstanceID()))
        //   {
        //        UnityEngine.Debug.Log($"FOUND GameObject with name {go.name}");
        //        Destroy(go, 1f);
        //    }
        //}
    }

    /* private void ComprobaPedidoCompleto()
     {

         UnityEngine.Debug.Log($"PEDIDO [ComprobaPedidoCompleto] Checking bocadillo '{bocadillo.name}'...");
         int activeProds = 0;
         for (int i = 0; i < bocadillo.transform.childCount; i++)
         {
             if (bocadillo.transform.GetChild(i).gameObject.activeSelf)
             {
                 activeProds++;
             }
         }
         UnityEngine.Debug.Log("Pedido active prods : " + activeProds.ToString());
         if (activeProds == 0)
         {
             pedir.GetComponent<PedidoLogic>().new_pedido_completado = true;
             UnityEngine.Debug.Log("Pedido COMPLETADOOOO");
             GameObject pedido = this.transform.parent.parent.gameObject;
             //StartCoroutine(WaitToDeactivated(pedido, 1f));
             pedido.SetActive(false);
             bocadillo.SetActive(false);

             pedir.GetComponent<PedidoLogic>().new_pedido_completado = true;
         }
     }*/

    public void OnDisable()
    {
        ClearActiveChildrenDynamicObjects(this.transform);
    }

    private void New_error()
    {
        //UnityEngine.Debug.Log("BOXCOLL ERROR FLAGGED");
        pedir.GetComponent<PedidoLogic>().new_fallo = true;
    }


    private void New_acierto()
    {
        //UnityEngine.Debug.Log("BOXCOLL SUCCESS FLAGGED");
        pedir.GetComponent<PedidoLogic>().new_acierto = true;
    }

   /* private IEnumerator WaitToDeactivated(GameObject toDeactivate, float waitTime)
    {
        
        
        yield return new WaitForSecondsRealtime(waitTime);
        toDeactivate.SetActive(false);
      }*/


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
                foreach (Collider key in box_collision.Keys.ToList())
                {
                    box_collision[key] = false;
                }
                //UnityEngine.Debug.Log("BOXCOLL set keys to false");
            }
        }
    }

}
