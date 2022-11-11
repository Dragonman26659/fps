using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoldierGun : MonoBehaviour
{
    public int damage = 10;
    public float range = 100f;
    public AudioSource ShootSound;
    public Animator animator;
    public ParticleSystem muzzleFlash;
    public GameObject RayOrigin;
    public float ImpactForce = 10f;
    public GameObject surfaceImpactEffect;
    public GameObject enemyImpactEffect;
    public int bulletsPerShot = 1;
    public float bulletSpread = 1f;

    //Shoot= function
    public void Shoot()
    {
        muzzleFlash.Play();
        animator.SetTrigger("shoot");
        ShootSound.Play();

        foreach(int value in Enumerable.Range(0, bulletsPerShot))
        {
            RaycastHit Hit;
            if (Physics.Raycast(RayOrigin.transform.position, RayOrigin.transform.forward + new Vector3(0, Random.Range(-bulletSpread, bulletSpread), Random.Range(-bulletSpread, bulletSpread)), out Hit, range))
            {
                Health enemy = Hit.transform.GetComponent<Health>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                    GameObject bloodGO = Instantiate(enemyImpactEffect, Hit.point, Quaternion.LookRotation(Hit.normal));
                    Destroy(bloodGO, 2f);
                }
                if (enemy == null)
                {
                    GameObject ImpactGO = Instantiate(surfaceImpactEffect, Hit.point, Quaternion.LookRotation(Hit.normal));
                    Destroy(ImpactGO, 2f);
                }
                if (Hit.rigidbody != null)
                {
                    Hit.rigidbody.AddForce(-Hit.normal * ImpactForce);
                }
            }
        }
    }
}
