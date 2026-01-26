using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class Replay : MonoBehaviour
{
    private string eyePathDir = "C:\\Users\\mdm\\Documents\\NeuronUpData\\Calibr\\";
    private string scenePathDir = "C:\\Users\\mdm\\Documents\\NeuronUpData\\Calibr\\";
    private string eventPathDir = "C:\\Users\\mdm\\Documents\\NeuronUpData\\Calibr\\";

    // File paths for the EyeTrack and LogScene CSVs
    public string eyeLogFilePath;
    public string sceneLogFilePath;
    public string eventLogFilePath;

    // Replay speed control: delay between event processing in seconds (0 for no delay)
    public float delayBetweenEvents = 0f;

    private List<object[]> eyeLogLines;
    private List<object[]> sceneLogLines;
    private List<object[]> eventLogLines;

    private int eventLogIndex = 0;
    private int eyeLogIndex = 0;
    private int sceneLogIndex = 0;

    private float timeSinceLastEvent = 0f;
    private bool super = false;
    private bool cafe = false;

    //public GameObject sup;
    //public GameObject caf;

    private Dictionary<int, GameObject> objectDictionary = new Dictionary<int, GameObject>();

    // Predefined objects in the scene that can be copied (indexed by their string name)
    public GameObject[] predefinedObjects;

    // Head position and rotation
    //public Transform cameraTransform; // Reference to the camera that will follow the head position
    public GameObject mainCamera;

    // LineRenderer for gaze direction
    //public LineRenderer leftEyeLine;
    //public LineRenderer rightEyeLine;
    //public LineRenderer combinedEyeLine;
    //public GameObject highlightMarkerPrefab; // A small sphere or marker to highlight the hit point

    public GameObject highlightMarker;

    // Toggle between using combined gaze or both eyes' gaze
    public bool useCombinedGaze = true;

    // Thresholds for pupil size to change color
    private const float minPupilSize = 0f;
    private const float maxPupilSize = 10f;
    public float colorValue = 5f;
    public float error_rad = 1.1f;

    public string user = "EC6";
    public string sess = "EC6";

    private bool firstSceneEv = false;
    private bool lastSceneEv = false;
    // Start is called before the first frame update
    
    void Start()
    {
        eyeLogFilePath = eyePathDir + "EyeMov.txt";// user + "\\EyeMov_" + user + "_" + sess + ".txt";
        sceneLogFilePath = scenePathDir + "Calibr.txt"; // user + "\\Scene_" + user + "_" + sess + ".txt";
        //eventLogFilePath = eventPathDir + user + "\\Events_" + user + "_" + sess + ".txt";

        // Load CSV lines into lists from file paths
        eyeLogLines = LoadEyeCSVFromFile(eyeLogFilePath);
        sceneLogLines = LoadSceneCSVFromFile(sceneLogFilePath);
        //eventLogLines = LoadEventLogFromFile(eventLogFilePath);
    }

    // Update is called once per frame
    void Update()
    {
        // Set the replay speed using the Time.timeScale
        if (delayBetweenEvents > 0)
        {
            timeSinceLastEvent += Time.deltaTime;

            // Process the next event only when the delay time has been reached
            if (timeSinceLastEvent < delayBetweenEvents) return;

            // Reset the timer after processing
            timeSinceLastEvent = 0;
        }

        ProcessNextEvent();
        //sup.SetActive(super);
        //caf.SetActive(cafe);
    }


    private List<object[]> LoadEyeCSVFromFile(string filePath)
    {
        List<object[]> parsedLines = new List<object[]>();

        try
        {
            // Read all lines from the CSV file at once
            string[] fileContent = File.ReadAllLines(filePath);

            // Start processing from the third line to skip the first two lines (headers)
            for (int i = 2; i < fileContent.Length; i++)
            {
                string line = fileContent[i];
                string[] columns = line.Split(';');

                // Parse each column to its corresponding data type
                try
                {
                    object[] parsedRow = new object[]
                    {
                    long.Parse(columns[0]),   // Timestamp as long
                    columns[1],               // Validity as string
                    float.Parse(columns[2]),  // HeadPositionX as float
                    float.Parse(columns[3]),  // HeadPositionY as float
                    float.Parse(columns[4]),  // HeadPositionZ as float
                    float.Parse(columns[5]),  // HeadRotationX as float
                    float.Parse(columns[6]),  // HeadRotationY as float
                    float.Parse(columns[7]),  // HeadRotationZ as float
                    float.Parse(columns[8]),  // LeftEyeOriginX as float
                    float.Parse(columns[9]),  // LeftEyeOriginY as float
                    float.Parse(columns[10]), // LeftEyeOriginZ as float
                    float.Parse(columns[11]), // RightEyeOriginX as float
                    float.Parse(columns[12]), // RightEyeOriginY as float
                    float.Parse(columns[13]), // RightEyeOriginZ as float
                    float.Parse(columns[14]), // CombinedEyeOriginX as float
                    float.Parse(columns[15]), // CombinedEyeOriginY as float
                    float.Parse(columns[16]), // CombinedEyeOriginZ as float
                    float.Parse(columns[17]), // LeftEyeDirectionX as float
                    float.Parse(columns[18]), // LeftEyeDirectionY as float
                    float.Parse(columns[19]), // LeftEyeDirectionZ as float
                    float.Parse(columns[20]), // RightEyeDirectionX as float
                    float.Parse(columns[21]), // RightEyeDirectionY as float
                    float.Parse(columns[22]), // RightEyeDirectionZ as float
                    float.Parse(columns[23]), // CombinedEyeDirectionX as float
                    float.Parse(columns[24]), // CombinedEyeDirectionY as float
                    float.Parse(columns[25]), // CombinedEyeDirectionZ as float
                    float.Parse(columns[26]), // LeftEyeOpenness as float
                    float.Parse(columns[27]), // RightEyeOpenness as float
                    float.Parse(columns[28]), // LeftEyePupilDiameter as float
                    float.Parse(columns[29])  // RightEyePupilDiameter as float
                    };

                    parsedLines.Add(parsedRow);
                }
                catch (System.Exception parseEx)
                {
                    //Debug.LogError($"Error parsing line {i}: {line}. Exception: {parseEx.Message}");
                }
            }

            Debug.Log($"Successfully loaded and parsed Eye CSV file: {filePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error loading Eye CSV file from {filePath}: {e.Message}");
        }

        return parsedLines;
    }

    private List<object[]> LoadSceneCSVFromFile(string filePath)
    {
        List<object[]> parsedLines = new List<object[]>();

        try
        {
            // Read all lines from the CSV file at once
            string[] fileContent = File.ReadAllLines(filePath);

            // Start processing from the second line to skip the header
            for (int i = 2; i < fileContent.Length; i++)
            {
                string line = fileContent[i];
                string[] columns = line.Split(';');

                // Parse each column to its corresponding data type
                try
                {
                    object[] parsedRow = new object[]
                    {
                    long.Parse(columns[0]),   // Timestamp as long
                    int.Parse(columns[1]),    // ID as int
                    columns[2],               // Object name as string
                    bool.Parse(columns[3]),   // Active state as bool
                    float.Parse(columns[4]),  // Position.x as float
                    float.Parse(columns[5]),  // Position.y as float
                    float.Parse(columns[6]),  // Position.z as float
                    float.Parse(columns[7]),  // Rotation.x as float
                    float.Parse(columns[8]),  // Rotation.y as float
                    float.Parse(columns[9]),  // Rotation.z as float
                    float.Parse(columns[10]), // Scale.x as float
                    float.Parse(columns[11]), // Scale.y as float
                    float.Parse(columns[12])  // Scale.z as float
                    };

                    parsedLines.Add(parsedRow);
                }
                catch (System.Exception parseEx)
                {
                    //Debug.LogError($"Error parsing line {i}: {line}. Exception: {parseEx.Message}");
                }
            }


            Debug.Log($"Successfully loaded and parsed CSV file: {filePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error loading CSV file from {filePath}: {e.Message}");
        }

        return parsedLines;
    }

    // Function to load and parse the event log CSV
    private List<object[]> LoadEventLogFromFile(string filePath)
    {
        List<object[]> parsedLines = new List<object[]>();

        try
        {
            // Read all lines from the CSV file
            string[] fileContent = File.ReadAllLines(filePath);

            // Start processing from the second line to skip the header
            for (int i = 2; i < fileContent.Length; i++)
            {
                string line = fileContent[i];
                string[] columns = line.Split(',');

                // Parse the row: timestamp as long, and event as a string
                object[] parsedRow = new object[]
                {
                    long.Parse(columns[0]), // Timestamp
                    columns[1]              // Event (e.g., "Lanza_Acierto")
                };

                parsedLines.Add(parsedRow);
            }

            Debug.Log($"Successfully loaded and parsed event log: {filePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error loading event log file from {filePath}: {e.Message}");
        }

        return parsedLines;
    }



    // Method to extract timestamp from a CSV row (as long)
    private long GetTimestamp(object[] csvLine)
    {
        return (long)csvLine[0];
    }

    // Method to process the next event from either EyeTrack or LogScene based on the next timestamp
    private void ProcessNextEvent()
    {

        // If both CSVs have been fully processed, stop
        if (eyeLogIndex >= eyeLogLines.Count && sceneLogIndex >= sceneLogLines.Count)
        {
            Debug.Log("Finished processing both CSV files.");
            return;
        }

        //If only EyeTrack events remain, process them
        if (eyeLogIndex < eyeLogLines.Count && sceneLogIndex >= sceneLogLines.Count)
        {
            ProcessEyeTrackEvent();
            return;
        }

        // If only LogScene events remain, process them
        if (sceneLogIndex < sceneLogLines.Count && eyeLogIndex >= eyeLogLines.Count)
        {
            ProcessLogSceneEvent();
            return;
        }

        // Compare timestamps and process the event with the smaller timestamp
        long eyeTrackNextTime = GetTimestamp(eyeLogLines[eyeLogIndex]);
        long logSceneNextTime = GetTimestamp(sceneLogLines[sceneLogIndex]);

        if (eyeTrackNextTime <= logSceneNextTime)
        {
            //eye data is next
            ProcessEyeTrackEvent();
        }
        else if (logSceneNextTime <= eyeTrackNextTime)
        {
            //scene data is next
            ProcessLogSceneEvent();
        }
        else
        {
            //event data is next
            //ProcessEventsLogEvent();
        }
    }


    // Method to process the next EyeTrack event
    private void ProcessEyeTrackEvent()
    {
        if (firstSceneEv)
        {
            HandleEyeData(eyeLogLines[eyeLogIndex]);
            Debug.Log("EyeTrack Event at " + GetTimestamp(eyeLogLines[eyeLogIndex]) + ": " + string.Join(",", eyeLogLines[eyeLogIndex]));
        }
        else
        {
            firstSceneEv = true;
        }
        eyeLogIndex++;
    }

    // Method to process the next LogScene event
    private void ProcessLogSceneEvent()
    {
        //if (!firstSceneEv)
        //{
        //    firstSceneEv = true;
        //}

        // Get the current timestamp to compare
        long currentTimestamp = GetTimestamp(sceneLogLines[sceneLogIndex]);

        // Process all lines with the same timestamp
        while (sceneLogIndex < sceneLogLines.Count && GetTimestamp(sceneLogLines[sceneLogIndex]) == currentTimestamp)
        {
            //Debug.Log("LogScene Event at " + currentTimestamp + ": " + string.Join(",", sceneLogLines[sceneLogIndex]));
            HandleObject(sceneLogLines[sceneLogIndex]);
            sceneLogIndex++; // Move to the next entry
        }

        //Debug.Log("LogScene Event at " + GetTimestamp(logSceneLines[logSceneIndex]) + ": " + string.Join(",", logSceneLines[logSceneIndex]));
        //HandleObject(logSceneLines[logSceneIndex]);
        //logSceneIndex++;
    }

    private void ProcessEventsLogEvent()
    {
        if (!firstSceneEv)
        {
            firstSceneEv = true;
        }

        HandleEvent(eventLogLines[eventLogIndex]);
        eventLogIndex++;
    }

    // Function to handle object creation or update
    private void HandleObject(object[] parsedRow)
    {
        // Extract relevant data from parsedRow
        int id = (int)parsedRow[1];
        string objectName = (string)parsedRow[2];
        bool isActive = (bool)parsedRow[3];
        Vector3 position = new Vector3((float)parsedRow[4], (float)parsedRow[5], (float)parsedRow[6]);
        Vector3 rotation = new Vector3((float)parsedRow[7], (float)parsedRow[8], (float)parsedRow[9]);
        Vector3 scale = new Vector3((float)parsedRow[10], (float)parsedRow[11], (float)parsedRow[12]);

        GameObject obj;

        // Check if the object with this ID is already in the dictionary
        if (objectDictionary.TryGetValue(id, out obj))
        {
            // Object already exists, so update its properties
            obj.transform.localPosition = position;
            obj.transform.localRotation = Quaternion.Euler(rotation);
            obj.transform.localScale = scale;
            obj.SetActive(isActive);
        }
        else
        {
            // Object does not exist, so find and copy the predefined object
            GameObject original = FindPredefinedObject(objectName);
            if (original != null)
            {
                // Instantiate a copy of the original object
                obj = Instantiate(original); //, position, Quaternion.Euler(rotation));
                obj.transform.SetParent(mainCamera.transform);
                obj.transform.localPosition = position;
                obj.transform.localRotation = Quaternion.Euler(rotation);
                obj.transform.localScale = scale;
                obj.SetActive(isActive);
                Rigidbody rb = obj.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    // Make the object kinematic
                    rb.isKinematic = true;

                    // Disable gravity
                    rb.useGravity = false;
                }


                // Add the new object to the dictionary
                objectDictionary[id] = obj;
            }
            else
            {
                Debug.LogError($"Could not find predefined object with name: {objectName}");
            }
        }

    }
    private GameObject FindPredefinedObject(string objectName)
    {
        //Remove trailing space and "([number])" or "(Clone)" from the saved object name
        string cleanedObjectName = Regex.Replace(objectName, @"\s*\(\d+\)|\s*\(Clone\)", ""); //Regex.Replace(objectName, @"\s*\(\d+\)", "");

        foreach (GameObject predefined in predefinedObjects)
        {
            if (predefined.name == cleanedObjectName)
            {
                // Instantiate a copy of the predefined object
                //GameObject copy = Instantiate(predefined);
                return predefined; // Return the instantiated copy
            }
        }
        return null; // Return null if no object is found
    }


    private void HandleEyeData(object[] parsedRow)
    {
        // Extract relevant data from parsedRow
        string validity = (string)parsedRow[1];
        Vector3 headPosition = new Vector3((float)parsedRow[2], (float)parsedRow[3], (float)parsedRow[4]); //HeadPosition
        Vector3 headRotation = new Vector3((float)parsedRow[5], (float)parsedRow[6], (float)parsedRow[7]); //HeadRotation
        Vector3 leftOr = new Vector3((float)parsedRow[8], (float)parsedRow[9], (float)parsedRow[10]);  // LeftEyeOrigin
        Vector3 rightOr = new Vector3((float)parsedRow[11], (float)parsedRow[12], (float)parsedRow[13]); // RightEyeOrigin
        Vector3 combOr = new Vector3((float)parsedRow[14], (float)parsedRow[15], (float)parsedRow[16]); // CombinedEyeOrigin
        Vector3 leftDir = new Vector3((float)parsedRow[17], (float)parsedRow[18], (float)parsedRow[19]); // LeftEyeDirection
        Vector3 rightDir = new Vector3((float)parsedRow[20], (float)parsedRow[21], (float)parsedRow[22]); // RightEyeDirection
        Vector3 combDir = new Vector3((float)parsedRow[23], (float)parsedRow[24], (float)parsedRow[25]); // CombinedEyeDirection
        float leftOpen = (float)parsedRow[26];
        float rightOpen = (float)parsedRow[27];
        float leftPupil = (float)parsedRow[28];
        float rightPupil = (float)parsedRow[29];

        UpdateHeadPose(headPosition, headRotation);
        UpdateGazeLines(validity, leftOr, rightOr, combOr, leftDir, rightDir, combDir, leftPupil, rightPupil);
    }


    // Update head position and rotation based on eye tracking data
    public void UpdateHeadPose(Vector3 headPosition, Vector3 headRotation)
    {
        mainCamera.transform.position = headPosition;
        mainCamera.transform.rotation = Quaternion.Euler(headRotation);
        Debug.Log("CAM POS: " + mainCamera.transform.position.ToString() + "(" + headPosition.ToString() +"), " + mainCamera.transform.rotation.ToString() + "(" + Quaternion.Euler(headRotation) + ")");
        //cameraTransform.position = headPosition;
        //cameraTransform.rotation = Quaternion.Euler(headRotation);
    }

    // Update the gaze lines based on the pupil size and direction
    public void UpdateGazeLines(string validity, Vector3 leftEyeOrigin, Vector3 rightEyeOrigin, Vector3 combinedEyeOrigin, Vector3 leftGazeDir,
                                Vector3 rightGazeDir, Vector3 combinedGazeDir, float leftPupilSize, float rightPupilSize)
    {
        // Check if the validity string is entirely "1"s (to ensure valid eye tracking data)
        if (IsValidGaze(validity))
        {
            // If using combined gaze, update only the combined line
            if (useCombinedGaze)
            {
                //UpdateLine(combinedEyeLine, combinedEyeOrigin, combinedGazeDir, (leftPupilSize+rightPupilSize)/2 );
                //INSIDE A SPHERE WE NEED TO REVERSE THE RAY DIRECTION
                Vector3 new_or = combinedEyeOrigin + 20 * combinedGazeDir;
                Vector3 new_dir = (combinedEyeOrigin - new_or).normalized;
                Ray ray = new Ray(new_or, new_dir);//combinedEyeOrigin, combinedGazeDir);
                RaycastHit hitInfo;
                Debug.DrawRay(ray.origin, ray.direction * 20, Color.red);
                // Perform the raycast
                if (Physics.Raycast(ray, out hitInfo))
                {

                    colorValue = (leftPupilSize + rightPupilSize) / 2;
                    //float distance = Vector3.Distance(combinedEyeOrigin, hitInfo.point);

                    float distance = Vector3.Distance(combinedEyeOrigin, hitInfo.point);

                    // Calculate the radius based on the 1.1-degree error cone
                    float radius = distance * Mathf.Tan(Mathf.Deg2Rad * error_rad);

                    // Move the marker to the hit point
                    if (highlightMarker != null)
                    {
                        highlightMarker.transform.localScale = Vector3.one * radius * 2; // Set sphere diameter
                        Color markerColor = Color.Lerp(Color.white, Color.black, colorValue / 10f);
                        highlightMarker.GetComponent<Renderer>().material.color = markerColor;
                        highlightMarker.SetActive(true);
                        highlightMarker.transform.position = hitInfo.point;
                        Debug.Log("HIIT");
                        Debug.DrawRay(ray.origin, ray.direction * distance, Color.red);
                    }
                }
                else
                {
                    Debug.Log("NO HIT");
                    // Hide the marker if no hit occurs
                    if (highlightMarker != null)
                    {
                        highlightMarker.SetActive(false);
                    }

                }
                //highlightMarker.SetActive(true);
                //combinedEyeLine.gameObject.SetActive(false);
                //leftEyeLine.gameObject.SetActive(false);
                //rightEyeLine.gameObject.SetActive(false);
            }
            else
            {
                // Update both left and right eye lines if using individual eyes
                //UpdateLine(leftEyeLine, leftEyeOrigin, leftGazeDir, leftPupilSize);
                //UpdateLine(rightEyeLine, rightEyeOrigin, rightGazeDir, rightPupilSize);

                highlightMarker.SetActive(false);
                //combinedEyeLine.gameObject.SetActive(false);
                //leftEyeLine.gameObject.SetActive(true);
                //rightEyeLine.gameObject.SetActive(true);
            }
        }
        else
        {
            // Hide the lines if the validity string is not all "1"s
            //combinedEyeLine.gameObject.SetActive(false);
            //leftEyeLine.gameObject.SetActive(false);
            //rightEyeLine.gameObject.SetActive(false);
        }
    }

    // Helper method to update the LineRenderer for each eye or combined gaze
    private void UpdateLine(LineRenderer line, Vector3 origin, Vector3 direction, float pupilSize)
    {
        // Set the starting point of the line at the camera's position
        line.SetPosition(0, origin);

        // Set the ending point of the line a certain distance away in the gaze direction
        Vector3 gazeEnd = origin + direction * 3.0f; // Assuming the gaze line should extend 10 units
        line.SetPosition(1, gazeEnd);

        // Adjust the color of the line based on pupil size (blue for small, green for medium, red for large)
        Color lineColor = ColorForPupilSize(pupilSize);
        line.startColor = lineColor;
        line.endColor = lineColor;
    }

    // Method to determine if the validity string contains only "1"s
    private bool IsValidGaze(string validity)
    {
        foreach (char c in validity)
        {
            if (c != '1') return false;
        }
        return true;
    }

    // Method to determine the color based on pupil size
    private Color ColorForPupilSize(float pupilSize)
    {
        float t = Mathf.InverseLerp(minPupilSize, maxPupilSize, pupilSize); // Normalize pupil size between 0 and 1
        return Color.Lerp(Color.blue, Color.red, t); // Interpolate between blue and red
    }

    private void HandleEvent(object[] parsedRow)
    {
        // Extract relevant data from parsedRow
        string ev = (string)parsedRow[1];

        if (ev == "Escena_Cafeteria_Entrenamiento")
        {
            cafe = true;
            super = false;
        }
        else if (ev == "Escena_Supermercado_Entrenamiento")
        {
            cafe = false;
            super = true;
        }
        else
        {
            Debug.Log("Event " + ev + " at time: " + ((long)parsedRow[0]).ToString());
        }

    }


}
