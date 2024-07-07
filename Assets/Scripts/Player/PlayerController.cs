using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float playerSpeed; // player's max speed
    public float groundDrag; // drag when player is on the ground
    public float jumpForce; // force applied when player jumps
    public float jumpCooldown; // time between jumps
    public float airMultiplier; // how much faster player moves in the air
    public float crouchHeight = 0.5f; // height of the player when crouching
    public float crouchSpeed = 10f; // speed at which player crouches
    public float playerHeight; // player's height
    private bool isCrouching = false; // is the player crouching
    bool readyToJump; // is the player ready to jump
    bool grounded; // is the player on the ground
    float horizontalInput; // horizontal input from player
    float verticalInput; // vertical input from player

    [HideInInspector] public float walkSpeed; // player's walk speed
    [HideInInspector] public float sprintSpeed; // player's sprint speed

    public KeyCode jumpKey = KeyCode.Space; // key to jump
    public KeyCode crouchKey = KeyCode.LeftShift; // key to crouch

    public LayerMask whatIsGround; // layer mask for ground
    public Transform orientation; // player's orientation
    Vector3 moveDirection; // direction to move the player
    Rigidbody rb; // player's rigidbody
    private Vector3 originalScale; // original scale of the player

    public Camera playerCamera; // camera used for raycasting
    public Text pickupText; // UI text element for pickup prompt
    public Text openText; // UI text element for open prompt
    public float interactionRange = 3f; // range within which the player can interact with objects

    private void Start()
    {
        rb = GetComponent<Rigidbody>(); // get the player's rigidbody
        rb.freezeRotation = true; // freeze rotation
        readyToJump = true; // set readyToJump to true
        originalScale = transform.localScale; // store the original scale of the player

        playerCamera = Camera.main; // get the main camera
        pickupText.enabled = false; // initially hide the pickup text
        openText.enabled = false; // initially hide the open text
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround); // check if player is grounded
        MyInput(); // get player input
        SpeedControl(); // control player speed
        HandleCrouch(); // handle crouching
        if (grounded)
            rb.drag = groundDrag; // if player is grounded, add ground drag
        else
            rb.drag = 0; // else, remove ground drag

        CheckForClick(); // check for mouse click
        CheckForPickupOrOpen(); // check if player is looking at a repair kit or interactable object
    }

    private void FixedUpdate()
    {
        MovePlayer(); // move the player
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal"); // get horizontal input
        verticalInput = Input.GetAxisRaw("Vertical"); // get vertical input
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false; // if player presses jump key, set readyToJump to false
            Jump(); // jump
            Invoke(nameof(ResetJump), jumpCooldown); // set a delay before player can jump again
        }
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput; // get the direction to move the player
        if (grounded)
            rb.AddForce(moveDirection.normalized * playerSpeed * 10f, ForceMode.Force); // if the player is grounded, move the player
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * playerSpeed * 10f * airMultiplier, ForceMode.Force); // if the player is not grounded, move the player faster
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // get the player's velocity
        if (flatVel.magnitude > playerSpeed)
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

    private void HandleCrouch()
    {
        if (Input.GetKeyDown(crouchKey))
        {
            if (!isCrouching)
            {
                isCrouching = true; // set isCrouching to true
                StopAllCoroutines(); // stop all coroutines
                StartCoroutine(Crouch()); // start crouching
            }
        }
        else if (Input.GetKeyUp(crouchKey))
        {
            if (isCrouching)
            {
                isCrouching = false; // set isCrouching to false
                StopAllCoroutines(); // stop all coroutines
                StartCoroutine(Uncrouch()); // start uncrouching
            }
        }
    }

    private IEnumerator Crouch()
    {
        Vector3 targetScale = new Vector3(originalScale.x, crouchHeight, originalScale.z); // set the target scale
        while (transform.localScale.y > crouchHeight)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, crouchSpeed * Time.deltaTime); // lerp the scale
            yield return null; // wait for the next frame
        }
        transform.localScale = targetScale; // set the scale to the target scale
    }

    private IEnumerator Uncrouch()
    {
        Vector3 targetScale = originalScale; // set the target scale
        while (transform.localScale.y < originalScale.y)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, crouchSpeed * Time.deltaTime); // lerp the scale
            yield return null; // wait for the next frame
        }
        transform.localScale = targetScale; // set the scale to the target scale
    }

    private void CheckForClick()
    {
        if (Input.GetMouseButtonDown(0)) // Check for left mouse button click
        {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("RepairKit"))
                {
                    FindObjectOfType<GameManager>().CollectRepairKit(); // Call CollectRepairKit function from GameManager
                    Destroy(hit.collider.gameObject); // Destroy the repair kit
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.E)) // Check for 'E' key press
        {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("Interactable"))
                {
                    var interactable = hit.collider.GetComponent<InteractableObject>();
                    if (interactable != null)
                    {
                        interactable.ResetState();
                    }
                }
            }
        }
    }

    private void CheckForPickupOrOpen()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange))
        {
            if (hit.collider.CompareTag("RepairKit"))
            {
                pickupText.enabled = true;
                openText.enabled = false;
            }
            else if (hit.collider.CompareTag("Interactable"))
            {
                openText.enabled = true;
                pickupText.enabled = false;
            }
            else
            {
                pickupText.enabled = false;
                openText.enabled = false;
            }
        }
        else
        {
            pickupText.enabled = false;
            openText.enabled = false;
        }
    }
}