using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(PlayerInput))]
public class InputReader : MonoBehaviour
{
    public Vector2 Move { get; private set;}
    public Vector2 Look { get; private set;}

    public bool CrouchPressed { get; private set;}

    public int CrouchedPressCount { get; private set;}

    private PlayerInput playerInput;

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction crouchAction;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        var gameplay = playerInput.actions.FindActionMap("Gameplay", true);
        moveAction = gameplay.FindAction("Move", true);
        lookAction = gameplay.FindAction("Look", true);
        crouchAction = gameplay.FindAction("Crouch", true);
    }

    private void OnEnable()
    {
        playerInput.actions.FindActionMap("Gameplay", true).Enable();

        moveAction.performed += OnMove;
        moveAction.canceled += OnMove;

        lookAction.performed += OnLook;
        lookAction.canceled += OnLook;

        crouchAction.started += OnCrouch;
    }

    private void OnDisable()
    {
        moveAction.performed -= OnMove;
        moveAction.canceled -= OnMove;

        lookAction.performed -= OnLook;
        lookAction.canceled -= OnLook;

        crouchAction.started -= OnCrouch;
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        Move = ctx.ReadValue<Vector2>();
    }

    private void OnLook(InputAction.CallbackContext ctx)
    {
        Look = ctx.ReadValue<Vector2>();
    }

    private void OnCrouch(InputAction.CallbackContext ctx)
    {
        CrouchPressed = true;
        CrouchedPressCount++;
    }

    public void ResetCrouchPressed() => CrouchPressed = false;

    public void Clear()
    {
        Move = Vector2.zero;
        Look = Vector2.zero;
        CrouchPressed = false;
    }

    private void OnGUI()
    {
        var cam = playerInput.camera;
        if (cam == null) cam = GetComponentInChildren<Camera>(true);
        if (cam == null) cam = Camera.main;
        if (cam == null) return;

        Rect pr = cam.pixelRect;

        float pad = 10f;
        float w = 320f;
        float h = 120f;

        var area = new Rect(pr.xMin + pad, pr.yMin + pad, w, h );

        GUILayout.BeginArea(area, GUI.skin.box);
        GUILayout.Label($"P{playerInput.playerIndex} Scheme: {playerInput.currentControlScheme}");

        GUILayout.Label($"Move: {Move}");
        GUILayout.Label($"Look: {Look}");
        GUILayout.Label($"CrouchPressed: {CrouchPressed} (Count: {CrouchedPressCount})");
        GUILayout.EndArea();
    }
}

