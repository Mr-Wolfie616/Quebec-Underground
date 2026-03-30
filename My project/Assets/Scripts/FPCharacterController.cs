using System.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.Timeline;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(InputReader))]
public class FPCharacterController:MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float backwardSpeed = 2.5f;
    private float speedDebug = 0f;

    [Header("Camera")]
    [SerializeField] private float lookSensitivity = 2f;
    [SerializeField] private Transform CameraTransform;
    [SerializeField] private float xRotation;

    [Header("Crouch")]
    [SerializeField] private float crouchSpeed = 2f;
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 crouchScale = new Vector3 (0.5f, 0.25f, 0.5f);
    private Vector3 standardScale = new Vector3 (0.5f, 0.5f, 0.5f);

    [Header("Audio")]
    [SerializeField] private float crouchFootstepInterval = 0.01f;
    [SerializeField] private float walkFootstepInterval = 0.05f;
    [SerializeField] private float runFootstepInterval = 0.1f;
    private float footstepInterval = 0f;

    private Coroutine footstepRoutine;
    private CharacterController controller;
    private InputReader input;
    private PlayerHideScript playerHide;
    private void Awake()
    {
        playerHide = GetComponent<PlayerHideScript>();
        controller = GetComponent<CharacterController>();
        input = GetComponent<InputReader>();
        if (CameraTransform == null)
        {
            var cam = GetComponentInChildren<Camera>();
            if (cam != null) CameraTransform = cam.transform;
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        MovePlayer();
        PlayerRotation();
    }

    IEnumerator FootStepCoroutine()
    {
        while (true)
        {
            AudioManager.Instance.PlaySound("SFX_Footsteps", transform.position, null);
            yield return new WaitForSeconds(footstepInterval);
        }
    }

    void HandleFootsteps()
    {
        bool isMoving = controller.velocity.magnitude > 0.1f && controller.isGrounded;

        if (isMoving)
        {
            if (footstepRoutine == null)
            {
                footstepRoutine = StartCoroutine(FootStepCoroutine());
            }
        }
        else
        {
            if (footstepRoutine != null)
            {
                StopCoroutine(footstepRoutine);
                footstepRoutine = null;
            }
        }
    }

    void MovePlayer()
    {
        float currentSpeed;

        if (input.Move.y < 0)
        {
            currentSpeed = backwardSpeed;
            footstepInterval = walkFootstepInterval;
        }
        else if (input.Move.y > 0.1f && input.Sprint)
        {
            currentSpeed = sprintSpeed;
            footstepInterval = runFootstepInterval;
        }
        else if ((input.Move.y > 0.1f && input.Crouch) || playerHide.isHiding)
        {
            currentSpeed = crouchSpeed;
            footstepInterval = crouchFootstepInterval;
            //Debug.Log("Player Hidden");
        }
        else
        {
            currentSpeed = moveSpeed;
            footstepInterval = walkFootstepInterval;
        }

        if (input.Crouch || playerHide.isHiding)
        {
            transform.localScale = crouchScale;
            //Debug.Log("Player Hidden 1");
        }
        else
        {
            transform.localScale = standardScale;
        }

        Vector3 Forward = CameraTransform.forward;
        Vector3 Right = CameraTransform.right;

        Vector3 moveDir = (Forward * input.Move.y + Right * input.Move.x).normalized;

        Vector3 velocity = moveDir * currentSpeed;
        velocity.y = -2f;

        controller.Move(velocity * Time.deltaTime);
        speedDebug = controller.velocity.magnitude;
        //Debug.Log(speedDebug);

        HandleFootsteps();
    }

    void PlayerRotation()
    {
        float mouseY = input.Look.y * lookSensitivity * Time.deltaTime;
        float mouseX = input.Look.x * lookSensitivity * Time.deltaTime;
        
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        
        CameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}
