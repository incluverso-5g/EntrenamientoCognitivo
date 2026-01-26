using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Wave.Essence;

public class CheckCollisionNiveles_Tarea2 : MonoBehaviour
{
    private GameObject cambioNivel;
    // Nos referimos a los otros scripts:
    private VariablesComunes_T2 variablesComunes;
    private GameObject variablesNivel;
    private Cafeteria_Tarea2 cafeteria;
    ListaPrefabsNiveles listaPrefabsNiveles;

    // Colisiones / Interacciones de las bebidas con los platos:
    public GameObject objetoColision;
    public GameObject collisionFatherM1 = null;
    public GameObject collisionFatherM2 = null;
    public GameObject collisionFatherM3 = null;

    // Variables para saber en qu fase nos encontramos, y modificaciones de ellas:
    private int platosServidos;

    // Variables para errores:
    public int errores_repeticion;
    private bool lanzarError;
    private bool errorContado;

    // Variables para bebidas:
    private HashSet<GameObject> countedDrinks = new HashSet<GameObject>();
    private Dictionary<GameObject, float> drinksTimers = new Dictionary<GameObject, float>();

    // Otros:
    private GameObject WaveRig;
    LogSaver logSaver;
    private bool lanzarNivelFinalizado = false;

    private bool aciertoChecked;

    VariablesNiveles variablesNiveles;

    private string grabbedObjectName;

    void Start()
    {
        WaveRig = GameObject.Find("Wave Rig");
        logSaver = WaveRig.GetComponent<LogSaver>();
        variablesNiveles = WaveRig.GetComponent<VariablesNiveles>();

        cambioNivel = GameObject.Find("CambioNivel");
        variablesComunes = cambioNivel.GetComponent<VariablesComunes_T2>();

        listaPrefabsNiveles = cambioNivel.GetComponent<ListaPrefabsNiveles>();

        aciertoChecked = false;
        errorContado = false;
        lanzarError = false;

        platosServidos = 0;
    }

