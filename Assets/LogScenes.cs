using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

public class LogScenes : MonoBehaviour
{
    // Path for the log file
    private string LogUrl;
    private string DirPath;
    private float logInterval = 1f; // 200ms
    public string UserId;
    public string NSession;

    private StreamWriter sw;
    const int BufferSize = 131072; //65536;
    public bool new_event=false;
    public string evnt = "";
    public string obj = "";
    private float timeCounter = 0f;
    private bool isClosing = false;
    private Dictionary<int, DynamicObj> dynamicObjects = new Dictionary<int, DynamicObj>();

    public class DynamicObj
    {
        // Properties for the DynamicObj class
        public int uniqueID; // Unique identifier for the object
        public string objName; // Name of the object
        public Vector3 rotation; // Rotation of the object
        public Vector3 position; // Position of the object
        public Vector3 scale; // Scale of the object
        public bool active; // Indicates if the object is active

        // Constructor for DynamicObj
        public DynamicObj(int uniqueID, string objName, Vector3 rotation, Vector3 position, Vector3 scale, bool active)
        {
            this.uniqueID = uniqueID;
            this.objName = objName;
            this.active = active;
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
        }


        // ToString method to return the object data in CSV format with ";" as separator
        public override string ToString()
        {
            return $"{uniqueID};{objName};{active};{position.x};{position.y};{position.z};{rotation.x};{rotation.y};{rotation.z};{scale.x};{scale.y};{scale.z}";
        }
    }


    void Start()
    {
        //Register the WantsToQuit event
        Application.wantsToQuit += OnApplicationWantsToQuit;

        UserId = GetComponent<LogSaver>().UserId;
        NSession = GetComponent<LogSaver>().NSession;

        if (LogUrl == null)
        {

#if UNITY_ANDROID && !UNITY_EDITOR
            DirPath = Application.persistentDataPath + "/SceneLogs";
            LogUrl = DirPath + "/Scene_" + UserId + "_" + NSession + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
#else
            LogUrl = "C:/Users/Incluverso/Documents/SceneLogs/" + UserId + "_" + NSession + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
#endif
            Debug.Log("Logging " + LogUrl);
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

                //OLD FIRST LINE
                //sw.WriteLine("timestamp;object;ID;position;rotation;scale");

                //NEW FIRST LINE
                sw.WriteLine("timestamp;ID;object;active;position.x;position.y;position.z;rotation.x;rotation.y;rotation.z;scale.x;scale.y;scale.z");

                sw.Flush();
            }
            Debug.Log("LOGSC file created at " + LogUrl);
        }

        sw = new StreamWriter(LogUrl, true, System.Text.Encoding.UTF8, BufferSize);
        sw.AutoFlush = false;
        Debug.Log("LOGSC sw created");
        
        // OLD COROT
        //StartCoroutine(LogTransformsCoroutine());

