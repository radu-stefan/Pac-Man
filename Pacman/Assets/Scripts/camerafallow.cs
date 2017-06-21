using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camerafallow : MonoBehaviour {

    // Use this for initialization
    public GameObject player;
    public Vector3 offeset;
    public float turnSpeed;
    public float yaw, pitch;
    public int cameraNr;

    void Start () {
        offeset = transform.position - player.transform.position;
        turnSpeed = 0.7f;
        yaw = -158.8051f;
        pitch = 36.31509f;
        if(cameraNr == 1)
            offeset = new Vector3(4.49f, 7.44f, 0.339f);
    }

    // Update is called once per frame
    void Update () {
        transform.position = player.transform.position + offeset;

        if (cameraNr == 2)
            return;

        //transform.LookAt(player.transform.position);
        if (Input.GetMouseButton(0))
        {
            //transform.RotateAround(player.transform.position, Vector3.up, Input.GetAxis("Mouse X") * turnSpeed);
            //transform.RotateAround(player.transform.position, Vector3.right, -Input.GetAxis("Mouse Y") * turnSpeed);
            if (yaw > -185.94 && yaw < -146.7452)
                yaw += turnSpeed * Input.GetAxis("Mouse X");
            else
            {
                if (yaw < -185)
                    yaw = -185.93f;
                else
                    yaw = -146.7453f;
            }
            if(pitch > 24.99 && pitch <37.01)
                pitch -= turnSpeed * Input.GetAxis("Mouse Y");
            else
            {
                if (pitch < 25)
                    pitch = 25;
                else
                    pitch = 37;
            }

            //transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
        }
    }
}
