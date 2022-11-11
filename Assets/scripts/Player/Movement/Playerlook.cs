using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playerlook : MonoBehaviour
{
    public float sens;

    float xRotation;
    float yRotation;

    public Transform orientation;



    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        //if (PauseMenu.GameIsPaused = false)
        //{
            //get mouse input
            float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sens;
            float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sens;

            //get x roation
            xRotation += mouseX;

            //clamp y roation
            yRotation -= mouseY;
            yRotation = Mathf.Clamp(yRotation, -90f, 90f);

            //rotate camera and orientation
            transform.rotation = Quaternion.Euler(yRotation, xRotation, 0);
            orientation.rotation = Quaternion.Euler(0, xRotation, 0);

            //lock cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        //}
        //else
        //{
            //Cursor.lockState = CursorLockMode.None;
            //Cursor.visible = true;
        //}

    }
}
