using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Rendering.Universal.Internal;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(InputReader))]
public class FPCharacterController:MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;

    [Header("Camera")]
    [SerializeField] private float lookSensitivity = 2f;
    [SerializeField] private Transform CameraTransform;
    [SerializeField] private float xRotation;



    private CharacterController controller;
    private InputReader input;
    private void Awake()
    {
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

    void MovePlayer()
    {
        Vector3 Forward = CameraTransform.forward;
        Vector3 Right = CameraTransform.right;

        Vector3 moveDir = (Forward * input.Move.y + Right * input.Move.x).normalized;

        Vector3 velocity = moveDir * moveSpeed;
        velocity.y = -2f;
        
        controller.Move(velocity * Time.deltaTime);
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
