using System;
using UnityEngine;

public class CharacterInteraction : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private LayerMask interactlayer;
    [SerializeField] private Transform Camera;
    [SerializeField] private float range = 3f;

    private InputReader input;
    FPCharacterController controller;

    private void Awake()
    {
        input = GetComponent<InputReader>();
        if (Camera == null)
        {
            var cam = GetComponentInChildren<Camera>();
            if (cam != null) Camera = cam.transform;
        }
    }

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.transform.position, Camera.transform.TransformDirection(Vector3.forward), out hit, range, interactlayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                if (input.InteractPressed)
                {
                    interactable.Interact();
                }
                if (input.PressPressed)
                {
                    interactable.Press();
                }
            }
        }
    } 
}
