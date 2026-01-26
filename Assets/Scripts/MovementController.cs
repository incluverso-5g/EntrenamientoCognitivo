using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wave.Essence;
using Wave.Native;

public class MovementController : MonoBehaviour
{
    private float moveSpeed = 1.2f;
    private float rotationSpeed = 30f;
    private WVR_DeviceType device_rigth = WVR_DeviceType.WVR_DeviceType_Controller_Right;
    private WVR_DeviceType device_left = WVR_DeviceType.WVR_DeviceType_Controller_Left;
    public float max_speed = 1;
    void Update()
    {
        Vector2 inputAxis_right = WXRDevice.ButtonAxis(device_rigth, WVR_InputId.WVR_InputId_Alias1_Thumbstick);
        Vector3 direction = new Vector3(inputAxis_right.x, 0, inputAxis_right.y);
        float speed = inputAxis_right.magnitude;
        if (speed > max_speed)
        {
            max_speed = speed;
            //moveSpeed = moveSpeed / max_speed;
        }

        //Vector3 velocity = direction * moveSpeed * Time.deltaTime;
        //transform.Translate(velocity);

        Vector3 cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0; // Ignore the y component to keep movement horizontal.
        cameraForward.Normalize();

        Vector3 cameraRight = Camera.main.transform.right;
        cameraRight.y = 0;
        cameraRight.Normalize();

        // Calculate the movement direction relative to the camera's forward and right vectors.
        Vector3 adjustedDirection = cameraForward * direction.z + cameraRight * direction.x;
        adjustedDirection.Normalize();

        // Apply movement with the adjusted direction.
        Vector3 velocity = adjustedDirection * moveSpeed * speed/max_speed * Time.deltaTime;
        transform.Translate(velocity, Space.World);



        Vector2 inputAxis_left = WXRDevice.ButtonAxis(device_left, WVR_InputId.WVR_InputId_Alias1_Thumbstick);
        float rotation = inputAxis_left.x * rotationSpeed * Time.deltaTime;
        transform.RotateAround(transform.position, Vector3.up, rotation);
    }
}