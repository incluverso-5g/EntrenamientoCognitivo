using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CubeTest : MonoBehaviour
{
    public string ini_file = "TerapiaEscaleras.ini";

    public string newUri = null;

    private string user_id = "";
    private string NSession = "";
    private string Mano = "";
    public string hmd_ip = "192.168.31.123";
    private string status = "idle";

    // LogSaver LogComponent;
    GameObject waveRig;

    ControlApp controlApp;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        waveRig = GameObject.Find("Wave Rig");

        ReadConfigFile();
    }

    private void Start()
    {
        controlApp = GetComponent<ControlApp>();
    }


    void Update()
    {
        if (newUri != null)
        {
            Debug.Log("New external uri received" + newUri);
            string UritoProcess = newUri;
            newUri = "processing";
            controlApp.launchExternalUri(UritoProcess);
            newUri = null;
        }
    }

    private void ReadConfigFile()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        ini_file = Application.persistentDataPath + "/"+ini_file;
#else
        ini_file = "C:/Users/Incluverso/Documents/neuronup_interfaz4/py-scrcpy-client/scrcpy_ui/" + ini_file;
#endif

        Debug.Log("Reading config file " + ini_file);

        INIParser ini = new INIParser();
        ini.OpenFromString(File.ReadAllText(ini_file));

        user_id = ini.ReadValue("Defaults", "user_id", user_id);        
        NSession = ini.ReadValue("Defaults", "nsession", NSession);
        Mano = ini.ReadValue("Defaults", "mano", Mano);
        hmd_ip = ini.ReadValue("TCP", "hmd_ip", hmd_ip);

        waveRig.GetComponent<LogSaver>().UserId = user_id;
        waveRig.GetComponent<LogSaver>().NSession = NSession;
        waveRig.GetComponent<LogSaver>().Mano = Mano;

        ini.Close();
    }
}
