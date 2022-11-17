using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class SoldierAI : MonoBehaviour
{
    [Header("animation")]
    public Animator soldierAnimator;



    [Header("Sound")]
    //sfx
    public AudioSource WalkSound;
    //chatter
    public AudioSource ChatterLocked;
    public AudioSource ChatterAdvancing;
    public AudioSource ChatterHelp;
    public AudioSource ChatterAknowlaged;



    [Header("AI")]
    public float HealthThreshHold = 50f;
    public NpcHealth HealthScript;
    public SoldierState DefaultState;
    public SoldierState _state;
    public Transform player;
    public float updateSpeed = 0.1f;
    public float IdleLocationRadius = 4f;    
    public LayerMask whatIsPlayer;
    public float sightRange = 7f;
    public float attackRange = 4f;
    public bool radioRequest = false;




    [Header("Line Of Sight Checker")]
    public float radius;
    [Range(30 ,90 )]
    public float angle;
    public GameObject PlayerRef;
    public LayerMask targetMask, obstructionMask;
    public bool canSeePlayer;


    [Header("Gun")]
    public SoldierGun Gun;
    public float ShootDelay = 0.3f;

    

    [Header("Squad")]
    public SoldierAI SquadMember1;
    public SoldierAI SquadMember2;
    public SoldierAI SquadMember3;
    public SoldierAI SquadMember4;


    //private stuff
    private NavMeshAgent Agent;
    private bool playerInSightRange, playerInAttackRange;
    private bool helpRequested, respondedToRequest, locked = false;
    private float ResponceWaitTime = 2f;
    private float ShootWaitTime;






    //soldier states
    public enum SoldierState
    {
        Spawn,
        Idle,
        Attack,
        Hide,
        Patrol,
        Follow,
        Dead
    }






    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        PlayerRef = GameObject.FindGameObjectWithTag("Player");
        Agent = GetComponent<NavMeshAgent>();
        StateManager(_state, DefaultState);
        ShootWaitTime = ShootDelay;
        StartCoroutine(FOVRoutine());
    }




    void Update()
    {
        //check health
        if (HealthScript.health <= 0f)
        {
            Die();
        }




        //check ranges
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);;


        //ajust timers
        if(ShootWaitTime > 0f)
        {
            ShootWaitTime -= Time.deltaTime;
        }



        //set state based on ranges and health
        if (playerInSightRange && !playerInAttackRange && canSeePlayer)
        {
            StateManager(_state, SoldierState.Follow);
            radioRequest = false;
        }
        if (playerInSightRange && playerInAttackRange && canSeePlayer)
        {
            StateManager(_state, SoldierState.Attack);
            radioRequest = false;
        }
        if (!playerInSightRange && !playerInAttackRange)
        {
            if (!radioRequest)
            {
                helpRequested = false;
            }
        }
        if (HealthScript.health <= HealthThreshHold && !helpRequested)
        {
            StateManager(_state, SoldierState.Hide);
        }


        //alert others in squad if player spotted
        if (playerInSightRange && !helpRequested && canSeePlayer)
        {
            ChatterAdvancing.Play();
            AlertSquad(SoldierState.Follow);
        }



        //state mechanics
        if(_state == SoldierState.Follow)
        {
            Agent.SetDestination(player.position);
        }
        if (_state == SoldierState.Attack)
        {
            Attack();
        }
        if (_state == SoldierState.Hide)
        {
            Hide();
        }
        if (_state == SoldierState.Idle)
        {
            IdleMovement();
        }
        if (_state == SoldierState.Dead)
        {
            Agent.enabled = false;
        }


        //animations
        if (Agent.remainingDistance <= Agent.stoppingDistance)
        {
            soldierAnimator.SetBool("IsRunning", false);
        }
        if (Agent.remainingDistance > Agent.stoppingDistance)
        {
            soldierAnimator.SetBool("IsRunning", true);
        }

        //sound
        if (!respondedToRequest && helpRequested && (ResponceWaitTime <= 0f))
        {
            ChatterAknowlaged.Play();
            respondedToRequest = true;
            ResponceWaitTime = 2f;
        }
        if (!respondedToRequest && helpRequested && (ResponceWaitTime > 0f))
        {
            ResponceWaitTime -= Time.deltaTime;
        }

    }



    void StateManager(SoldierState oldState, SoldierState newState)
    {
        if (oldState == newState)
        {
            return;
        }
        else
        {
            _state = newState;
        }

        switch(newState)
        {
            case SoldierState.Idle:
                locked = false;
                break;

            case SoldierState.Patrol:
                locked = false;
                break;

            case SoldierState.Attack:
                break;

            case SoldierState.Hide:
                locked = false;
                break;

            case SoldierState.Follow:
                locked = false;
                break;
            case SoldierState.Dead:
                locked = false;
                break;
        }
    }



    private void AlertSquad(SoldierState state)
    {
        respondedToRequest = true;
        if (SquadMember1 != null)
        {
            SquadMember1._state = state;
            SquadMember1.helpRequested = true;
            SquadMember1.radioRequest = true;
            SquadMember1.respondedToRequest = false;
        }
        if (SquadMember2 != null)
        {
            SquadMember2._state = state;
            SquadMember2.helpRequested = true;
            SquadMember2.radioRequest = true;
            SquadMember2.respondedToRequest = false;
        }
        if (SquadMember3 != null)
        {
            SquadMember3._state = state;
            SquadMember3.helpRequested = true;
            SquadMember3.radioRequest = true;
            SquadMember3.respondedToRequest = false;
        }
        if (SquadMember4 != null)
        {
            SquadMember4._state = state;
            SquadMember4.helpRequested = true;
            SquadMember4.radioRequest = true;
            SquadMember4.respondedToRequest = false;
        }
        helpRequested = true;
    }



    void Attack()
    {
        if (!locked)
        {
            ChatterLocked.Play();
            locked = true;
        }
        if (canSeePlayer)
        {
            Agent.SetDestination(gameObject.transform.position);
            Vector3 lookPos = new Vector3(player.position.x, gameObject.transform.position.y, player.position.z);
            transform.LookAt(lookPos);
            if (ShootWaitTime <= 0f)
            {
                Gun.Shoot();
                ShootWaitTime = ShootDelay;
            }
        }
        else
        {
            _state = SoldierState.Follow;
        }

    }


    void Hide()
    {
        
    }


    void IdleMovement()
    {
        if (Agent.remainingDistance <= Agent.stoppingDistance)
        {
            Vector2 point = Random.insideUnitCircle * IdleLocationRadius;
            NavMeshHit hit;

            if (NavMesh.SamplePosition(Agent.transform.position + new Vector3(point.x, 0, point.y), out hit, 2f, Agent.areaMask))
            {
                Agent.SetDestination(hit.position);
            }
        }
    }



    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            int i = 0;
            foreach(Collider item in rangeChecks) 
            {
                Transform target = rangeChecks[i].transform;
                Vector3 directionToTarget = (target.position - transform.position).normalized;

                if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, target.position);

                    if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                    {
                        canSeePlayer = true;
                    }
                        else
                    {
                        canSeePlayer = false;
                    }
                }
                   else
                {
                    canSeePlayer = false;
                }
                i++;
            }
        }

        else if(canSeePlayer)
        {
            canSeePlayer = false;
        }
    }



    void Die()
    {
        StateManager(_state, SoldierState.Dead);
    }





    //Gizmos
    private void OnDrawGizmosSelected()
    {
        Gizmos.color= Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