    void Update()
    {
        grabbedObjectName = variablesComunes.grabbedObject != null ? variablesComunes.grabbedObject.name : "None";
        //Debug.Log("Grabbed object is: " + variablesComunes.grabbedObject.name);

        if (variablesNivel == null)
        {
            variablesNivel = GameObject.Find("CambioNivel");
            cafeteria = variablesNivel.GetComponent<Cafeteria_Tarea2>();
        }

        if (variablesNiveles.reiniciaNivel == true)
        {
            ReiniciarNivel();
            variablesNiveles.reiniciaNivel = false;
        }
        else if (variablesNiveles.lanzaError)
        {
            Debug.Log("Entra en este punto 1");
            variablesComunes.errores += 1;
            //errores_repeticion += 1;
            variablesComunes.audioSource_error.Play();
            StartCoroutine(LaunchError());
            variablesNiveles.lanzaError = false;
        }

        if (variablesNiveles.Recoloca == true) 
        {
            Debug.Log("Entra en este punto 2"); 
            ReinstanciarTodosEnBarra(); 
            variablesNiveles.Recoloca = false;
        }

        if (variablesComunes.currentLevel == 1)  //&& variablesComunes.grabbedObject != null)
        {
            if (variablesComunes.grabbedObject != null)
            {
                Debug.Log("Grabbed object is: " + variablesComunes.grabbedObject.name);
                if (variablesComunes.instantiatedDrinksBocadilloM1 != null &&  cafeteria.bocadillo_M1.activeSelf)
                {
                    CheckGrabbedObject(variablesComunes.instantiatedDrinksBocadilloM1);
                    Debug.Log("e1");
                }
                else if (variablesComunes.instantiatedDrinksBocadilloM2 != null && cafeteria.bocadillo_M2.activeSelf)
                {
                    CheckGrabbedObject(variablesComunes.instantiatedDrinksBocadilloM2);
                    Debug.Log("e2");
                }
                else if (variablesComunes.instantiatedDrinksBocadilloM3 != null && cafeteria.bocadillo_M3.activeSelf)
                {
                    CheckGrabbedObject(variablesComunes.instantiatedDrinksBocadilloM3);
                    Debug.Log("e3");
                }
            }
            else
            {
                // Reset state if no object is grabbed
                errorContado = false;
                lanzarError = false;
            }

            if (collisionFatherM1 != null)
            {
                CheckAcierto(variablesComunes.instantiatedDrinksBocadilloM1, cafeteria.selectedColliderM1, collisionFatherM1);
            }
            if (collisionFatherM2 != null)
            {
                CheckAcierto(variablesComunes.instantiatedDrinksBocadilloM2, cafeteria.selectedColliderM2, collisionFatherM2);
            }
            if (collisionFatherM3 != null)
            {
                CheckAcierto(variablesComunes.instantiatedDrinksBocadilloM3, cafeteria.selectedColliderM3, collisionFatherM3);
            }

            if (variablesComunes.faseActual < variablesComunes.fases_max)
            {
                if (platosServidos == variablesComunes.platos_a_servir)
                {
                    //Debug.Log("Entra en todos los platos servidos");
                    variablesComunes.faseActual += 1;
                    platosServidos = 0;
                }
            }

            if (variablesComunes.faseActual == variablesComunes.fases_max)
            {
                if (!lanzarNivelFinalizado)
                {
                    StartCoroutine(LaunchFinalNivel());
                }
            }
        }

        if (variablesComunes.currentLevel == 2) // && variablesComunes.grabbedObject != null)
        {
            if (variablesComunes.grabbedObject != null)
            {
                if (variablesComunes.instantiatedDrinksBocadilloM1 != null)
                {
                    CheckGrabbedObject(variablesComunes.instantiatedDrinksBocadilloM1);
                }
                else if (variablesComunes.instantiatedDrinksBocadilloM2 != null)
                {
                    CheckGrabbedObject(variablesComunes.instantiatedDrinksBocadilloM2);
                }
                else if (variablesComunes.instantiatedDrinksBocadilloM3 != null)
                {
                    CheckGrabbedObject(variablesComunes.instantiatedDrinksBocadilloM3);
                }
            }
            else
            {
                // Reset state if no object is grabbed
                errorContado = false;
                lanzarError = false;
            }

            if (variablesComunes.instantiatedDrinksBocadilloM1 != null && variablesComunes.instantiatedDrinksBocadilloM2 != null)
            {
                if (collisionFatherM1 != null)
                {
                    CheckAcierto(variablesComunes.instantiatedDrinksBocadilloM1, cafeteria.selectedColliderM1, collisionFatherM1);
                }

                if (collisionFatherM2 != null)
                {
                    CheckAcierto(variablesComunes.instantiatedDrinksBocadilloM2, cafeteria.selectedColliderM2, collisionFatherM2);
                }
            }
            if (variablesComunes.instantiatedDrinksBocadilloM1 != null && variablesComunes.instantiatedDrinksBocadilloM3 != null)
            {
                if (collisionFatherM1 != null)
                {
                    CheckAcierto(variablesComunes.instantiatedDrinksBocadilloM1, cafeteria.selectedColliderM1, collisionFatherM1);
                }
                if (collisionFatherM3 != null)
                {
                    CheckAcierto(variablesComunes.instantiatedDrinksBocadilloM3, cafeteria.selectedColliderM3, collisionFatherM3);
                }
            }
            if (variablesComunes.instantiatedDrinksBocadilloM2 != null && variablesComunes.instantiatedDrinksBocadilloM3 != null)
            {
                if (collisionFatherM2 != null)
                {
                    CheckAcierto(variablesComunes.instantiatedDrinksBocadilloM2, cafeteria.selectedColliderM2, collisionFatherM2);
                }
                if (collisionFatherM3 != null)
                {
                    CheckAcierto(variablesComunes.instantiatedDrinksBocadilloM3, cafeteria.selectedColliderM3, collisionFatherM3);
                }
            }

            //Debug.Log("Fase actual: " + variablesComunes.faseActual);

            if (variablesComunes.faseActual < variablesComunes.fases_max)
            {
                if (platosServidos == variablesComunes.platos_a_servir)
                {
                    //Debug.Log("Entra en todos los platos servidos");
                    variablesComunes.faseActual += 1;
                    platosServidos = 0;
                }
            }

            if (variablesComunes.faseActual == variablesComunes.fases_max)
            {
                if (!lanzarNivelFinalizado)
                {
                    StartCoroutine(LaunchFinalNivel());
                }
            }
        }

        if (variablesComunes.currentLevel == 3 || variablesComunes.currentLevel == 4)
        {
            if (variablesComunes.grabbedObject != null)
            {
                CheckGrabbedObject(variablesComunes.instantiatedDrinksBocadilloM1);
                CheckGrabbedObject(variablesComunes.instantiatedDrinksBocadilloM2);
                CheckGrabbedObject(variablesComunes.instantiatedDrinksBocadilloM3);
            }
            else
            {
                errorContado = false;
                lanzarError = false;
            }

            if (collisionFatherM1 != null)
            {
                CheckAcierto(variablesComunes.instantiatedDrinksBocadilloM1, cafeteria.selectedColliderM1, collisionFatherM1);
            }
            if (collisionFatherM2 != null)
            {
                CheckAcierto(variablesComunes.instantiatedDrinksBocadilloM2, cafeteria.selectedColliderM2, collisionFatherM2);
            }
            if (collisionFatherM3 != null)
            {
                CheckAcierto(variablesComunes.instantiatedDrinksBocadilloM3, cafeteria.selectedColliderM3, collisionFatherM3);
            }

            if (variablesComunes.faseActual < variablesComunes.fases_max)
            {
                if (platosServidos == variablesComunes.platos_a_servir)
                {
                    variablesComunes.faseActual += 1;
                    platosServidos = 0;
                }
            }

            if (variablesComunes.faseActual == variablesComunes.fases_max)
            {
                if (!lanzarNivelFinalizado)
                {
                    StartCoroutine(LaunchFinalNivel());
                }
            }
        }

        if (variablesComunes.currentLevel >= 5)
        {
            if (variablesComunes.grabbedObject != null)
            {
                CheckGrabbedObject(variablesComunes.instantiatedDrinksBocadilloM1);
                CheckGrabbedObject(variablesComunes.instantiatedDrinksBocadilloM2);
                CheckGrabbedObject(variablesComunes.instantiatedDrinksBocadilloM3);
            }
            else
            {
                errorContado = false;
                lanzarError = false;
            }

            // ACIERTOS
            if (collisionFatherM1 != null)
            {
                Check2Aciertos(variablesComunes.instantiatedDrinksBocadilloM1, cafeteria.selectedColliderM1, collisionFatherM1);
            }
            if (collisionFatherM2 != null)
            {
                Check2Aciertos(variablesComunes.instantiatedDrinksBocadilloM2, cafeteria.selectedColliderM2, collisionFatherM2);
            }
            if (collisionFatherM3 != null)
            {
                Check2Aciertos(variablesComunes.instantiatedDrinksBocadilloM3, cafeteria.selectedColliderM3, collisionFatherM3);
            }
            // 

            if (variablesComunes.faseActual < variablesComunes.fases_max)
            {
                if (platosServidos == variablesComunes.platos_a_servir)
                {
                    variablesComunes.faseActual += 1;
                    platosServidos = 0;
                }
            }

            if (variablesComunes.faseActual == variablesComunes.fases_max)
            {
                if (!lanzarNivelFinalizado)
                {
                    StartCoroutine(LaunchFinalNivel());
                }
            }
        }
    }

