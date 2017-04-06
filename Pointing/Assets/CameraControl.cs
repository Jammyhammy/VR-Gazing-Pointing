using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {
    public float LookSensitivity = 10;
    public Camera LookCamera;
    public float MovementSpeed = 1;

	
	// Update is called once per frame
	void Update () {
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized * MovementSpeed;
        move = transform.TransformDirection(move);
        transform.Translate(move * Time.deltaTime);
        if(Input.GetButton("Switch Control"))
        {
            transform.Rotate(0, Input.GetAxis("Mouse X") * LookSensitivity, 0);
            LookCamera.transform.Rotate(-Input.GetAxis("Mouse Y") * LookSensitivity, 0, 0);
        } 		
	}
}
