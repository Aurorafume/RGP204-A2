using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // Public variables for player movement and interaction settings
    public float playerSpeed;
    public float groundDrag;
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    public float crouchHeight = 0.5f;
    public float crouchSpeed = 10f;
    public float playerHeight;

    // Private variables for managing player state
    private bool isCrouching = false;
    bool readyToJump;
    bool grounded;
    float horizontalInput;
    float verticalInput;

    // Public variables for key bindings
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode crouchKey = KeyCode.LeftShift;

    // Public variables for managing ground detection and orientation
    public LayerMask whatIsGround;
    public Transform orientation;
    Vector3 moveDirection;
    Rigidbody rb;
    private Vector3 originalScale;

    // Public variables for interaction
    public Camera playerCamera;
    public Text pickupText;
    public Text openText;
    public float interactionRange = 3f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        originalScale = transform.localScale;

        playerCamera = Camera.main;
        pickupText.enabled = false;
        openText.enabled = false;
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround); // Check if the player is grounded
        MyInput();
        SpeedControl();
        HandleCrouch();
        
        rb.drag = grounded ? groundDrag : 0; // Change the drag value based on player state

        CheckForClick();
        CheckForPickupOrOpen();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false; // Set the jump cooldown
            Jump(); // Jump
            Invoke(nameof(ResetJump), jumpCooldown); // Reset the jump cooldown
        }
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput; // Calculate the move direction
        
        if (grounded)
            rb.AddForce(moveDirection.normalized * playerSpeed * 10f, ForceMode.Force); // Move the player
        else
            rb.AddForce(moveDirection.normalized * playerSpeed * 10f * airMultiplier, ForceMode.Force); // Move the player in the air
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // Calculate the flat velocity
         
        if (flatVel.magnitude > playerSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * playerSpeed; // Limit the velocity
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z); // Set the limited velocity
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // Reset the vertical velocity
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse); // Add the jump force
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void HandleCrouch()
    {
        if (Input.GetKeyDown(crouchKey)) // Check if the crouch key is pressed
        {
            if (!isCrouching)
            {
                isCrouching = true; // Set the crouching state
                StopAllCoroutines(); // Stop all coroutines
                StartCoroutine(Crouch());  // Start the crouch coroutine
            }
        } 
        else if (Input.GetKeyUp(crouchKey)) // Check if the crouch key is released
        {
            if (isCrouching)
            {
                isCrouching = false; // Reset the crouching state
                StopAllCoroutines(); // Stop all coroutines
                StartCoroutine(Uncrouch()); // Start the uncrouch coroutine
            }
        }
    }

    private IEnumerator Crouch()
    {
        Vector3 targetScale = new Vector3(originalScale.x, crouchHeight, originalScale.z); // Calculate the target scale
        
        while (transform.localScale.y > crouchHeight)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, crouchSpeed * Time.deltaTime); // Lerp the scale
            yield return null; // Wait for the next frame
        }
        
        transform.localScale = targetScale; // Set the scale to the target scale
    }

    private IEnumerator Uncrouch()
    {
        Vector3 targetScale = originalScale; // Calculate the target scale
        
        while (transform.localScale.y < originalScale.y)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, crouchSpeed * Time.deltaTime); // Lerp the scale
            yield return null; // Wait for the next frame
        }
        
        transform.localScale = targetScale; // Set the scale to the target scale
    }

    private void CheckForClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition); // Cast a ray from the camera to the mouse position
            
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("RepairKit"))
                {
                    FindObjectOfType<GameManager>().CollectRepairKit(); // Call the CollectRepairKit function
                    Destroy(hit.collider.gameObject); // Destroy the repair kit
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition); // Cast a ray from the camera to the mouse position
            
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("Interactable"))
                {
                    var interactable = hit.collider.GetComponent<InteractableObject>(); // Get the interactable object component
                    
                    if (interactable != null)
                    {
                        interactable.ResetState(); // Call the ResetState function
                    }
                }
            }
        }
    }

    private void CheckForPickupOrOpen()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition); // Cast a ray from the camera to the mouse position
        
        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange))
        {
            if (hit.collider.CompareTag("RepairKit"))
            {
                pickupText.enabled = true; // Enable the pickup text
                openText.enabled = false; // Disable the open text
            }
            else if (hit.collider.CompareTag("Interactable"))
            {
                openText.enabled = true; // Enable the open text
                pickupText.enabled = false; // Disable the pickup text
            }
            else
            {
                pickupText.enabled = false; // Disable the pickup text
                openText.enabled = false; // Disable the open text
            } 
        }
        else
        {
            pickupText.enabled = false; // Disable the pickup text
            openText.enabled = false; // Disable the open text
        }
    }
}
