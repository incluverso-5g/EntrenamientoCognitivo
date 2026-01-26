using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Wave.Essence.Eye;
using System.IO;
using Unity.VisualScripting;

public class EyeTrackingScript : MonoBehaviour
{

    public Vector3[] positions = new Vector3[2];
    public Vector3 origin;
    public Vector3 direction;
    public Vector3 dest;
    public float leftPupil, rightPupil;
    public float timer = 0f;
    public LineRenderer line;
    public float lineLen = 10f;
    private StreamWriter sw;
    public string user = "Test";
    public string level = "Level1";
    public string log_file_path = "SD";
    public bool showLine = true;

    // TEST VARS
    public Vector3[] dirs = new Vector3[3];
    int aa = 0;


    private void Awake()
    {
        if (EyeManager.Instance != null) { EyeManager.Instance.EnableEyeTracking = true; }
    }

    private void Start()
    {
        if (EyeManager.Instance != null) { EyeManager.Instance.EnableEyeTracking = true; }
        if (EyeManager.Instance.LocationSpace == EyeManager.EyeSpace.Local)
        {
            EyeManager.Instance.LocationSpace = EyeManager.EyeSpace.World;
        }
        Debug.Log("Location Space " + EyeManager.Instance.LocationSpace.ToString());
        dirs[0] = new Vector3(-1, 1, 1);
        dirs[1] = new Vector3(0, 1, 1);
        dirs[2] = new Vector3(1, 1, 1);
        origin = new Vector3(0, 0, 0);
        direction = dirs[aa]; //new Vector3(1, 1, 1);
        aa += 1;
        dest = origin + lineLen * direction;
        positions[0] = origin;
        positions[1] = dest;


        //initialize the stream writer at the right path and with a buffer size
        const int BufferSize = 65536;
        sw = new StreamWriter(log_file_path, true, System.Text.Encoding.UTF8, BufferSize);
    }

    // Update is called once per frame
    void Update()
    {
        if (EyeManager.Instance == null)
        {
            Debug.Log("EYE MANAGER NULL NOT VALID DIR");
        }

        bool validDir = EyeManager.Instance.GetEyeDirectionNormalized(EyeManager.EyeType.Combined, out direction);
        bool validOr = EyeManager.Instance.GetEyeOrigin(EyeManager.EyeType.Combined, out origin);
        bool validPDL = EyeManager.Instance.GetLeftEyePupilDiameter(out leftPupil);
        bool validPDR = EyeManager.Instance.GetRightEyePupilDiameter(out rightPupil);

        if (validDir & validOr)
        {

            Vector3 origin_cam = transform.TransformPoint(Camera.main.transform.position);
            Vector3 world_direction = transform.TransformDirection(direction);
            Vector3 world_origin = transform.TransformPoint(origin);
            
            
            if (showLine)
            {
                dest = world_origin + lineLen * world_direction;
                positions[0] = world_origin;
                positions[1] = dest;
                line.enabled = true;
                line.SetPositions(positions);
            }
            else 
            {
                line.enabled = false;
            }

            Debug.Log("Valid Direction " + world_direction.ToString() + " or Origin " + world_origin.ToString() + " CamOrigin " + origin_cam.ToString());
        }
        else
        {
            Debug.Log("Not valid: Direction = " + validDir.ToString() + " ; Origin = " + validOr.ToString());
            direction = new Vector3(1, 1, 1);
            direction = dirs[aa]; //new Vector3(1, 1, 1);
            aa += 1;
            if (aa >= dirs.Length)
            {
            aa = 0;
            }
            dest = origin + lineLen * direction;
            positions[0] = new Vector3(0, 0, 0);
            positions[1] = dest;
            line.enabled = true;
            line.SetPositions(positions);
        }


        if (validPDL)
        {
            //leftPupil = leftPupil / 10;
            //sphereLeft.transform.localScale = new Vector3(0.1f + leftPupil, 0.1f + leftPupil, 0.1f + leftPupil);
            //Debug.Log("Left pupil size: " + leftPupil.ToString());
        }

        if (validPDR)
        {
            //ightPupil = rightPupil / 10;
            //sphereRight.transform.localScale = new Vector3(0.1f + rightPupil, 0.1f + rightPupil, 0.1f + rightPupil);
            //Debug.Log("Right pupil size: " + rightPupil.ToString());
        }
    }
}
