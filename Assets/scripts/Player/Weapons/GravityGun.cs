using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class GravityGun : MonoBehaviour
{
 
    [SerializeField] Camera cam;
    [SerializeField] float maxGrabDistance = 10f, lerpSpeed = 10f;
    [SerializeField] Transform objectHolder;
    public KeyCode grabKey;
 
    Rigidbody grabbedRB;
 
    void Update()
    {
        if(grabbedRB)
        {
            grabbedRB.MovePosition(Vector3.Lerp(grabbedRB.position, objectHolder.transform.position, Time.deltaTime * lerpSpeed));
        }
 
        if(Input.GetKeyDown(grabKey))
        {
            if(grabbedRB)
            {
                grabbedRB.isKinematic = false;
                grabbedRB = null;
            }
            else
            {
                RaycastHit hit;
                Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
                if(Physics.Raycast(ray, out hit, maxGrabDistance))
                {
                    grabbedRB = hit.collider.gameObject.GetComponent<Rigidbody>();
                    if(grabbedRB)
                    {
                        grabbedRB.isKinematic = true;
                    }
                }
            }
        }
    }
}