    private void Check2Aciertos(List<GameObject> instantiatedObjectsBocadillo, GameObject selectedColliderMesa, GameObject collisionFatherMesa)
    {
        if (instantiatedObjectsBocadillo.Count > 0 && objetoColision != null)
        {
            if (selectedColliderMesa == collisionFatherMesa)
            {
                GameObject matchingObject = instantiatedObjectsBocadillo.Find(obj => objetoColision.name.Contains(obj.name));
                if( matchingObject != null)
                {
                    Debug.Log("Entra en este punto 2");
                    instantiatedObjectsBocadillo.Remove(matchingObject);
                    objetoColision.SetActive(false);
                    objetoColision = null;
                    
                    ReinstanciarEnBarra(matchingObject);

                    aciertoChecked = true;

                    StartCoroutine(HandleCollision(matchingObject)); 

                    if(instantiatedObjectsBocadillo.Count == 0)
                    {
                        StartCoroutine(DeactivateAfterDelay(matchingObject, collisionFatherMesa, GetBocadilloFromCollider(collisionFatherMesa)));
                    }
                    else
                    {
                        UpdateBocadillo(instantiatedObjectsBocadillo, collisionFatherMesa);
                    }

                    collisionFatherM1 = null;
                    collisionFatherM2 = null;
                    collisionFatherM3 = null;
                }
                else
                {
                    Debug.Log("Entra en este punto 3");
                    HandleError(); 
                }
            }
        }
        else
        {
            aciertoChecked = false; 
            errorContado = false;   
        }
    }

    private IEnumerator ResetGrabbedObject(GameObject grabbedObject)
    {
        if (grabbedObject == null) yield break;

        // Espera 2 segundos antes de reinstanciar
        yield return new WaitForSeconds(2f);

        // Desactiva el objeto actual
        grabbedObject.SetActive(false);

        // Llama al método que ya funciona para reinstanciar
        ReinstanciarEnBarra(grabbedObject);

        Debug.Log($"Objeto {grabbedObject.name} reinstanciado correctamente en la barra.");
    }

    private GameObject GetBocadilloFromCollider(GameObject collider)
    {
        if (collider == cafeteria.selectedColliderM1) return cafeteria.bocadillo_M1;
        if (collider == cafeteria.selectedColliderM2) return cafeteria.bocadillo_M2;
        if (collider == cafeteria.selectedColliderM3) return cafeteria.bocadillo_M3;
        return null;
    }

