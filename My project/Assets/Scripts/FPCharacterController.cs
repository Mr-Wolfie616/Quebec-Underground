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
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float sprintSpeed = 2.5f;
    [SerializeField] private float backwardSpeed = 1f;
    private float speedDebug = 0f;

    [Header("Camera")]
    [SerializeField] private float lookSensitivity = 5f;
    [SerializeField] private Transform CameraTransform;
    [SerializeField] private float xRotation;

    [Header("Crouch")]
    [SerializeField] private float crouchSpeed = 1f;
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 crouchScale = new Vector3 (0.3f, 0.25f, 0.3f);
    private Vector3 standardScale = new Vector3 (0.5f, 0.5f, 0.5f);
    public bool isCrouching = false;

    [Header("Audio")]
    [SerializeField] private float crouchFootstepInterval;
    [SerializeField] private float walkFootstepInterval;
    [SerializeField] private float runFootstepInterval;
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
        bool isMoving = input.Move.sqrMagnitude > 0.01f && controller.isGrounded;

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
        isCrouching = input.Crouch;

        if (input.Move.y < 0)
        {
            currentSpeed = backwardSpeed;
            footstepInterval = walkFootstepInterval;
        }
        else if (input.Move.y > 0.1f && input.Sprint && !input.Crouch)
        {
            currentSpeed = sprintSpeed;
            footstepInterval = runFootstepInterval;
        }
        else if ((input.Move.y > 0.1f && input.Crouch) || playerHide.isHiding && !input.Sprint)
        {
            currentSpeed = crouchSpeed;
            footstepInterval = crouchFootstepInterval;
        }
        else
        {
            currentSpeed = moveSpeed;
            footstepInterval = walkFootstepInterval;
        }

        if (input.Crouch || playerHide.crouchHide)
        {
            transform.localScale = crouchScale;
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
