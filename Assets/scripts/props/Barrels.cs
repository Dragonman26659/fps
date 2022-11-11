using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Barrels : MonoBehaviour
{
    //public config varables
    public NpcHealth NpcHealth;
    public float WaitTime = 3f;
    public GameObject FireEffect;
    public GameObject ExplosionEffect;
    public AudioSource BoomNoise;
    public float BlastRadius = 4f;
    public float BlastForce = 100f;
    public int PlayerDamage = 50;
    public float EnemyDamage = 50f;

    //bool logic
    private bool isOnFire = false;
    private bool hasExploded = false;
    private bool isTimerStarted = false;

    //extra varables
    private float health;
    private float TimeRemaing = 0f;
    private GameObject Fire = null;
    private GameObject Explosion = null;





    void Update()
    {
        //set health
        health = NpcHealth.health;


        //check heath for explode OR set alight
        if (health <= 20f)
        {
            StartTimer();
        }
        if (health <= 10f)
        {
            Explode();
        }



        if (TimeRemaing > 0f && !hasExploded && isTimerStarted)
        {
            TimeRemaing -= Time.deltaTime;
            Fire.transform.position = gameObject.transform.position;
            Fire.transform.rotation = gameObject.transform.rotation;
        }
        else if (TimeRemaing <= 0f && isTimerStarted)
        {
            Explode();
        }
    }

    private void Explode()
    {
        if (!hasExploded)
        {
            BoomNoise.Play();
            Explosion = Instantiate(ExplosionEffect, transform.position, transform.rotation);

            Collider[] collidersA = Physics.OverlapSphere(transform.position, BlastRadius);

            foreach (Collider nearbyObject in collidersA)
            {
                Health Health = nearbyObject.GetComponent<Health>();
                if (Health != null)
                {
                    Health.TakeDamage(PlayerDamage);
                }
                NpcHealth EnemyHealth = nearbyObject.GetComponent<NpcHealth>();
                if (NpcHealth != null)
                {
                    NpcHealth.TakeDamage(EnemyDamage);
                }
            }

            Collider[] collidersB = Physics.OverlapSphere(transform.position, BlastRadius);
            foreach (Collider nearbyObject in collidersB)
            {
                Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddExplosionForce(BlastForce, transform.position, BlastRadius);
                }
            }
            hasExploded = true;
        }
        if (hasExploded)
        {
            Destroy(gameObject, 0.80f);
            Destroy(Fire);
            Destroy(Explosion, 1f);
        }
    }

    private void StartTimer()
    {
        if (!isTimerStarted)
        {
            TimeRemaing = WaitTime;
            Fire = Instantiate(FireEffect, transform.position, transform.rotation);
            isTimerStarted = true;
        }
    }
    //Gizmos
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, BlastRadius);
    }
}