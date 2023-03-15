using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    public float lookRadius = 10f;
    public Transform target;
    public Animator animator;
    public GameObject[] ammoPrefabs;
    public float dropChance = 0.5f;

    private NavMeshAgent agent;
    private bool isFollowing = false;
    private bool isDead = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (isDead)
        {
            return;
        }

        float distance = Vector3.Distance(target.position, transform.position);

        if (distance <= lookRadius)
        {
            isFollowing = true;
        }
        else
        {
            isFollowing = false;
        }

        if (isFollowing)
        {
            agent.SetDestination(target.position);

            if (distance <= agent.stoppingDistance)
            {
                animator.SetBool("isWalking", false);
                animator.SetBool("isAttacking", true);

                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Z_Attack"))
                {
                    animator.SetBool("isAttacking", false);
                }
            }
            else
            {
                animator.SetBool("isWalking", true);
                animator.SetBool("isAttacking", false);
            }
        }
        else
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isAttacking", false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }

    public void TakeDamage()
    {
        if (!isDead)
        {
            isDead = true;
            animator.SetBool("isWalking", false);
            animator.SetBool("isAttacking", false);
            animator.SetBool("isDead", true);

            DropAmmo();
            Destroy(gameObject, 2f);
        }
    }

    private void DropAmmo()
    {
        if (ammoPrefabs.Length == 0)
        {
            return;
        }

        float dropRoll = Random.Range(0f, 1f);

        if (dropRoll <= dropChance)
        {
            int randomIndex = Random.Range(0, ammoPrefabs.Length);
            Instantiate(ammoPrefabs[randomIndex], transform.position, Quaternion.identity);
        }
    }
}
