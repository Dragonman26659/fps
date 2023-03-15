using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitching : MonoBehaviour
{

    public int selectedWeapon = -1;
    public int enabledWeapons = 0;
    public bool[] unlockedWeapons; // new array to track unlocked weapons


    void Start()
    {
        SelectWeapon();
    }

    void Update()
    {
        int prevSelectedWeapon = selectedWeapon;
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if(selectedWeapon >= enabledWeapons - 1)
                selectedWeapon = 0;
            else
                selectedWeapon++;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if(selectedWeapon <= 0)
                selectedWeapon = enabledWeapons - 1;
            else
                selectedWeapon--;
        }
        if (selectedWeapon >= enabledWeapons)
        {
            selectedWeapon = 0;
        }

        if (prevSelectedWeapon != selectedWeapon)
        {
            SelectWeapon();
        }
    }


    void SelectWeapon()
    {
        int i = 0;
        foreach (Transform weapon in transform)
        {
            if(i == selectedWeapon && i < enabledWeapons && unlockedWeapons[i])
            {
                weapon.gameObject.SetActive(true);
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
            i++;
        }
    }

    public void WeaponPickup(int ID)
    {
        unlockedWeapons[ID-1] = true;
        enabledWeapons++;
        SelectWeapon();
    }
}