    private void ReinstanciarEnBarra(GameObject objeto)
    {
        // Busca un prefab que coincida con el objeto entregado
        GameObject prefab = listaPrefabsNiveles.FindPrefabByName(objeto.name);
        if (prefab != null)
        {
            foreach (Vector3 posicion in variablesComunes.posBarra)
            {
                if (!IsPositionOccupied(posicion))
                {
                    Quaternion rotacionInicial;

                    // Verifica si la rotación inicial está guardada
                    if (variablesComunes.objetosPosicionesIniciales.TryGetValue(objeto, out var datosIniciales))
                    {
                        rotacionInicial = datosIniciales.Item2; // Usar la rotación guardada
                    }
                    else
                    {
                        rotacionInicial = prefab.transform.rotation; // Usar la rotación del prefab si no hay guardada
                    }

                    // Reinstancia el objeto en una posición libre de la barra con la rotación adecuada
                    GameObject nuevoObjeto = variablesComunes.CreateNewPlateRandomList(prefab, posicion, rotacionInicial.eulerAngles);
                    variablesComunes.instantiatedDrinksBarra.Add(nuevoObjeto);
                    Debug.Log($"Objeto reinstanciado en la barra: {nuevoObjeto.name} con rotación: {rotacionInicial.eulerAngles}");
                    break;
                }
            }
        }
    }

    public void ReinstanciarTodosEnBarra()
    {
        List<GameObject> objetosAReinstanciar = new List<GameObject>();

        foreach (var entry in variablesComunes.objetosPosicionesIniciales)
        {
            GameObject objetoOriginal = entry.Key;
            bool existeEnBarra = variablesComunes.instantiatedDrinksBarra.Exists(
                inst => inst.name.Contains(objetoOriginal.name)
            );

            if (existeEnBarra)
            {
                objetosAReinstanciar.Add(objetoOriginal);
            }
        }

        // Desactivar y limpiar las instancias actuales de la barra
        foreach (GameObject obj in variablesComunes.instantiatedDrinksBarra)
        {
            obj.SetActive(false);
        }
        variablesComunes.instantiatedDrinksBarra.Clear();

        // Reinstanciar cada objeto
        foreach (GameObject objetoOriginal in objetosAReinstanciar)
        {
            ReinstanciarEnBarra(objetoOriginal);
        }

        // ✅ Comprobación de solapamientos
        ComprobarSolapamientosEnBarra();

        Debug.Log($"Se han reinstanciado {objetosAReinstanciar.Count} objetos en la barra.");
    }

    private void ComprobarSolapamientosEnBarra()
    {
        float distanciaMinima = 0.05f; // Distancia para considerar que hay solapamiento

        List<GameObject> objetos = variablesComunes.instantiatedDrinksBarra;

        for (int i = 0; i < objetos.Count; i++)
        {
            for (int j = i + 1; j < objetos.Count; j++)
            {
                if (objetos[i] != null && objetos[j] != null)
                {
                    float distancia = Vector3.Distance(objetos[i].transform.position, objetos[j].transform.position);
                    if (distancia < distanciaMinima)
                    {
                        Debug.LogWarning($"Solapamiento detectado entre {objetos[i].name} y {objetos[j].name}");

                        // Buscar una posición libre para mover el segundo objeto
                        bool reubicado = false;
                        foreach (Vector3 posicionLibre in variablesComunes.posBarra)
                        {
                            if (!IsPositionOccupied(posicionLibre))
                            {
                                objetos[j].transform.position = posicionLibre;
                                reubicado = true;
                                Debug.Log($"Reubicado {objetos[j].name} en {posicionLibre}");
                                break;
                            }
                        }

                        // Si no hay espacio, desactivar el objeto
                        if (!reubicado)
                        {
                            objetos[j].SetActive(false);
                            Debug.LogWarning($"No se pudo reubicar {objetos[j].name}, se desactiva.");
                        }
                    }
                }
            }
        }
    }

    private void UpdateBocadillo(List<GameObject> remainingObjects, GameObject collisionFatherMesa)
    {
        GameObject bocadillo = GetBocadilloFromCollider(collisionFatherMesa);
        if (bocadillo != null && remainingObjects.Count > 0)
        {
            GameObject nextObject = remainingObjects[0];
            // Actualiza la representación visual del bocadillo (si corresponde)
            nextObject.SetActive(true);
        }
    }

    private bool bloqueoTemporal = false; 