        //NEW COROT
        StartCoroutine(CheckDynamicObjects());
    }

    private void Update()
    {
        timeCounter += Time.deltaTime;
    }


    void OnDestroy()
    {
        if (sw != null)
        {
            sw.Close();
            sw = null;
        }
    }


    private void OnApplicationPause()
    {
        if (sw != null)
        {
            sw.Flush();
        }
    }

    private void OnApplicationQuit()
    {
        if (sw != null)
        {
            sw.Close();
            sw = null;
        }
    }

    private bool OnApplicationWantsToQuit()
    {
        // Set the isClosing flag to stop coroutines
        isClosing = true;

        // Stop new coroutines from being scheduled and stop all ongoing coroutines
        StopAllCoroutines();

        // Close the StreamWriter
        if (sw != null)
        {
            sw.Close();
            sw = null;
        }

        // Now allow the application to quit
        return true;
    }


    // Coroutine to check for new and updated objects
    private IEnumerator CheckDynamicObjects()
    {
        while (!isClosing)
        {
            HashSet<int> checkedObjects = new HashSet<int>();
            // Find all objects with tag "DynamicObject"
            GameObject[] dynamicObjs = GameObject.FindGameObjectsWithTag("DynamicObject");
            long time = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();

            foreach (GameObject obj in dynamicObjs)
            {
                // Get a unique ID for the object (you can modify this to suit your needs)
                int uniqueID = obj.GetInstanceID(); // Using instance ID as unique identifier
                //time = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
                // Get the object's current properties
                string objName = obj.name;
                Vector3 rotation = obj.transform.eulerAngles;
                Vector3 position = obj.transform.position;
                Vector3 scale = obj.transform.lossyScale;
                bool active = obj.activeSelf;
                checkedObjects.Add(uniqueID);
                if (position.magnitude > 20)
                {
                    if (dynamicObjects.ContainsKey(uniqueID))
                    {
                        //Object is too far, remove it from the list
                        dynamicObjects.Remove(uniqueID);
                    }
                }           
                else if (!dynamicObjects.ContainsKey(uniqueID))
                {
                    // If the object is not already in the dictionary, add it
                    DynamicObj newObj = new DynamicObj(uniqueID, objName, rotation, position, scale, active);
                    dynamicObjects.Add(uniqueID, newObj);
                    // Write the new object's data to the file
                    sw.WriteLine(time.ToString() + ";" + newObj.ToString());
                }
                else
                {
                    // Check if any properties of the object have changed
                    DynamicObj existingObj = dynamicObjects[uniqueID];
                    if (existingObj.position != position ||
                    existingObj.rotation != rotation ||
                    existingObj.scale != scale ||
                    existingObj.active != active)
                    {
                        // Update the existing object in the dictionary
                        existingObj.position = position;
                        existingObj.rotation = rotation;
                        existingObj.scale = scale;
                        existingObj.active = active;

                        // Write the updated object's data to the file
                        sw.WriteLine(time.ToString() + ";" + existingObj.ToString());
                    }
                    
                }
            }

            // Process inactive objects (those not found in the current active objects list)
            foreach (int uniqueID in dynamicObjects.Keys)
            {
                // If the uniqueID was not checked in the active objects, mark it as inactive
                if (!checkedObjects.Contains(uniqueID))
                {
                    DynamicObj existingObj = dynamicObjects[uniqueID];

                    //time = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();

                    // Only write to the log if the object was previously active
                    if (existingObj.active == true)
                    {
                        // Set the object's active state to false
                        existingObj.active = false;

                        // Write the updated object's data to the file
                        sw.WriteLine(time.ToString() + ";" + existingObj.ToString());
                    }
                }
            }

            // Flush the StreamWriter every 30seconds
            if (timeCounter > 30)
            {
                sw.Flush();
                timeCounter = 0;
            }

            // Wait for 0.5 seconds before the next iteration
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator LogTransformsCoroutine()
    {  
        while (true)
        {
            // Get all objects with the tag "DynamicObject"
            GameObject[] dynamicObjects = GameObject.FindGameObjectsWithTag("DynamicObject");

            //Get current timestamp
            long time = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();


            foreach (GameObject obj in dynamicObjects)
            {
                Transform objTransform = obj.transform;
                // Format: ObjectName | Position | Rotation | Scale
                string logLine = string.Format("{0};{1};{2};{3};{4};{5}",
                                               time,
                                               obj.name,
                                               obj.GetInstanceID(),
                                               objTransform.position,
                                               objTransform.rotation.eulerAngles,
                                               objTransform.lossyScale);
                sw.WriteLine(logLine);
                //logLines.Add(logLine);
            }

            //To avoid losing data, flush the streamwriter every 30seconds
            if (timeCounter>30)
            {
                sw.Flush();
                timeCounter = 0;
            }

            // Wait for the specified interval before running again
            yield return new WaitForSeconds(logInterval);
        }
    }
}