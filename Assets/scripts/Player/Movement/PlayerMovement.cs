using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]

    //floats
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
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


    public enum MovementState
    {
        walking,
        sprinting,
        air,
        chrouching
    }




    //set rigidbody
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

        startYScale = transform.localScale.y;
    }


    //updates each frame
    void Update()
    {
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


        //jump
        if (Input.GetKey(jumpkey) && readyToJump && grounded)
        {
            Jump();
            readyToJump = false;
            Invoke(nameof(resetJump), jumpCooldown);
        }
        if (Input.GetKeyDown(crouchKey))
        {
            crouchNoise.Play();
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            if (grounded)
            {
                rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
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
        //reset y veleocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        jumpNoise.Play();
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }


    private void resetJump()
    {
        readyToJump = true;
    }


    private void StateHandeler()
    {
        //sprint
        if(grounded && Input.GetKey(sprintkey))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }

        //walking
        else if (grounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }

        //chrouch
        else if (Input.GetKey(crouchKey))
        {
            state = MovementState.chrouching;
            moveSpeed = chrouchSpeed;
        }



        //air
        else
        {
            state = MovementState.air;
        }


    }
}
