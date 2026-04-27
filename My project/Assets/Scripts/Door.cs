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
                AudioManager.Instance.PlaySound("SFX_Door_Close", transform.position, null);
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

        Open();
    }

    public void ForceOpen()
    {
        locked = false;
        Open();
    }

    public void NPCOpen()
    {
        Open();
    }

    private void Open()
    {
        if (!isOpen)
        {
            AudioManager.Instance.PlaySound("SFX_Door_Open", transform.position, null);
        }

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