    private void CheckGrabbedObject(List<GameObject> instantiatedDrinksBocadillo)
    {
        if (bloqueoTemporal) return; 

        if (variablesComunes.currentLevel == 1)
        {
            bool matchFound = false;

            foreach (GameObject obj in instantiatedDrinksBocadillo)
            {
                Debug.Log("Object in bocadillo: " + obj.name);
                if (variablesComunes.grabbedObject.name.Contains(obj.name))
                {
                    Debug.Log("Acierto");
                    matchFound = true;
                    break;
                }
            }

            if (!matchFound && !errorContado)
            {
                lanzarError = true;
                variablesComunes.errores += 1;
                if (variablesComunes.grabbedObject != null) 
                { 
                // Llama a la corutina para reinstanciar el objeto
                StartCoroutine(ResetGrabbedObject(variablesComunes.grabbedObject));
                }
                logSaver.SetLogEvent("Lanza_Error");               
                StartCoroutine(LaunchError());
                errorContado = true; // Ensure subsequent calls don't count the error again
            }
            else if (matchFound)
            {
                errorContado = false; // Reset errorContado if the match is found
            }
        }

        if (variablesComunes.currentLevel >= 2)
        {
            List<GameObject> bocadillos = new List<GameObject>();

            // Add all instantiated objects from bocadillo_M1 if it's active
            if (variablesComunes.instantiatedDrinksBocadilloM1 != null && cafeteria.bocadillo_M1.activeSelf)
            {
                bocadillos.AddRange(variablesComunes.instantiatedDrinksBocadilloM1);
            }

            // Add all instantiated objects from bocadillo_M2 if it's active
            if (variablesComunes.instantiatedDrinksBocadilloM2 != null && cafeteria.bocadillo_M2.activeSelf)
            {
                bocadillos.AddRange(variablesComunes.instantiatedDrinksBocadilloM2);
            }

            // Add all instantiated objects from bocadillo_M3 if it's active
            if (variablesComunes.instantiatedDrinksBocadilloM3 != null && cafeteria.bocadillo_M3.activeSelf)
            {
                bocadillos.AddRange(variablesComunes.instantiatedDrinksBocadilloM3);
            }


            bool matchFound = false;

            foreach (GameObject bocadillo in bocadillos)
            {
                //Debug.Log("Bebidas instanciadas: " + bocadillo);

                if (bocadillo != null && variablesComunes.grabbedObject.name == bocadillo.name)
                {
                    Debug.Log("Acierto");
                    matchFound = true;
                    break;
                }
            }

            if (!matchFound && !errorContado)
            {
                Debug.Log("Creo que es este!");
                lanzarError = true;
                variablesComunes.errores += 1;
                if (variablesComunes.grabbedObject != null)
                {
                    // Llama a la corutina para reinstanciar el objeto
                    StartCoroutine(ResetGrabbedObject(variablesComunes.grabbedObject));
                }
                logSaver.SetLogEvent("Lanza_Error");
                StartCoroutine(LaunchError());
                errorContado = true; // Ensure subsequent calls don't count the error again
            }
            else if (matchFound)
            {
                errorContado = false; // Reset errorContado if the match is found
            }
        }
    }

    private IEnumerator BloqueoTemporal(float delay)
    {
        bloqueoTemporal = true; 
        yield return new WaitForSeconds(delay);
        bloqueoTemporal = false; 
    }


    public void NotifyCollision(GameObject child, Collision collision)
    {
        if (child == cafeteria.selectedColliderM1 ||
            child == cafeteria.selectedColliderM2 ||
            child == cafeteria.selectedColliderM3)
        {
            //Debug.Log("Entra aqui");

            if (child.name.Contains("ColisionPlatosM1"))
            {
                collisionFatherM1 = child;
                //Debug.Log("CollisionFather: " + collisionFatherM1);
            }
            else if (child.name.Contains("ColisionPlatosM2"))
            {
                collisionFatherM2 = child;
                //Debug.Log("CollisionFather: " + collisionFatherM2);
            }
            else if (child.name.Contains("ColisionPlatosM3"))
            {
                collisionFatherM3 = child;
                //Debug.Log("CollisionFather: " + collisionFatherM3);
            }

            objetoColision = collision.gameObject;
        }
    }

    private void ResetBebidasPosition(List<GameObject> gameobjects, Vector3 posMesa, Vector3 rotMesa)
    {
        if (gameobjects.Count > 0)
        {
            foreach (GameObject drink in gameobjects)
            {
                drink.SetActive(false);
            }
        }
    }

    private bool aciertoContado = false; 
    IEnumerator HandleCollision(GameObject drink)
    {
        if (!aciertoContado)
        {
            StartCoroutine(BloqueoTemporal(1.5f));
            variablesComunes.LaunchSuccess();
            variablesComunes.audioSource_ok.Play();
            aciertoContado = true; 
        }
        

        if (!drinksTimers.ContainsKey(drink))
        {
            drinksTimers[drink] = Time.time + 1.0f;
        }

        yield return new WaitUntil(() => Time.time >= drinksTimers[drink]);

        if (!countedDrinks.Contains(drink))
        {
            logSaver.SetLogEvent("Lanza_Acierto");
            countedDrinks.Add(drink);
            drink.SetActive(false);
            variablesComunes.tick.SetActive(false);
            variablesComunes.audioSource_ok.Stop();
            //collisionObject = null;
            platosServidos += 1;
            Debug.Log($"Aciertos actuales: {platosServidos} / {variablesComunes.platos_a_servir}");
            aciertoContado = false; 
        }
    }

