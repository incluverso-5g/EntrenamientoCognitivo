using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GazeDotsSequence : MonoBehaviour
{
    // Log Variables
    private string LogUrl;
    private string DirPath;
    public string UserId;
    public string NSession;
    private StreamWriter sw;
    string log_first_line = "timestamp;ID;object;active;loc_position.x;loc_position.y;loc_position.z;loc_rotation.x;loc_rotation.y;loc_rotation.z;loc_scale.x;loc_scale.y;loc_scale.z;" +
                            "world_position.x;world_position.y;world_position.z;world_rotation.x;world_rotation.y;world_rotation.z;world_scale.x;world_scale.y;world_scale.z";
    
    //LogSaver variables
    public GameObject WaveRig;
    private LogSaver logSaver;

    //Dots variables
    public int numberOfDots = 5; // Number of dots to show
    public float initialSizeMultiplier = 5;
    public Vector3 originalScale = new Vector3(0.05f, 0.05f, 1f); 
    public float timePerDot = 8f; // How long each dot stays on screen
    public float moveSpeed = 0.1f; // Speed at which the dot moves between positions
    public float pulseDuration = 1f; // Duration to pulsate at each position
    public float shrinkDuration = 1f;
    public Camera vrCamera; // VR camera to calculate canvas positions
    public List<Vector3> dotPos;
    public List<GameObject> images;
    private GameObject currentDot;
    private int currentDotIndex = 0;
    public float radius = 2; //salient point distance from the camera
    public float omega = 20; //salient point zy(?) angle from the camera
    public float gamma = 30; //salient point xz(?) angle from the camera
    public GameObject sphere;
    public GameObject nivelFinalizado;
    public AudioSource audioFin;

    private void Start()
    {
        Debug.Log("CALIBR Sart");
        WaveRig = GameObject.Find("Wave Rig");
        if (WaveRig == null)
        {
            Debug.Log("CALIBR Wave Null");
        }
        else
        {
            logSaver = WaveRig.GetComponent<LogSaver>();
            UserId = logSaver.UserId;
            NSession = logSaver.NSession;
        }

        if (LogUrl == null)
        {

#if UNITY_ANDROID && !UNITY_EDITOR
            DirPath = Application.persistentDataPath + "/CalibrationTest";
            LogUrl = DirPath + "/Calibr_" + UserId + "_" + NSession + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
#else
            LogUrl = "C:/Users/Incluverso/Documents/CalibrationTest/" + UserId + "_" + NSession + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
#endif
            Debug.Log("CALIBR Logging " + LogUrl);
        }

        if (!Directory.Exists(DirPath))
        {
            // Create the directory
            Directory.CreateDirectory(DirPath);
            Debug.Log($"CALIBR Directory created at {DirPath}");
        }

        if (!File.Exists(LogUrl))
        {
            using (StreamWriter sw = File.CreateText(LogUrl))
            {
                sw.WriteLine("UserID;" + UserId + ";NSession;" + NSession);

                //NEW FIRST LINE
                sw.WriteLine(log_first_line);
                sw.AutoFlush = false;
                sw.Flush();
            }
            Debug.Log("CALIBR file created at " + LogUrl);
        }
       

        Transform cameraOffsetTransform = WaveRig.transform.Find("Camera Offset");
        vrCamera = cameraOffsetTransform.Find("Main Camera").GetComponent<Camera>();
        if (vrCamera == null)
        {
            Debug.Log("CALIBR VrCam Null");
        }

        transform.SetParent(vrCamera.transform);

        // Optionally, reset the object's local position, rotation, and scale
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        sw = new StreamWriter(LogUrl, true, System.Text.Encoding.UTF8);
        Debug.Log("CALIBR sw created");

        Vector3 pc = SphericalToCartesian(radius, 0, 0);
        dotPos.Add(pc);
        //Debug.Log("conversion: (" + radius.ToString() + ", 0, 0) = " + pc.ToString());

        Vector3 pur = SphericalToCartesian(radius, omega, gamma);
        dotPos.Add(pur);
        //Debug.Log("conversion: (" + radius.ToString() + ", " + omega.ToString() + ", " + gamma.ToString() +") = " + pur.ToString());

        Vector3 pul = SphericalToCartesian(radius, -omega, -gamma);
        dotPos.Add(pul);
        //Debug.Log("conversion: (" + radius.ToString() + ", " + (-1*omega).ToString() + ", " + (-1 * gamma).ToString() + ") = " + pul.ToString());

        Vector3 pbr = SphericalToCartesian(radius, -omega, gamma);
        dotPos.Add(pbr);
        //Debug.Log("conversion: (" + radius.ToString() + ", " + (-1 * omega).ToString() + ", " + (gamma).ToString() + ") = " + pul.ToString());

        Vector3 pbl = SphericalToCartesian(radius, omega, -gamma);
        dotPos.Add(pbl);
        //Debug.Log("conversion: (" + radius.ToString() + ", " + (omega).ToString() + ", " + (-1 * gamma).ToString() + ") = " + pul.ToString());

        nivelFinalizado = GameObject.Find("Escena_completada");
        audioFin = nivelFinalizado.GetComponentInChildren<AudioSource>();
        nivelFinalizado.SetActive(false);

        StartCoroutine(ShowDotsSequence());
    }

    public string printInfo(GameObject dot)
    {
        if (dot == null)
        {
            Debug.LogError("CALBIR The GameObject is null.");

            long t = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();

            string ss = $"{t};;;;" +
                            ";;;" +
                            ";;;" +
                            ";;;" +
                            ";;;" +
                            ";;;" +
                            ";;;";
            return ss;
        }

        // Get the transform of the GameObject
        Transform dotTransform = dot.transform;

        // Local position, rotation, and scale
        Vector3 locPosition = dotTransform.localPosition;
        Vector3 locRotation = dotTransform.localEulerAngles;
        Vector3 locScale = dotTransform.localScale;

        // World position, rotation, and scale
        Vector3 worldPosition = dotTransform.position;
        Vector3 worldRotation = dotTransform.eulerAngles;
        Vector3 worldScale = dotTransform.lossyScale;

        // Current timestamp
        long timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();

        // Object ID (could be the instance ID or custom ID)
        int objectID = dot.GetInstanceID();

        // Active state of the object
        bool isActive = dot.activeSelf;

        // return all the information in the requested format
        string s = $"{timestamp};{objectID};{dot.name};{isActive};" +
                  $"{locPosition.x};{locPosition.y};{locPosition.z};" +
                  $"{locRotation.x};{locRotation.y};{locRotation.z};" +
                  $"{locScale.x};{locScale.y};{locScale.z};" +
                  $"{worldPosition.x};{worldPosition.y};{worldPosition.z};" +
                  $"{worldRotation.x};{worldRotation.y};{worldRotation.z};" +
                  $"{worldScale.x};{worldScale.y};{worldScale.z};";

        return s;
    }

    IEnumerator AnimateDot(GameObject dot)
    {
        float pulseSpeed = 1.5f; // Speed of size fluctuation
        float maxScale = 1.3f; // Maximum size (110%)
        float minScale = 0.8f; // Minimum size (90%)
        float elapsedTime = 0f;

        while (elapsedTime < shrinkDuration)
        {
            float t = elapsedTime / shrinkDuration;
            dot.transform.localScale = Vector3.Lerp(originalScale * initialSizeMultiplier, originalScale, t*t);
            elapsedTime += Time.deltaTime;

            sw.WriteLine(printInfo(dot));

            yield return null;
        }
        sw.Flush();

        elapsedTime = 0f;
        while (true) // Infinite loop to keep the animation going while the dot is active
        {
            float t = elapsedTime / pulseSpeed;
            // Oscillate size to attract gaze
            float scale = Mathf.Lerp(minScale, maxScale, Mathf.PingPong(t * pulseSpeed, 1));
            dot.transform.localScale = scale * originalScale;

            //float brightness = Mathf.PingPong(Time.time * pulseSpeed, 1); // Oscillate between 0 and 1
            //dotImage.color = Color.Lerp(originalColor * 0.8f, originalColor * 1.2f, brightness); // Lerp between a darker and brighter version of the original color

            // Change color to create a glowing effect
            //dotImage.color = Color.Lerp(dotImage.color, dotImage.color * 1.2f, Mathf.PingPong(Time.time / pulseSpeed, 1)); // Increase brightness
            //dotImage.color = new Color(dotImage.color.r, dotImage.color.g, dotImage.color.b, 1f); // Ensure alpha is 1
            elapsedTime += Time.deltaTime;

            sw.WriteLine(printInfo(dot));

            yield return null; // Wait for the next frame
        }
    }

    IEnumerator ShowDotsSequence()
    {
        if(logSaver != null)
        {
            logSaver.SetLogEvent("StartCalibrationTest");
        }

        while (currentDotIndex < images.Count)
        {
            Debug.Log($"CALIBR new img");
            // Create a dot at the specified position
            currentDot = Instantiate(images[currentDotIndex]);
            currentDot.transform.SetParent(this.transform, false);
            currentDot.transform.localPosition = dotPos[currentDotIndex];
            currentDot.SetActive(true);
            currentDot.transform.localScale = originalScale * initialSizeMultiplier;
            currentDot.transform.LookAt(vrCamera.transform);//Vector3.zero);
            currentDot.transform.Rotate(0, 180, 0); // Adjust rotation based on image orientation
            
            //s += "Name: " + currentDot.name + " position: Vecor3(" + dotPos[currentDotIndex].x + ", " + dotPos[currentDotIndex].y + ", " + dotPos[currentDotIndex].z + ")\n"; 
            //Debug.Log("Name: " + currentDot.name + " position: Vecor3(" + dotPos[currentDotIndex].x + ", " + dotPos[currentDotIndex].y + ", " + dotPos[currentDotIndex].z + ")");

            sw.WriteLine(printInfo(currentDot));

            // Animate the dot (shine and size fluctuation)
            Coroutine cr = StartCoroutine(AnimateDot(currentDot));

            // Wait for the time to show the dot
            yield return new WaitForSeconds(timePerDot);

            StopCoroutine(cr);
            // Destroy the current dot and move to the next
            currentDot.SetActive(false);
            
            sw.WriteLine(printInfo(currentDot));
            sw.Flush();
            
            currentDotIndex++;

            //TODO: REMOVE NEXT
            //if(currentDotIndex>= images.Count)
            //{
            //    currentDotIndex = 0;
            //}
        }
        nivelFinalizado.SetActive(true);
        audioFin.Play();
        sw.WriteLine(printInfo(nivelFinalizado));
        sw.Flush();

        //yield return new WaitForSeconds(2f);
        //Destroy(sphere);
        //Destroy(nivelFinalizado);
    }

    public static Vector3 SphericalToCartesian(float r, float phig, float omegag)
    {
        float phi = Mathf.Deg2Rad*phig;
        float omega = Mathf.Deg2Rad * omegag;
        float x = r * Mathf.Sin(omega) * Mathf.Cos(phi);
        float y = r * Mathf.Sin(omega) * Mathf.Sin(phi);
        float z = r * Mathf.Cos(omega);
        return new Vector3(x, y, z);
    }

    private void OnDestroy()
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

}
