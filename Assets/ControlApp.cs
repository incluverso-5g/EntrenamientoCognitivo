using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Wave.Essence;
using Wave.Native;


public class ControlApp : MonoBehaviour
{
    GameObject waveRig;
    public Transform RightController;
    GameObject instrucciones;

    VariablesNiveles variablesNiveles;
    SelectAudioClip selectAudioClip;
    LogSaver logSaver;
    public TCPServer tcpServer;

    private int erroresAnteriores;

    public RandomizeShelfPositions2 randomizeScript; //MARTINA
    public RandomizeShelfPositions3 randomizeScript3;
    public GameObject shelfObject; // MARTINA
    public GameObject shelfObject3;

    private MovementController scriptMovementController;

    void Start()
    {
        waveRig = GameObject.Find("Wave Rig");
        variablesNiveles = waveRig.GetComponent<VariablesNiveles>();
        logSaver = waveRig.GetComponent<LogSaver>();
        tcpServer = GetComponent<TCPServer>();

        scriptMovementController = waveRig.GetComponent<MovementController>();
        scriptMovementController.enabled = false;

        erroresAnteriores = 0;

        instrucciones = GameObject.Find("Instrucciones");
        selectAudioClip = instrucciones.GetComponent<SelectAudioClip>();

        for (int i = 0; i < waveRig.transform.childCount; i++)
        {
            if (waveRig.transform.GetChild(i).name == "WaveRightController")
            {
                RightController = waveRig.transform.GetChild(i);
            }
        }

        //randomizeScript = shelfObject.GetComponent<RandomizeShelfPositions2>();
        //randomizeScript3 = shelfObject3.GetComponent<RandomizeShelfPositions3>();
    }

    void Update()
    {
        if (erroresAnteriores != variablesNiveles.erroresNivel)
        {
            tcpServer.SendDataFromAdditionalServer(variablesNiveles.erroresNivel.ToString());
        }
    }

    public void launchExternalUri(string uri)
    {
        if (uri.Contains("Escena") && uri.Contains("Tarea") && uri.Contains("Nivel"))
        {
            logSaver.SetLogEvent(uri);
            DeactivateAllDynamicObjects();
            variablesNiveles.corazones = 3;
            RestartScene();
            CambioTarea_Y_Nivel(uri);
        }
        else if (uri.Contains("Escena"))
        {
            logSaver.SetLogEvent(uri);
            uri = uri.Replace("Escena_", "");
            DeactivateAllDynamicObjects();
            RestartScene();
            SceneManager.LoadScene(uri);
        }
        else if (uri.Contains(".mp4"))
        {
            logSaver.SetLogEvent("Escenario_Relajante_" + uri);
            variablesNiveles.videoRelax = uri;
            SceneManager.LoadScene("Escenario_Relajante");
        }
        else if (uri.Contains("LanzaAudio"))
        {
            logSaver.SetLogEvent(uri);
            //Debug.Log("Entra audioclip");
            Lanza_Seleccion_Audio(uri);
        }
        else if (uri == "ResetLevel")
        {
            //if (SceneManager.GetActiveScene().name.Contains("Cafeteria"))
            //{
            logSaver.SetLogEvent(uri);
            DeactivateAllDynamicObjects();
            RestartScene();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            variablesNiveles.reiniciaNivel = true;
            //}
        } 
        else if(uri == "LanzarCalibracion")
        {
            logSaver.SetLogEvent(uri);
            RunEyeCalibration();
        }
        else if (uri == "LanzaError")
        {
            logSaver.SetLogEvent("Lanza_Error_Forzado");
            variablesNiveles.lanzaError = true;
        }
        else if (uri == "QuitarError")
        {
            logSaver.SetLogEvent(uri);
            variablesNiveles.erroresNivel -= 1;
        }
        else if (uri.Contains("ObjetosChocan"))
        {
            logSaver.SetLogEvent(uri);
            ObjetosChocan(uri);
        }
        else if (uri == "LanzarAcierto")
        {
            logSaver.SetLogEvent("Lanza_Acierto");
            variablesNiveles.lanzaAcierto = true;
        }
        else if (uri.Contains("InstruccionesEscritas"))
        {
            logSaver.SetLogEvent(uri);
            InstruccionesEscritas(uri);
        }
        else if (uri == "RecolocaProductos")
        {
            logSaver.SetLogEvent("Recoloca_Productos");
            variablesNiveles.Reset = true; 
        }
        else if (uri == "RecolocaProductosCaf")
        {
            logSaver.SetLogEvent("Recoloca_Productos");
            variablesNiveles.Recoloca = true;
        }
        else if (uri.Contains("BarrasEspaciadoras"))
        {
            BarrasEspaciadoras(uri);
        }
        else if (uri.Contains("ActivateMovementController"))
        {
            ActivateMovementController(uri);
        }
    }

    public void ActivateMovementController(string uri)
    {
        if (uri.Contains("True"))
        {
            scriptMovementController.enabled = true;
        }
        else if (uri.Contains("False"))
        {
            scriptMovementController.enabled = false;
        }
    }