    private void CheckAcierto(List<GameObject> instantiatedObjectsBocadillo, GameObject selectedColliderMesa, GameObject collisionFatherMesa)
    {
        if (instantiatedObjectsBocadillo.Count > 0 && objetoColision != null && !aciertoChecked)
        {
            //Debug.Log("TOY AQUI1");
            if (selectedColliderMesa == collisionFatherMesa)
            {
                //Debug.Log("TOY AQUI2");
                if (objetoColision.name.Contains(instantiatedObjectsBocadillo[0].name) && !aciertoChecked)
                {
                    //Debug.Log("TOY AQUI3");
                    // Success (acierto)
                    GameObject reinstaciar_obj = objetoColision;
                    objetoColision = null;

                    ReinstanciarEnBarra(reinstaciar_obj);

                    aciertoChecked = true; // Set success flag
                    StartCoroutine(HandleCollision(reinstaciar_obj));
                    CheckAndSavePosition(reinstaciar_obj);

                    if (platosServidos < variablesComunes.platos_a_servir)
                    {
                        // Llama a la corutina para desactivar el bocadillo y el collider tras 2 segundos
                        GameObject bocadilloToDeactivate = null;
                        if (collisionFatherMesa == cafeteria.selectedColliderM1) bocadilloToDeactivate = cafeteria.bocadillo_M1;
                        if (collisionFatherMesa == cafeteria.selectedColliderM2) bocadilloToDeactivate = cafeteria.bocadillo_M2;
                        if (collisionFatherMesa == cafeteria.selectedColliderM3) bocadilloToDeactivate = cafeteria.bocadillo_M3;

                        StartCoroutine(DeactivateAfterDelay(instantiatedObjectsBocadillo[0], collisionFatherMesa, bocadilloToDeactivate));
                    }

                    collisionFatherM1 = null;
                    collisionFatherM2 = null;
                    collisionFatherM3 = null;

                    //Debug.Log("Acierto detected: " + reinstaciar_obj.name);
                }
                else
                {
                    HandleError();
                }
            }
            else if (selectedColliderMesa != collisionFatherMesa && !errorContado)
            {
                //Debug.Log("Selected Collider Mesa: " + selectedColliderMesa);
                //Debug.Log("Selected Collider Father: " + collisionFatherMesa);

                HandleError();
            }
        }
        else
        {
            // Reset flags for invalid or no objects
            //Debug.Log("No objects or invalid collision detected.");
            aciertoChecked = false;
            errorContado = false;
        }
    }

    bool IsPositionOccupied(Vector3 position)
    {
        // OverlapSphere radius; adjust 0.1f if needed.
        float radius = 0.1f;
        Collider[] colliders = Physics.OverlapSphere(position, radius);

        foreach (var collider in colliders)
        {
            // Debug.Log($"Collider found: {collider.name} in position {position}");

            // Check if the collider's GameObject has the "DynamicObject" tag
            if (collider.gameObject.CompareTag("DynamicObject"))
            {
                Debug.Log($"Collider with tag 'DynamicObject' found: {collider.name}");
                return true; // Position is occupied
            }
        }

        // No matching collider found
        return false;
    }

    void CheckAndSavePosition(GameObject obj)
    {
        Debug.Log($"Checking positions for {obj.name}");

        foreach (var position in variablesComunes.posBarra)
        {
            Vector3 adjustedPosition = position;

            if (!IsPositionOccupied(adjustedPosition))
            {
                var prefab = listaPrefabsNiveles.FindPrefabByName(obj.name);
                if (prefab == null)
                {
                    //Debug.LogError($"Prefab not found for {obj.name}. Skipping position.");
                    continue;
                }
                GameObject newDrink = variablesComunes.CreateNewPlateRandomList(prefab, adjustedPosition, obj.transform.eulerAngles);
                variablesComunes.instantiatedDrinksBarra.Add(newDrink);

                break;
            }
            else
            {
                Debug.Log($"Position {adjustedPosition} is occupied. Moving to the next position.");
            }
        }
    }

    private void HandleError()
    {
        Debug.Log("Holaaa");
        if (objetoColision == null) return;
        variablesComunes.errores += 1;
        //errores_repeticion += 1;
        GameObject reiniciarPrefab = listaPrefabsNiveles.FindPrefabByName(objetoColision.name);
        objetoColision.SetActive(false);
        objetoColision = null;
        //StartCoroutine(Wait1Second());
        //LanzarError();
        StartCoroutine(LaunchError());
        CheckAndSavePosition(reiniciarPrefab);
        errorContado = true;
    }

