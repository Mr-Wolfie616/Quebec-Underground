
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

    public bool InteractPressed { get; private set; }
    public bool PressPressed { get; private set; }

    public bool pausePressed {get; private set;}

    private PlayerInput playerInput;

    private InputAction moveAction;
    private InputAction sprintAction;
    private InputAction lookAction;
    private InputAction crouchAction;
    private InputAction interactAction;
    private InputAction pressAction;
    private InputAction pauseAction;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        var gameplay = playerInput.actions.FindActionMap("Gameplay", true);
        moveAction = gameplay.FindAction("Move", true);
        sprintAction = gameplay.FindAction("Sprint", true);
        lookAction = gameplay.FindAction("Look", true);
        crouchAction = gameplay.FindAction("Crouch", true);
        interactAction = gameplay.FindAction("Interact", true);
        pressAction = gameplay.FindAction("Press", true);
        pauseAction = gameplay.FindAction("Pause", true);
    }

    private void LateUpdate()
    {
        InteractPressed = false;
        PressPressed = false;
        pausePressed = false;
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
        //interactAction.canceled += OnInteract;

        pressAction.performed += OnPress;
        //pressAction.canceled += OnPress;

        pauseAction.performed += OnPause;
        //pauseAction.canceled += OnPause;
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
        //interactAction.canceled -= OnInteract;

        pressAction.performed -= OnPress;
        //pressAction.canceled -= OnPress;

        pauseAction.performed -= OnPause;
        //pauseAction.canceled -= OnPause;
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
        InteractPressed = true;
    }

    private void OnPress(InputAction.CallbackContext ctx)
    {
        PressPressed = true;
    }

    private void OnPause(InputAction.CallbackContext ctx)
    {
        pausePressed = true;
    }

    public void Clear()
    {
        Move = Vector2.zero;
        Look = Vector2.zero;
        Crouch = false;
        Sprint = false;
    }
}

