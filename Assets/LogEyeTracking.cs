using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using Wave.Essence.Eye;
using System.Text;
using UnityEngine.SceneManagement;

public class LogEyeTracking : MonoBehaviour
{

    //Stream Writer Variables
    private StreamWriter sw;
    const int BufferSize = 65536;
    string log_first_line = "Timestamp;Validity;"+
        "HeadPositionX;HeadPositionY;HeadPositionZ;" +
        "HeadRotationX;HeadRotationY;HeadRotationZ;" +
        "EyeTrackingStatus;EyeTrackingValidity;" +
        "LeftEyeOriginX;LeftEyeOriginY;LeftEyeOriginZ;" +
        "RightEyeOriginX;RightEyeOriginY;RightEyeOriginZ;" +
        "CombinedEyeOriginX;CombinedEyeOriginY;CombinedEyeOriginZ;" +
        "LeftEyeDirectionX;LeftEyeDirectionY;LeftEyeDirectionZ;" +
        "RightEyeDirectionX;RightEyeDirectionY;RightEyeDirectionZ;" +
        "CombinedEyeDirectionX;CombinedEyeDirectionY;CombinedEyeDirectionZ;" +
        "LeftEyeOpenness;RightEyeOpenness;" +
        "LeftEyePupilDiameter;RightEyePupilDiameter";

    string LogUrl;
    string DirPath;

    //User Variables
    public string UserId;
    public string NSession;

    //Data Variables
    string timestamp = "";
    Vector3 leftOrigin, rightOrigin, combinedOrigin;
    Vector3 leftDirection, rightDirection, combinedDirection;
    float leftOpenness, rightOpenness;
    float leftPupilDiameter, rightPupilDiameter;  
    bool[] val = new bool[10];
    public Camera userCamera;
    private Vector3 headPos;
    private Vector3 headRot;

    public GameObject pupilSphere;

    private float timeCounter = 0;
    private void Awake()
    {
        if (EyeManager.Instance != null) { EyeManager.Instance.EnableEyeTracking = true; }
    }

    void Start()
    {
        UserId = GetComponent<LogSaver>().UserId;
        NSession = GetComponent<LogSaver>().NSession;

        if (LogUrl == null)
        {

#if UNITY_ANDROID && !UNITY_EDITOR
            DirPath = Application.persistentDataPath + "/EyeMovementLogs";
            LogUrl = DirPath + "/EyeMov_" + UserId + "_" + NSession + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
            
#else
            LogUrl = Application.persistentDataPath + UserId + "_" + NSession + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
            //DirPath = Application.persistentDataPath + "/EyeMovementLogs";
#endif
            //Debug.Log("Logging " + LogUrl);
        }

        if (!Directory.Exists(DirPath))
        {
            // Create the directory
            Directory.CreateDirectory(DirPath);
            Debug.Log($"LOGEYE Directory created at {DirPath}");
        }


        if (!File.Exists(LogUrl))
        {
            using (StreamWriter sw = File.CreateText(LogUrl))
            {
                sw.WriteLine("UserID;" + UserId + ";NSession;" + NSession);
                sw.WriteLine(log_first_line);
                sw.Flush();
            }
            Debug.Log("LOGEYE file created at " + LogUrl);
        }

        sw = new StreamWriter(LogUrl, true, System.Text.Encoding.UTF8, BufferSize);
        sw.AutoFlush = false;
        //Debug.Log("LOGEYE sw created");


        if (EyeManager.Instance != null) { EyeManager.Instance.EnableEyeTracking = true; }
        else
        {
            Debug.Log("LOGEYE EyeManager == NULL");
        }
        if (EyeManager.Instance.LocationSpace == EyeManager.EyeSpace.Local)
        {
            EyeManager.Instance.LocationSpace = EyeManager.EyeSpace.World;
        }
    }

    void Update()
    {
            
        if (sw == null)
        {
            sw = new StreamWriter(LogUrl, true, System.Text.Encoding.UTF8, BufferSize);
            sw.AutoFlush = false;
        }

        //DEBUG CODE
        /*
        if (EyeManager.Instance == null)
        {
            Scene currentScene = SceneManager.GetActiveScene();
            // Retrieve the name of the active scene
            string sceneName = currentScene.name;
            Debug.Log("LOGEYE Current Scene Name: " + sceneName);
            Debug.Log("LOGEYE EYE MANAGER NULL");
        }
        */

        if (EyeManager.Instance.IsEyeTrackingAvailable())
        {
            timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds().ToString();
            headRot = userCamera.transform.eulerAngles;
            headPos = userCamera.transform.position;
            val[0] = EyeManager.Instance.GetLeftEyeOrigin(out leftOrigin);
            val[1] = EyeManager.Instance.GetRightEyeOrigin(out rightOrigin);
            val[2] = EyeManager.Instance.GetCombinedEyeOrigin(out combinedOrigin);

            val[3] = EyeManager.Instance.GetLeftEyeDirectionNormalized(out leftDirection);
            val[4] = EyeManager.Instance.GetRightEyeDirectionNormalized(out rightDirection);
            val[5] = EyeManager.Instance.GetCombindedEyeDirectionNormalized(out combinedDirection);

            val[6] = EyeManager.Instance.GetLeftEyeOpenness(out leftOpenness);
            val[7] = EyeManager.Instance.GetRightEyeOpenness(out rightOpenness);

            val[8] = EyeManager.Instance.GetLeftEyePupilDiameter(out leftPupilDiameter);
            val[9] = EyeManager.Instance.GetRightEyePupilDiameter(out rightPupilDiameter);

            /*PUPILSIZE SCENE CODE
             * if(val[8] && val[9])
            {
                float pup = (leftPupilDiameter + rightPupilDiameter) / 2;
                pup = 0.5f + (pup / 10);
                if(pupilSphere != null)
                {
                    pupilSphere.transform.localScale = new Vector3(pup, pup, pup);
                }
            }
            */

            StringBuilder sb = new StringBuilder();

            // Iterate over each boolean in the list and append '1' or '0'
            foreach (bool value in val)
            {
                sb.Append(value ? '1' : '0');
            }
            
            string data = $"{timestamp};{sb};" + 
                $"{headPos.x};{headPos.y};{headPos.z};" +
                $"{headRot.x};{headRot.y};{headRot.z};" +
                $"{leftOrigin.x};{leftOrigin.y};{leftOrigin.z};" +
                $"{rightOrigin.x};{rightOrigin.y};{rightOrigin.z};" +
                $"{combinedOrigin.x};{combinedOrigin.y};{combinedOrigin.z};" +
                $"{leftDirection.x};{leftDirection.y};{leftDirection.z};" +
                $"{rightDirection.x};{rightDirection.y};{rightDirection.z};" +
                $"{combinedDirection.x};{combinedDirection.y};{combinedDirection.z};" +
                $"{leftOpenness};{rightOpenness};{leftPupilDiameter};{rightPupilDiameter}";
           
            sw.WriteLine(data);
        }
        timeCounter += Time.deltaTime;

        //To avoid losing data, flush the streamwriter every 30seconds
        if (timeCounter > 30)
        {
            sw.Flush();
            timeCounter = 0;
        }
    }

    void OnDestroy()
    {
        if (sw != null)
        {
            sw.Close();
            sw = null;
        }
    }

    void OnApplicationPause()
    {
        if (sw != null)
        {
            sw.Flush();
        }
    }

    void OnApplicationQuit()
    {
        if (sw != null)
        {
            sw.Close();
            sw = null;
        }
    }
}
