using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dooropen : MonoBehaviour
{
    public bool DoorOpen;
    public Animator doorAnimator;


    // Start is called before the first frame update
    void Start()
    {
        doorAnimator.SetBool("Open", false);
    }

    // Update is called once per frame
    void Update()
    {
        if (DoorOpen)
        {
            doorAnimator.SetBool("Open", true);
        }
    }
}
