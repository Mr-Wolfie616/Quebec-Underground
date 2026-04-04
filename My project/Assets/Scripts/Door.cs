using System;
using System.Collections.Concurrent;
using UnityEngine;


public class Door : MonoBehaviour, IInteractable
{
     [Header("References")]
    public Transform doorParent; // the actual door object
    public Transform doorPivot;  // hinge position
    public Transform interactPoint;

    [Header("Door Settings")]
    public float openAngle = 90f;
    public float openSpeed = 120f;
    public bool locked = false;

    [Header ("Auto Close")]
    public float openDuration = 5f;
    private float openTimer = 0f;

    private Quaternion startRotation;
    private bool isOpen = false;
    private float currentAngle = 0f;

    void Start()
    {
        startRotation = doorParent.rotation;
    }

    void Update()
    {
        if (doorParent == null || doorPivot == null) return;

        if (isOpen)
        {
            openTimer += Time.deltaTime;

            if (openTimer >= openDuration)
            {
                isOpen = false;
                openTimer = 0f;
            }
        }

        float targetAngle = isOpen ? openAngle : 0f;

        float angleStep = openSpeed * Time.deltaTime;
        float newAngle = Mathf.MoveTowards(currentAngle, targetAngle, angleStep);

        float delta = newAngle - currentAngle;

        doorParent.RotateAround(doorPivot.position, Vector3.up, delta);

        if (interactPoint != null)
            interactPoint.RotateAround(doorPivot.position, Vector3.up, delta);

        currentAngle = newAngle;
    }
   
    public void Interact()
    {
        if (locked) return;
       
        isOpen = true;
        openTimer = 0f;
    }

    public void ForceOpen()
    {
        locked = false;
        isOpen = true;
        openTimer = 0f;
    }

    public void ToggleLock()
    {
        locked = !locked;
    }

    public void Press()
    {
        return;
    }
}
