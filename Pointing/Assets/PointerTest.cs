using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PointerTest : MonoBehaviour {

    public Camera LookCamera;
    public Transform Controller;
    public float MovementSpeed = 1;

    public float LookSensitivity = 10;
    public float PointSensitivity = 10;
    
    private float pointer_pitch = 0;
    private float pointer_yaw = 0;

    void Update() {
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized * MovementSpeed;
        move = transform.TransformDirection(move);
        transform.Translate(move * Time.deltaTime);
        if(Input.GetButton("Switch Control"))
        {
            transform.Rotate(0, Input.GetAxis("Mouse X") * LookSensitivity, 0);
            LookCamera.transform.Rotate(-Input.GetAxis("Mouse Y") * LookSensitivity, 0, 0);
        } else {
            pointer_pitch += -Input.GetAxis("Mouse Y") * PointSensitivity;
            pointer_yaw += Input.GetAxis("Mouse X") * PointSensitivity;
            Controller.localRotation = Quaternion.Euler(pointer_pitch, pointer_yaw, 0);
        }
    }
}