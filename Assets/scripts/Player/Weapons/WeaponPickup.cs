using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public int WeaponID;
    public WeaponSwitching Holder;


    void OnTriggerEnter()
    {
        Holder.WeaponPickup(WeaponID);
        Destroy(gameObject);
    }
}
