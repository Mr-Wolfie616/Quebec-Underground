using System;
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

    private bool isOpen = false;
    private float currentAngle = 0f;

    void Update()
    {
        if (doorParent == null || doorPivot == null) return;

        float targetAngle = isOpen ? openAngle : 0f;

        float angleStep = openSpeed * Time.deltaTime;
        float newAngle = Mathf.MoveTowards(currentAngle, targetAngle, angleStep);

        float delta = newAngle - currentAngle;

        doorParent.RotateAround(
            doorPivot.position,
            Vector3.up,
            delta
        );

        interactPoint.RotateAround(
            doorPivot.position,
            Vector3.up,
            delta
        );

        currentAngle = newAngle;
    }
   

    // Called by your interaction system
    public void Interact()
    {
        isOpen = !isOpen;
    }
}
