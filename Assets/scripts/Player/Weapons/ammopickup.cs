using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ammopickup : MonoBehaviour
{
    public Gun Weapon;
    public AudioSource pickup;

    // Update is called once per frame
    void OnTriggerEnter()
    {
        Weapon.AmmoPickup();
        pickup.Play();
        Destroy(gameObject);
    }
}
