using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float playerSpeed; // player's max speed
    public float groundDrag; // drag when player is on the ground
    public float jumpForce; // force applied when player jumps
    public float jumpCooldown; // time between jumps
    public float airMultiplier; // how much faster player moves in the air
    bool readyToJump; // is the player ready to jump

    [HideInInspector] public float walkSpeed; // player's walk speed
    [HideInInspector] public float sprintSpeed; // player's sprint speed

    public KeyCode jumpKey = KeyCode.Space; // key to jump

    public float playerHeight; // player's height
    public LayerMask whatIsGround; // layer mask for ground
    bool grounded; // is the player on the ground

    public Transform orientation; // player's orientation
    float horizontalInput; // horizontal input from player
    float verticalInput; // vertical input from player
    Vector3 moveDirection; // direction to move the player
    Rigidbody rb; // player's rigidbody

    private void Start()
    {
        rb = GetComponent<Rigidbody>(); // get the player's rigidbody
        rb.freezeRotation = true; // freeze rotation
        readyToJump = true; // set readyToJump to true
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround); // check if player is grounded
        MyInput(); // get player input
        SpeedControl(); // control player speed
        if (grounded)
            rb.drag = groundDrag; // if player is grounded, add ground drag
        else
            rb.drag = 0; // else, remove ground drag
    }

    private void FixedUpdate()
    {
        MovePlayer(); // move the player
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal"); // get horizontal input
        verticalInput = Input.GetAxisRaw("Vertical"); // get vertical input
        if(Input.GetKey(jumpKey) && readyToJump && grounded) 
        {
            readyToJump = false; // if player presses jump key, set readyToJump to false
            Jump(); // jump
            Invoke(nameof(ResetJump), jumpCooldown); // set a delay before player can jump again
        }
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput; // get the direction to move the player
        if(grounded)
            rb.AddForce(moveDirection.normalized * playerSpeed * 10f, ForceMode.Force); // if the player is grounded, move the player
        else if(!grounded)
            rb.AddForce(moveDirection.normalized * playerSpeed * 10f * airMultiplier, ForceMode.Force); // if the player is not grounded, move the player faster
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // get the player's velocity
        if(flatVel.magnitude > playerSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * playerSpeed; // if the player's velocity is greater than the max speed, limit the velocity
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z); // set the player's velocity
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // reset the player's velocity
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse); // add force to make the player jump
    }
    
    private void ResetJump()
    {
        readyToJump = true; // set readyToJump to true
    }
}