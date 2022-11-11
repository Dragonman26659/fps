using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public Health health;
    public int healAmmount = 20;


    void Start()
    {
        health = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
    }


    void OnTriggerEnter()
    {
        if (health.health < health.MaxHealth)
        {
            health.PlayerHeal(healAmmount);
            Destroy(gameObject);
        }
    }
}
