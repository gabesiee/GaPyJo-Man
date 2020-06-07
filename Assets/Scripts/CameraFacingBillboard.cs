using UnityEngine;
using System.Collections;

public class CameraFacingBillboard : MonoBehaviour
{
   
    public bool activate = false;

    //Orient the camera after all movement is completed this frame to avoid jittering
    void LateUpdate()
        
    {
        Camera cam = GameObject.Find("CameraPlayer").GetComponent<Camera>();
        if (activate) {
            transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward,
            cam.transform.rotation * Vector3.up);
        }

    }
}