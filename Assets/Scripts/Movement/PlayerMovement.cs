using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private static PlayerMovement singleton;
    GameManager gameManager;
    PlayerInput playerInput;
    public CharacterController controller;

    public Animator animator;
    int isWalkingHash;
    int isRunningHash;

    private Vector2 movementInput;
    private Vector3 currentMovement;
    private Vector3 previousPosition;

    bool isMovementPressed;
    bool isRunningPressed;
    float currentSpeed;

    public float walkingSpeed;
    public float runningSpeed;
    public float rotationFactor;
    public float rotationSmoothness;
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
    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if(gameManager.playerPosition != Vector3.zero && gameManager.playerRotation != Quaternion.identity)
        {
            transform.position = gameManager.playerPosition;
            transform.rotation = gameManager.playerRotation;
        }
        gameManager.player = gameObject;
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
        CinemachineFreeLook cinemachineCamera = GameObject.FindWithTag("CinemachineFreeLook").GetComponent<CinemachineFreeLook>();
        var lookAt = GameObject.FindWithTag("LookAtPlayer");
        cinemachineCamera.LookAt = lookAt.transform;
        cinemachineCamera.Follow = lookAt.transform;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
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
        previousPosition = transform.position;
        MovePlayer();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "EncounterZone")
        {
            EncounterZone currentZone = other.gameObject.GetComponent<EncounterZone>();
            currentZone.encounterDistance = 0f;
        }
        
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "EncounterZone")
        {
            EncounterZone currentZone = other.gameObject.GetComponent<EncounterZone>();
            currentZone.encounterDistance = 0f;
        }
        
    }
    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "EncounterZone")
        {
            EncounterZone currentZone = other.gameObject.GetComponent<EncounterZone>();
            currentZone.encounterDistance += Vector3.Distance(transform.position, previousPosition);
        }
        
    }
}
