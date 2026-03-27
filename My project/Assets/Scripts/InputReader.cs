
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(PlayerInput))]
public class InputReader : MonoBehaviour
{
    public Vector2 Move { get; private set;}
    public Vector2 Look { get; private set;}

    public bool Crouch { get; private set;}
    public bool Sprint { get; private set;}

    public bool Interact {get; private set;}

    private PlayerInput playerInput;

    private InputAction moveAction;
    private InputAction sprintAction;
    private InputAction lookAction;
    private InputAction crouchAction;
    private InputAction interactAction;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        var gameplay = playerInput.actions.FindActionMap("Gameplay", true);
        moveAction = gameplay.FindAction("Move", true);
        sprintAction = gameplay.FindAction("Sprint", true);
        lookAction = gameplay.FindAction("Look", true);
        crouchAction = gameplay.FindAction("Crouch", true);
        interactAction = gameplay.FindAction("Interact", true);
    }

    private void OnEnable()
    {
        playerInput.actions.FindActionMap("Gameplay", true).Enable();

        moveAction.performed += OnMove;
        moveAction.canceled += OnMove;

        sprintAction.performed += OnSprint;
        sprintAction.canceled += OnSprint;

        lookAction.performed += OnLook;
        lookAction.canceled += OnLook;

       crouchAction.performed += OnCrouch;
       crouchAction.canceled += OnCrouch;

       interactAction.performed += OnInteract;
       interactAction.canceled += OnInteract;
    }

    private void OnDisable()
    {
        moveAction.performed -= OnMove;
        moveAction.canceled -= OnMove;

        sprintAction.performed -= OnSprint;
        sprintAction.canceled -= OnSprint;

        lookAction.performed -= OnLook;
        lookAction.canceled -= OnLook;

        crouchAction.performed -= OnCrouch;
        crouchAction.canceled -= OnCrouch;

        interactAction.performed -= OnInteract;
        interactAction.canceled -=  OnInteract;
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        Move = ctx.ReadValue<Vector2>();
    }

    private void OnSprint(InputAction.CallbackContext ctx)
    {
        Sprint = ctx.ReadValueAsButton();
    }

    private void OnLook(InputAction.CallbackContext ctx)
    {
        Look = ctx.ReadValue<Vector2>();
    }

    private void OnCrouch(InputAction.CallbackContext ctx)
    {
        Crouch = ctx.ReadValueAsButton();
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        Interact = ctx.ReadValueAsButton();
    }

    public void Clear()
    {
        Move = Vector2.zero;
        Look = Vector2.zero;
        Crouch = false;
        Sprint = false;
        Interact = false;
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
        GUILayout.Label($"Sprint: {Sprint}");
        GUILayout.Label($"Crouch: {Crouch}");
        GUILayout.EndArea();
    }
}

