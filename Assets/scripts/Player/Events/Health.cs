using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public int MaxHealth;
    public int health = 100;
    public float TimeToStartRegen = 2f;
    private float timeSinseLastDamage;
    private float timeSinseLastHeal;
    public Slider slider;
    public PlayersMovement movementScript;


    void Start()
    {
        MaxHealth = health;
        slider.value = health;
    }

    // Update is called once per frame
    void Update()
    {
        timeSinseLastDamage += Time.deltaTime;
        timeSinseLastHeal += Time.deltaTime;

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

        if (timeSinseLastDamage >= TimeToStartRegen && movementScript.stamina >= movementScript.maxStamina)
        {
            PlayerHeal(1);
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        timeSinseLastDamage = 0f;
        slider.value = health;

}

    public void PlayerHeal(int ammount)
    {
        if (timeSinseLastHeal >= 0.02 && health < MaxHealth)
        {
            health += ammount;
            timeSinseLastHeal = 0f;
            slider.value = health;
        }
    }
}
