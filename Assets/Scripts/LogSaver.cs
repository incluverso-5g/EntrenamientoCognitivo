using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;

public class LogSaver : MonoBehaviour
{
    string LogUrl;
    string state = "Idle";

    public string UserId;
    public string NSession;
    public string Mano;

    public Camera UserCamera;

    public string Event = "none";
    string previousEvent = "none";

    public void SetLogEvent(string newEvent)
    {
        //if (newEvent != previousEvent)
        //{
            Event = newEvent; 
            LogEvent(); 
            previousEvent = Event;
            Event = "none";            
        //}
    }
    void Start()
    {
        string DirPath = Application.persistentDataPath + "/EventLogs/";

        if (LogUrl == null)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            LogUrl = DirPath + "Events_" + UserId + "_" + NSession + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
#else
            LogUrl = "C:/Users/Incluverso/Documents/Event_Logs/" + UserId + "_" + NSession + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
#endif
            //Debug.Log(LogUrl);
        }        

        if (!Directory.Exists(DirPath))
        {
            // Create the directory
            Directory.CreateDirectory(DirPath);
            Console.WriteLine($"LOGEYE Directory created at {DirPath}");
        }


        if (!File.Exists(LogUrl))
        {
            using (StreamWriter sw = File.CreateText(LogUrl))
            {
                sw.WriteLine("UserID," + UserId + ",NSession," + NSession + ",ManoE4," + Mano);
                sw.WriteLine("TimeStamp,EVENT");
            }
        }
    }

    void LogEvent()
    {
        string UnixTime = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds().ToString();

        using (StreamWriter sw = File.AppendText(LogUrl))
        {
            sw.WriteLine(UnixTime + "," + Event);
        }
    }

    void OnApplicationQuit()
    {
        Debug.Log("Application has ended after " + Time.time + " seconds");
    }












}




