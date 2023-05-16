using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class EasyFirstPersonController : MonoBehaviour
{
    [Header("Movement")]
    public float WalkSpeed;
    public float RunSpeed;
    private float VarSpeed;
    private float SideFloat;
    private float ForwardFloat;
    [Header("Looking")]
    public Camera playerCamera;
    public float Sensitivity;
    private float Yaw;
    private float Pitch;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }


    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
            VarSpeed = RunSpeed;
        else
            VarSpeed = WalkSpeed;
        SideFloat = Input.GetAxis("Horizontal") * VarSpeed;
        ForwardFloat = Input.GetAxis("Vertical") * VarSpeed;
        Pitch += Input.GetAxis("Mouse Y") * Sensitivity * Time.deltaTime;
        Yaw += Input.GetAxis("Mouse X") * Sensitivity * Time.deltaTime;
        Pitch = Mathf.Clamp(Pitch, -90, 90);
        playerCamera.transform.rotation = Quaternion.Euler(-Pitch, Yaw, 0);
        transform.rotation = Quaternion.Euler(0, Yaw, 0);
        transform.Translate(Vector3.forward * ForwardFloat * Time.deltaTime + Vector3.right * SideFloat * Time.deltaTime);
    }
    
}