    public IEnumerator LaunchFinalNivel()
    {
        variablesComunes.canvasFinalNivel.SetActive(true);

        Vector3 targetPosition = Camera.main.transform.position + Camera.main.transform.forward * 0.7f + new Vector3(0f, 0.34f, 0f);
        Quaternion cameraRotation = Camera.main.transform.rotation;
        Quaternion yRotation = Quaternion.Euler(0, cameraRotation.eulerAngles.y, 0);

        variablesComunes.canvasFinalNivel.transform.SetPositionAndRotation(targetPosition, yRotation);

        AudioSource audioNivelfin = variablesComunes.canvasFinalNivel.GetComponentInChildren<AudioSource>();
        audioNivelfin.Play();

        lanzarNivelFinalizado = true;

        variablesComunes.repeticiones += 1;
        logSaver.SetLogEvent("Nivel_finalizado_" + SceneManager.GetActiveScene().name + "_" + variablesComunes.currentLevel + "_Repeticion_" + variablesComunes.repeticiones);

        yield return new WaitForSeconds(3f);

        if (variablesComunes.repeticiones < variablesComunes.repeticiones_max)
        {
            variablesComunes.canvasFinalNivel.SetActive(false);
            audioNivelfin.Stop();
            ReiniciarNivel();
            lanzarNivelFinalizado = false;
        }
        else
        {
            cafeteria.ClearLevel();
        }
    }

    private IEnumerator DeactivateAfterDelay(GameObject bocadilloObject, GameObject collider, GameObject bocadillo)
    {
        yield return new WaitForSeconds(1.0f); // Esperar 2 segundos

        if (bocadilloObject != null && bocadilloObject.activeSelf)
        {
            bocadilloObject.SetActive(false); // Desactivar el objeto en el bocadillo
        }
        if (collider != null && collider.activeSelf)
        {
            collider.SetActive(false); // Desactivar el collider
        }
        if (bocadillo != null && bocadillo.activeSelf)
        {
            bocadillo.SetActive(false); // Desactivar el bocadillo
        }

        Debug.Log("Bocadillo, collider y objeto desactivados tras 0.5 segundos");
    }

    public IEnumerator LaunchError()
    {
        variablesComunes.canvasError.SetActive(true);

        Vector3 targetPosition = Camera.main.transform.position + Camera.main.transform.forward * 0.5f + new Vector3(0f, 0.34f, 0f);
        Quaternion cameraRotation = Camera.main.transform.rotation;
        Quaternion yRotation = Quaternion.Euler(0, cameraRotation.eulerAngles.y, 0);

        variablesComunes.canvasError.transform.SetPositionAndRotation(targetPosition, yRotation);

        yield return new WaitForSeconds(2f);

        variablesComunes.canvasError.SetActive(false);
        variablesComunes.audioSource_error.Stop();
        
        if (variablesComunes.errores >= 3)
        {
            // StartCoroutine(WaitUntilResetLevel());
            variablesComunes.vidas -= 1;
            variablesComunes.errores = 0;

            if (variablesComunes.vidas > 0)
            {
                logSaver.SetLogEvent("3Errores_Nivel_" + variablesComunes.currentLevel + "_ReiniciaNivel");
                StartCoroutine(WaitUntilResetLevel());
            }
            else if (variablesComunes.vidas == 0)
            {
                logSaver.SetLogEvent("Pierde3Vidas_Nivel_" + variablesComunes.currentLevel + "_VuelveNivelAnterior");
                StartCoroutine(WaitUntilLowerLevel());
            }
        }
    }

    IEnumerator WaitUntilResetLevel()
    {
        variablesComunes.Launch_Supera_Errores(Camera.main.transform.position, Camera.main.transform.rotation);
        yield return new WaitForSeconds(5f);
        variablesComunes.audioSource_supera_erroes.Stop();
        variablesComunes.canvasSuperaErrores.SetActive(false);
        ReiniciarNivel();
    }

    IEnumerator WaitUntilLowerLevel()
    {
        variablesComunes.Launch_Pierde_Vidas(Camera.main.transform.position, Camera.main.transform.rotation);
        yield return new WaitForSeconds(5f);
        variablesComunes.audioSource_supera_erroes.Stop();
        variablesComunes.canvasSuperaErrores.SetActive(false);
        variablesComunes.currentLevel -= 1;
        ReiniciarNivel();
    }

