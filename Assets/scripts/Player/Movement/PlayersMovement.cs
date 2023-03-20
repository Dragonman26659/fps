using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayersMovement : MonoBehaviour
{

    public Slider slider;
    private float wait;
    public Health PlayerHealth;
    public coins CoinScript;

    [Header("Movement")]

    //floats
    private float moveSpeed;
    public float walkSpeed;
    public float wallRunSpeed;
    float horizontalInput;
    float VerticalInput;
    public float groundDrag;


    //jump
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump = true;

    //others
    public Transform orientation;
    Vector3 moveDirection;
    Vector3 slideDirection;
    Rigidbody rb;
    public MovementState state;


    [Header("Crouching")]
    public float chrouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("GroundCheck")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    [Header("KeyBinds")]
    public KeyCode jumpkey = KeyCode.Space;
    public KeyCode sprintkey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Sound")]
    public AudioSource walkNoise;
    public AudioSource jumpNoise;
    public AudioSource crouchNoise;




    [Header("Abilities")]
    public float slamForce = 10f;
    public float slideSpeed = 10f;
    public float slideTime = 2f;
    public float currentSlideTime = 0f;
    public int maxStamina = 100;
    public float StartRegenAfter = 1f;
    public int stamina = 0;
    private float nextStaminaMinus;
    private float timeSinceLastRegen;
    private float slideWait;
    private float MaxSlideForce;
    public float sprintSpeed;
    private int JumpCounter = 1;
    private int CurrentJumpCounter = 0;
    private int JumpStamina = 10;
    public float DashPower = 30;
    private int dashCount = 1;
    private int DashStamina = 10;
    private int CurrentDashCounter = 0;
    private int HealthLeachAmmount = 10;
    private float HealthLeachCooldown = 1f;
    private float CurrentHealthLeachCooldown = 0f;
    public int doubleJumpAmmount = 0;
    public int HealthLeachAmmt = 20;
    public int DashAmmount;

    [Header("Movement Upgrades")]
    public GameObject doubleJumpUpgrade;
    public GameObject dashUpgrade;
    public GameObject HealthLeachUpgrade;



    public enum MovementState
    {
        walking,
        sprinting,
        air,
        chrouching,
        sliding,
        wallrunning
    }


    public bool wallrunning;




    //set rigidbody
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

        startYScale = transform.localScale.y;
        MaxSlideForce = slideSpeed;

        if (doubleJumpUpgrade.activeSelf)
        {
            JumpCounter += doubleJumpAmmount;
        }
        if (dashUpgrade.activeSelf)
        {
            dashCount = DashAmmount;
        }
        else
        {
            dashCount = 0;
        }
        if (HealthLeachUpgrade.activeSelf)
        {
            HealthLeachAmmt = HealthLeachAmmt;
        }
        else
        {
            HealthLeachAmmount = 0;
        }
        
        CurrentJumpCounter = JumpCounter;
        stamina = maxStamina;
    }


    //updates each frame
    void Update()
    {
        slider.value = stamina;


        //check for upgrades
        if (doubleJumpUpgrade.activeSelf)
        {
            if (grounded)
            {
                JumpCounter = doubleJumpAmmount + 1;
            }
        }
        if (dashUpgrade.activeSelf)
        {
            if (grounded)
            {
                dashCount = DashAmmount;
            }
        }
        if (HealthLeachUpgrade.activeSelf)
        {
            HealthLeachAmmt = HealthLeachAmmt;
        }


        nextStaminaMinus += Time.deltaTime;
        timeSinceLastRegen += Time.deltaTime;
        wait += Time.deltaTime;
        CurrentHealthLeachCooldown += Time.deltaTime;

        //ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        //drag
        if(grounded)
        {
            rb.drag = groundDrag;
        }
            
        else
        {
            rb.drag = 0;
        }

        //function
        StateHandeler();
        MyInput();
        SpeedControll();

        if (nextStaminaMinus >= StartRegenAfter)
        {
            if (timeSinceLastRegen >= 0.2f && state != MovementState.sliding || state != MovementState.sprinting)
            {
                if (wait > 0.5f)
                {
                    stamina += 1;
                    wait = 0f;
                }

            }
        }
        if (stamina > maxStamina)
        {
            stamina = maxStamina;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }


    // get all needed inputs
    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        VerticalInput = Input.GetAxisRaw("Vertical");
        if (grounded)
        {
            CurrentJumpCounter = JumpCounter;
            CurrentDashCounter = dashCount;
        }

        //jump
        if (Input.GetKey(jumpkey) && readyToJump && CurrentJumpCounter > 0)
        {
            Jump();
            readyToJump = false;
            Invoke(nameof(resetJump), jumpCooldown);
        }
        if (state == MovementState.air && Input.GetKeyDown(sprintkey))
            {
                if (CurrentDashCounter > 0 && stamina > 0)
                {
                    rb.AddForce(orientation.forward * DashPower, ForceMode.Impulse);
                    CurrentDashCounter --;
                    stamina -= DashStamina;
                    jumpNoise.Play();
                }
            }
        if (Input.GetKeyDown(crouchKey))
        {

            crouchNoise.Play();
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);

            if (state == MovementState.sprinting)
            {
                if (state != MovementState.sliding)
                {
                    currentSlideTime = slideTime;
                    slideSpeed = MaxSlideForce;
                }
                state = MovementState.sliding;
            }
            if (grounded && state != MovementState.sliding)
            {
                state = MovementState.chrouching;
                rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
            }
            else if (state == MovementState.air && !Input.GetKeyDown(sprintkey))
            {
                rb.AddForce(Vector3.down * slamForce, ForceMode.Impulse);
                state = MovementState.chrouching;
            }
        }
            

        if (Input.GetKeyUp(crouchKey))
        {
            crouchNoise.Play();
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
            if (grounded)
            {
                rb.AddForce(Vector3.up * 3f, ForceMode.Impulse);
            }
        }

        if (Input.GetMouseButton(1) && CurrentHealthLeachCooldown > HealthLeachCooldown)
        {
            PlayerHealth.TakeDamage(HealthLeachAmmount);
            int newstamina = stamina + HealthLeachAmmount;
            int coins = newstamina - maxStamina;
            if (coins >= 0)
            {
                CoinScript.CoinAmmount += coins;
            }
            stamina = newstamina;
            CurrentHealthLeachCooldown = 0;
        }
    }

    //moves player
    private void MovePlayer()
    {
        //calculate movement direction
        moveDirection = orientation.forward * VerticalInput + orientation.right * horizontalInput;

        //on ground
        if (grounded)
        {
            //walkNoise.Play();
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if (!grounded)
        {
            //walkNoise.Stop();
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }


    private void SpeedControll()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        //limit velocity
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }


    private void Jump()
    {
        if (grounded || stamina > 0)
        {
            //reset y veleocity
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            jumpNoise.Play();
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            if (!grounded)
            {
                stamina -= JumpStamina;
            }
            CurrentJumpCounter -= 1;
        }
    }


    private void resetJump()
    {
        readyToJump = true;
    }


    private void StateHandeler()
    {
        //wallrun

        if (wallrunning)
        {
            state = MovementState.wallrunning;
            moveSpeed = wallRunSpeed;
        }




        //sprint
        if(grounded && Input.GetKey(sprintkey) && !Input.GetKey(crouchKey) && stamina > 0)
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
            if (nextStaminaMinus > 0.1)
            {
                stamina -= 2;
                if (stamina < 0)
                {
                    stamina = 0;
                }
                nextStaminaMinus = 0f;
            }
        }

        //walking
        else if (grounded && !Input.GetKey(crouchKey))
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }

        //chrouch
        if (state == MovementState.chrouching)
        {
            state = MovementState.chrouching;
            moveSpeed = chrouchSpeed;
        }

        if ( state == MovementState.sliding)
        {
            rb.AddForce(orientation.forward * slideSpeed * 10f, ForceMode.Force);
            slideWait -= Time.deltaTime;
            if (slideWait <= 0f)
            {
                slideSpeed -= slideTime;
                slideWait = 0.1f;
            }
            if (nextStaminaMinus > 0.1)
            {
                stamina -= 1;
                if (stamina < 0)
                {
                    stamina = 0;
                }
                nextStaminaMinus = 0f;
            }
            if (slideSpeed <= chrouchSpeed || stamina <= 0f)
            {
                state = MovementState.chrouching;
            }
        }

        //air
        if (!grounded)
        {
            state = MovementState.air;
        }


    }
}
