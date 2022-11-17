using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public int MaxHealth;
    public int health = 100;
    public Text healthDisp;


    void Start()
    {
        MaxHealth = health;
    }

    // Update is called once per frame
    void Update()
    {
        healthDisp.text = health + "/" + MaxHealth;
        if (health <= 0)
        {
            health = 0;
            Destroy(gameObject);
            Time.timeScale = 0.1f;
        }
        if (health > MaxHealth)
        {
            health = MaxHealth;
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
    }

    public void PlayerHeal(int ammount)
    {
        health += ammount;
    }
}
