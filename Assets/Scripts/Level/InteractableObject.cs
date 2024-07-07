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

    public OpenDirection openDirection;
    private bool isOpen = false;
    public float openAngle = 90f;
    public float openSpeed = 2f;
    private float epsilon = 0.01f; // small value to compare floats
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Vector3 closedPosition;
    private Vector3 openPosition;

    private void Start()
    {
        closedRotation = transform.localRotation;
        closedPosition = transform.localPosition;

        switch (openDirection)
        {
            case OpenDirection.Left:
                openRotation = Quaternion.Euler(transform.localEulerAngles + new Vector3(0, -openAngle, 0));
                break;
            case OpenDirection.Right:
                openRotation = Quaternion.Euler(transform.localEulerAngles + new Vector3(0, openAngle, 0));
                break;
            case OpenDirection.Drawer:
                openPosition = transform.localPosition + transform.forward * 0.5f; // Adjust 0.5f to the desired slide distance
                break;
        }
    }

    public void Open()
    {
        if (!isOpen)
        {
            StopAllCoroutines();
            if (openDirection == OpenDirection.Drawer)
            {
                StartCoroutine(SlideOpenRoutine());
            }
            else
            {
                StartCoroutine(OpenRoutine());
            }
        }
    }

    public void Close()
    {
        if (isOpen)
        {
            StopAllCoroutines();
            if (openDirection == OpenDirection.Drawer)
            {
                StartCoroutine(SlideCloseRoutine());
            }
            else
            {
                StartCoroutine(CloseRoutine());
            }
        }
    }

    private IEnumerator OpenRoutine()
    {
        while (Quaternion.Angle(transform.localRotation, openRotation) > epsilon)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, openRotation, Time.deltaTime * openSpeed);
            yield return null;
        }
        transform.localRotation = openRotation;
        isOpen = true;
    }

    private IEnumerator SlideOpenRoutine()
    {
        while (Vector3.Distance(transform.localPosition, openPosition) > epsilon)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, openPosition, Time.deltaTime * openSpeed);
            yield return null;
        }
        transform.localPosition = openPosition;
        isOpen = true;
    }

    public void ResetState()
    {
        if (isOpen)
        {
            StopAllCoroutines();
            if (openDirection == OpenDirection.Drawer)
            {
                StartCoroutine(SlideCloseRoutine());
            }
            else
            {
                StartCoroutine(CloseRoutine());
            }
        }
        else
        {
            Open();
        }
    }

    private IEnumerator CloseRoutine()
    {
        while (Quaternion.Angle(transform.localRotation, closedRotation) > epsilon)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, closedRotation, Time.deltaTime * openSpeed);
            yield return null;
        }
        transform.localRotation = closedRotation;
        isOpen = false;
    }

    private IEnumerator SlideCloseRoutine()
    {
        while (Vector3.Distance(transform.localPosition, closedPosition) > epsilon)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, closedPosition, Time.deltaTime * openSpeed);
            yield return null;
        }
        transform.localPosition = closedPosition;
        isOpen = false;
    }
}