    private void ReiniciarNivel()
    {
        variablesComunes.faseActual = 0;
        cafeteria.faseAnterior = -1;
        platosServidos = 0;
        errores_repeticion = 0;

        foreach (var drink in variablesComunes.instantiatedDrinksBarra)
        {
            Destroy(drink);
        }
        variablesComunes.instantiatedDrinksBarra.Clear();

        // Reset flags and state
        countedDrinks.Clear();
        drinksTimers.Clear();
        aciertoChecked = false;
        errorContado = false;

        // Reset UI and sounds
        variablesComunes.canvasFinalNivel.SetActive(false);
        variablesComunes.tick.SetActive(false);
        variablesComunes.audioSource_ok.Stop();

        // Log reset event
        logSaver.SetLogEvent("Nivel_reiniciado_" + SceneManager.GetActiveScene().name);
        ComprobarSolapamientosEnBarra();

    }
}

/*
            if (!matchFound)
            {
                lanzarError = true;
                variablesComunes.errores += 1;
                //errores_repeticion += 1;
                logSaver.SetLogEvent("Lanza_Error");
                StartCoroutine(LaunchError());
                errorContado = true;
            }
            else
            {
                errorContado = false;
            }*/

/*
 private GameObject FindPrefabByName(string name)
    {
        if (name.Contains("Cafe"))
        {
            return listaPrefabsNiveles.prefabCafe;
        }
        else if (name.Contains("cola"))
        {
            return listaPrefabsNiveles.prefabCola;
        }
        else if (name.Contains("soda_orange"))
        {
            return listaPrefabsNiveles.prefabFanta;
        }
        else if (name.Contains("Cerveza"))
        {
            return listaPrefabsNiveles.prefabCerveza;
        }
        else if (name.Contains("Agua"))
        {
            return listaPrefabsNiveles.prefabAgua;
        }
        else if (name.Contains("water_b"))
        {
            return listaPrefabsNiveles.prefabAgua2;
        }
        else if (name.Contains("rvine03"))
        {
            return listaPrefabsNiveles.prefabVino;
        }
        else if (name.Contains("wvine02"))
        {
            return listaPrefabsNiveles.prefabVino2;
        }
        else if (name.Contains("Zumo_Naranja"))
        {
            return listaPrefabsNiveles.prefabZumo2;
        }
        else if (name.Contains("Zumo_Manzana"))
        {
            return listaPrefabsNiveles.prefabZumo;
        }
        else if (name.Contains("Chicken"))
        {
            return listaPrefabsNiveles.prefabPlatoPollo;
        }
        else if (name.Contains("Plate_Pizza"))
        {
            return listaPrefabsNiveles.prefabPlatoPizza;
        }
        else if (name.Contains("Plate2_Pizza"))
        {
            return listaPrefabsNiveles.prefabPlatoPizza2;
        }
        else if (name.Contains("PlateTarta"))
        {
            return listaPrefabsNiveles.prefabPlatoTarta;
        }
        else if (name.Contains("Plate2Tarta"))
        {
            return listaPrefabsNiveles.prefabPlatoTarta2;
        }
        else if (name.Contains("Toast"))
        {
            return listaPrefabsNiveles.prefabPlatoDesayuno;
        }
        else if (name.Contains("PlateBurger"))
        {
            return listaPrefabsNiveles.prefabPlatoHamburguesa;
        }
        else if (name.Contains("PlateMacBurger"))
        {
            return listaPrefabsNiveles.prefabPlatoHamburguesa2;
        }
        else
        {
            return null;
        }
    }
 */

/*
            if (variablesComunes.instantiatedDrinksBocadilloM1 != null && cafeteria.bocadillo_M1.activeSelf)
            {
                bocadillos.Add(variablesComunes.instantiatedDrinksBocadilloM1[0]);
            }
            if (variablesComunes.instantiatedDrinksBocadilloM2 != null && cafeteria.bocadillo_M2.activeSelf)
            {
                bocadillos.Add(variablesComunes.instantiatedDrinksBocadilloM2[0]);
            }
            if (variablesComunes.instantiatedDrinksBocadilloM3 != null && cafeteria.bocadillo_M3.activeSelf)
            {
                bocadillos.Add(variablesComunes.instantiatedDrinksBocadilloM3[0]);
            }*/

/*
if (errores_repeticion >= 3 && variablesComunes.errores < 3)
{
    variablesComunes.errores += 1;

    StartCoroutine(WaitUntilResetLevel());
}
if (variablesComunes.errores >= 3 && errores_repeticion >= 3) 
{
    if (variablesComunes.currentLevel > 1)
    {
        logSaver.SetLogEvent("Pierde3Vidas_Nivel_" + variablesComunes.currentLevel + "_VuelveNivelAnterior");
        StartCoroutine(WaitUntilLowerLevel());
    }
    else if (variablesComunes.currentLevel == 1)
    {
        logSaver.SetLogEvent("Pierde3Vidas_Nivel_" + variablesComunes.currentLevel);
    }
}
*/