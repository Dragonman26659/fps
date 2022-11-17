using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcHealth : MonoBehaviour
{
    public float MaxHealth;
    public float health = 100f;
    public float velldamage = 40f;
    public bool CanTakeRbDamage = false;

    private void OnCollisionEnter(Collision coll)
    {
        if (CanTakeRbDamage)
        {
            Rigidbody rb = coll.gameObject.GetComponent<Rigidbody>();
            if (rb != null && rb.velocity.y >= velldamage || rb.velocity.x >= velldamage || rb.velocity.z >= velldamage)
            {
                TakeDamage(velldamage);
            }
        }
    }



    void Start()
    {
        MaxHealth = health;
    }

    public void TakeDamage(float damage)
    {
        if (health > 0f)
        {
            health -= damage;
        }
    }
}