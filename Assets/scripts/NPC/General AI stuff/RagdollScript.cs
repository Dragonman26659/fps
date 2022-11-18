using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollScript : MonoBehaviour
{
    public Collider maincoll;
    public GameObject thisGuysRig;
    public Animator ThisGuysAnimator;



    void Start()
    {
        GetRagdoll();
        RagdollModeOff();
    }

    Collider[] ragDollColliders;
    Rigidbody[] limbsRb;
    private void GetRagdoll()
    {
        ragDollColliders = thisGuysRig.GetComponentsInChildren<Collider>();
        limbsRb = thisGuysRig.GetComponentsInChildren<Rigidbody>();

    }

    public void RagdollModeOff()
    {
        foreach(Collider col in ragDollColliders)
        {
            col.enabled = false;
        }
        foreach (Rigidbody rigid in limbsRb)
        {
            rigid.isKinematic = true;
        }
        ThisGuysAnimator.enabled = true;
        maincoll.enabled = true;
    }


    public void RagdollModeOn()
    {
        foreach (Collider col in ragDollColliders)
        {
            col.enabled = true;
        }
        foreach (Rigidbody rigid in limbsRb)
        {
            rigid.isKinematic = false;
        }
        ThisGuysAnimator.enabled = false;
        maincoll.enabled = false;
    }
}