    public void InstruccionesEscritas(string uri)
    {
        if (uri.Contains("True"))
        {
            variablesNiveles.lanzaInstrucciones = true;
        }
        else if (uri.Contains("False"))
        {
            variablesNiveles.lanzaInstrucciones = false;
        }
    }

    public void BarrasEspaciadoras(string uri)
    {
        Debug.Log("Entra barras espaciadoras");
        if (uri.Contains("True"))
        {
            variablesNiveles.Separadores = true;
            logSaver.SetLogEvent("True_separadores_estanterías");
        }
        else if (uri.Contains("False"))
        {
            variablesNiveles.Separadores = false;
            logSaver.SetLogEvent("False_separadores_estanterías");
        }
    }

    public void ObjetosChocan(string uri)
    {
        if (uri.Contains("True"))
        {
            variablesNiveles.objetosChocan = true;
        }
        else if (uri.Contains("False"))
        {
            variablesNiveles.objetosChocan = false;
        }
    }


    public void Lanza_Seleccion_Audio(string uri) 
    {
        string[] uris = uri.Split(";");

        //Debug.Log("Entra audioclip");

        selectAudioClip.playAudio = true;
        selectAudioClip.Select_And_Play_Audio(uris[1]);
        selectAudioClip.playAudio = false;
    }

    public void CambioTarea_Y_Nivel(string uri)
    {
        string[] uris = uri.Split(";");

        if (uris[0].Contains("Escena"))
        {
            uris[0] = uris[0].Replace("Escena_", "");

            if (uris[0].Contains("Caf"))
            {
                if (uris[0].Contains("1"))
                {
                    uris[1] = uris[1].Replace("Nivel", "");
                    variablesNiveles.nivel_cafeteria_T1 = int.Parse(uris[1]);
                }
                else if (uris[0].Contains("2"))
                {
                    uris[1] = uris[1].Replace("Nivel", "");
                    variablesNiveles.nivel_cafeteria_T2 = int.Parse(uris[1]);
                }

            }
            else if (uris[0].Contains("Super"))
            {
                if (uris[0].Contains("1"))
                {
                    uris[1] = uris[1].Replace("Nivel", "");
                    variablesNiveles.nivel_supermercado_T1 = int.Parse(uris[1]);
                }
                else if (uris[0].Contains("2"))
                {
                    uris[1] = uris[1].Replace("Nivel", "");
                    variablesNiveles.nivel_supermercado_T2 = int.Parse(uris[1]);
                }
            }

            DeactivateAllDynamicObjects();
            RestartScene();
            SceneManager.LoadScene(uris[0]);
        }
    }

    public static Transform GetChildWithTag(Transform transform, string tag)
    {
        if (transform.childCount == 0)
        {
            return null;
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).CompareTag(tag))
            {
                return transform.GetChild(i);
            }
            else
            {
                Transform childWithTag = GetChildWithTag(transform.GetChild(i), tag);
                if (childWithTag != null)
                {
                    return childWithTag;
                }
            }
        }

        return null;
    }

    void DeactivateAllDynamicObjects()
    {
        GameObject[] dynamicObjs = GameObject.FindGameObjectsWithTag("DynamicObject");

        foreach (GameObject obj in dynamicObjs)
        {
            obj.SetActive(false);
        }
    }
    void RestartScene() {
        // Destroy grabbed objects
        Transform p = GetChildWithTag(RightController, "DynamicObject");
        if (p != null)
        {
            p.parent = null;
            p.gameObject.SetActive(false);
        }

        // Destroy EyeTrackingScene objects
        p= GetChildWithTag(waveRig.transform, "EyeTestObject");
        if (p != null)
        {
            p.parent = null;
            p.gameObject.SetActive(false);
        }

        variablesNiveles.erroresNivel = 0;
        variablesNiveles.reiniciaNivel = false;
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    public void RunEyeCalibration()
    {
        using (AndroidJavaObject joActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"))
        {
            try
            {
                AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
                intentObject.Call<AndroidJavaObject>("setAction", "android.intent.action.MAIN");
                intentObject.Call<AndroidJavaObject>("setClassName", "vive.wave.vr.eye.calibration", "com.htc.vr.unity.WVRUnityVRActivity");

                AndroidJavaObject component = intentObject.Call<AndroidJavaObject>("getComponent");

                string action = intentObject.Call<string>("getAction");
                string packageName = component.Call<string>("getPackageName");
                string className = component.Call<string>("getClassName");

                Debug.Log($"Action: {action}");
                Debug.Log($"Package: {packageName}");
                Debug.Log($"Class Name: {className}");

                //intentObject.Call("addFlags", 0x10000000);
                joActivity.Call("startActivity", intentObject);
            }
            catch (Exception e)
            {
                Debug.LogError("Error starting eye calibration activity: " + e.Message);
            }
        }
    }
}
