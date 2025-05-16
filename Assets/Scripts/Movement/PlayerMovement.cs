using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    PlayerInput playerInput;
    public CharacterController controller;

    public Animator animator;
    int isWalkingHash;
    int isRunningHash;

    Vector2 movementInput;
    Vector3 currentMovement;

    bool isMovementPressed;
    bool isRunningPressed;
    float currentSpeed;

    public float walkingSpeed = 7.0f;
    public float runningSpeed = 15.0f;
    public float rotationFactor = 15.0f;
    public float rotationSmoothness = 0.2f;
    float turnSmoothVelocity;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;

        playerInput = new PlayerInput();

        //animator hashes
        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");

        //movement logic
        playerInput.Movement.Move.started += OnMovementInput;
        playerInput.Movement.Move.canceled += OnMovementInput;
        playerInput.Movement.Move.performed += OnMovementInput;

        //sprint logic
        playerInput.Movement.Run.started += OnRunInput;
        playerInput.Movement.Run.canceled += OnRunInput;

        //get camera
        CinemachineFreeLook cinemachineCamera = GameObject.FindWithTag("CinemachineFreeLook").GetComponent<CinemachineFreeLook>();
        var lookAt = GameObject.FindWithTag("LookAtPlayer");
        cinemachineCamera.LookAt = lookAt.transform;
        cinemachineCamera.Follow = lookAt.transform;
    }

    void OnMovementInput(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
        currentMovement.x = movementInput.x; 
        currentMovement.z = movementInput.y;
        isMovementPressed = movementInput.x != 0 || movementInput.y != 0;
    }

    void OnRunInput(InputAction.CallbackContext context)
    {
        isRunningPressed = context.ReadValueAsButton();
    }


    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }

    void MovePlayer()
    {
        //Move player

        currentSpeed = isMovementPressed ? walkingSpeed : 0;
        currentSpeed = isMovementPressed && isRunningPressed ? runningSpeed : currentSpeed;

        float horizontal = currentMovement.x;
        float vertical = currentMovement.z;
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;
        //Rotate Player

        if (currentSpeed != 0)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, rotationSmoothness);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * currentSpeed * Time.deltaTime);
        }

    }

    void Animate()
    {
        animator.SetBool(isWalkingHash, isMovementPressed);
        if(isMovementPressed)
        {
            animator.SetBool(isRunningHash, isRunningPressed);
        }
        else
        {
            animator.SetBool(isRunningHash, false);
        }
    }


    // Update is called once per frame
    void Update()
    {
        Animate();
        MovePlayer();
    }
}
