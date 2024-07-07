using UnityEngine;
using System.Collections;

public class InteractableObject : MonoBehaviour
{
    public enum OpenDirection
    {
        Left,
        Right,
        Drawer
    }

    // Public variables for the door settings
    public OpenDirection openDirection;
    private bool isOpen = false;
    public float openAngle = 90f;
    public float openSpeed = 2f;
    private float epsilon = 0.01f; 
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Vector3 closedPosition;
    private Vector3 openPosition;

    private void Start()
    {
        closedRotation = transform.localRotation; // Set the closed rotation to the initial rotation
        closedPosition = transform.localPosition; // Set the closed position to the initial position

        switch (openDirection)
        {
            case OpenDirection.Left:
                openRotation = Quaternion.Euler(transform.localEulerAngles + new Vector3(0, -openAngle, 0)); // Set the open rotation to the left
                break;
            case OpenDirection.Right:
                openRotation = Quaternion.Euler(transform.localEulerAngles + new Vector3(0, openAngle, 0)); // Set the open rotation to the right
                break;
            case OpenDirection.Drawer:
                openPosition = transform.localPosition + transform.forward * 0.5f; // Set the open position for the drawer
                break;
        }
    }

    public void Open()
    {
        if (!isOpen)
        {
            StopAllCoroutines(); // Stop all coroutines
            if (openDirection == OpenDirection.Drawer)
            {
                StartCoroutine(SlideOpenRoutine()); // Start the slide open coroutine
            }
            else
            {
                StartCoroutine(OpenRoutine()); // Start the open coroutine
            }
        }
    }

    public void Close()
    {
        if (isOpen)
        {
            StopAllCoroutines(); // Stop all coroutines
            if (openDirection == OpenDirection.Drawer)
            {
                StartCoroutine(SlideCloseRoutine()); // Start the slide close coroutine
            }
            else
            {
                StartCoroutine(CloseRoutine()); // Start the close coroutine
            }
        }
    }

    private IEnumerator OpenRoutine()
    {
        while (Quaternion.Angle(transform.localRotation, openRotation) > epsilon)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, openRotation, Time.deltaTime * openSpeed); // Slerp the rotation
            yield return null; // Wait for the next frame
        }
        transform.localRotation = openRotation; // Set the rotation to the open rotation
        isOpen = true; // Set the open state to true
    }

    private IEnumerator SlideOpenRoutine()
    {
        while (Vector3.Distance(transform.localPosition, openPosition) > epsilon)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, openPosition, Time.deltaTime * openSpeed); // Lerp the position
            yield return null; // Wait for the next frame
        }
        transform.localPosition = openPosition; // Set the position to the open position
        isOpen = true; // Set the open state to true
    } 

    public void ResetState()
    {
        if (isOpen)
        {
            StopAllCoroutines(); // Stop all coroutines
            if (openDirection == OpenDirection.Drawer)
            { 
                StartCoroutine(SlideCloseRoutine()); // Start the slide close coroutine
            }
            else
            {
                StartCoroutine(CloseRoutine()); // Start the close coroutine
            }
        }
        else
        {
            Open(); // Open the door
        }
    }

    private IEnumerator CloseRoutine()
    {
        while (Quaternion.Angle(transform.localRotation, closedRotation) > epsilon)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, closedRotation, Time.deltaTime * openSpeed); // Slerp the rotation
            yield return null; // Wait for the next frame
        }
        transform.localRotation = closedRotation; // Set the rotation to the closed rotation
        isOpen = false; // Set the open state to false
    }

    private IEnumerator SlideCloseRoutine()
    {
        while (Vector3.Distance(transform.localPosition, closedPosition) > epsilon)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, closedPosition, Time.deltaTime * openSpeed); // Lerp the position
            yield return null; // Wait for the next frame
        }
        transform.localPosition = closedPosition; // Set the position to the closed position
        isOpen = false; // Set the open state to false
    } 
}