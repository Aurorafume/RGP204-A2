using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InteractableObject : MonoBehaviour
{
    private bool isOpen = false;
    public float openAngle = 90f;
    public float openSpeed = 2f;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    private void Start()
    {
        closedRotation = transform.rotation;
        openRotation = Quaternion.Euler(transform.eulerAngles + Vector3.up * openAngle);
    }

    public void Open()
    {
        if (!isOpen)
        {
            StopAllCoroutines();
            StartCoroutine(OpenRoutine());
        }
    }

    private IEnumerator OpenRoutine()
    {
        while (Quaternion.Angle(transform.rotation, openRotation) > 0.01f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, openRotation, Time.deltaTime * openSpeed);
            yield return null;
        }
        transform.rotation = openRotation;
        isOpen = true;
    }
}