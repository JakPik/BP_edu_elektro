using System;
using System.Threading;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement: MonoBehaviour
{
    #region Variables
    [Header("Player control variables")]
    [SerializeField] private float moveSpeed;

    [Tooltip("Sets camera sensitivity")]

    [SerializeField] private float sensitivity = 0.01f;
    [SerializeField] private float holdDistance = 2.0f;

    [Space(10)]
    [Header("Refferences")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Camera cam;
    [SerializeField] Transform orientation;     // Refference to Player orientation
    

    [Header("Player actions")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference lookAction;
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private InputActionReference holdtAction;
    public float drag;

    /* Helper variables to store action data and state flags*/
    private Vector2 _moveDirection;
    private Vector2 _lookDirection;
    [SerializeField] private bool holding = false;
    [SerializeField] private GameObject component = null;

    /* Runtime helper variables */
    private float xRotation = 0f;           // camera Vertical rotation
    private float yRotation = 0f;           // camera Horizontal rotation
    private float _speedCorrection = 10f;   // DeltaTime correction for movement speed
    [SerializeField] private IInteractable interactable;
    #endregion

    private Vector3 XZVectorNormalize(Vector3 input)
    {
        return new Vector3(input.x, 0, input.z).normalized;
    }

    private void Update()
    {
        UpdateCameraLook();
        UpdatePlayeMovement();
       // RayCast();
       // UpdateHold();
    }

    private void UpdateHold()
    {
        if(holding)
        {
            component.transform.position = cam.transform.forward * holdDistance + this.transform.position;
        }
    }

    private void RayCast()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            GameObject obj = hitInfo.collider.gameObject;
            //if(obj != component) holding = false;
            if (obj.TryGetComponent(out IInteractable interact))
            {
                interactable = interact;
                Logger.Log(this.name, interactable.GetInteractionInfo(), LogType.INFO);
            }
            if(obj != component && obj.TryGetComponent(out CircuitComponent _))
            {
                component = hitInfo.collider.gameObject;
            }
        }
        else
        {
            //Debug.Log("Nothing found");
        }
    }

    private void UpdateCameraLook()
    {
        _lookDirection *= sensitivity;
        xRotation -= _lookDirection.y;
        yRotation += _lookDirection.x;
        yRotation = yRotation % 360f;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        cam.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
    }

    private void UpdatePlayeMovement()
    {
        float speed = _speedCorrection * moveSpeed * Time.deltaTime;

        Vector3 move = orientation.transform.forward * _moveDirection.y + orientation.transform.right * _moveDirection.x;
        move = move.normalized * speed;
        
        Vector3 velocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if(velocity.magnitude < moveSpeed)
        {
            rb.linearVelocity += new Vector3(move.x, 0f, move.z);
        }
        if(move.magnitude == 0)
        {
            MoveDrag();
        }
    }

    private void MoveDrag()
    {
        float velocityX = rb.linearVelocity.x * Time.deltaTime * drag;
        float velocityZ = rb.linearVelocity.z * Time.deltaTime * drag;

        Vector3 vector = rb.linearVelocity;
        vector.x -= velocityX;
        vector.z -= velocityZ;

        rb.linearVelocity = vector;
    }

    private void Start()
    {
        yRotation = this.transform.eulerAngles.y;
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        moveAction.action.performed += OnMoveInput;
        moveAction.action.canceled += OnMoveInput;
        lookAction.action.performed += OnLookInput;
        lookAction.action.canceled += OnLookInput;
        interactAction.action.started += OnInteract;
        holdtAction.action.started += OnHold;
    }

    private void OnDisable()
    {
        moveAction.action.performed -= OnMoveInput;
        moveAction.action.canceled -= OnMoveInput;
        lookAction.action.performed -= OnLookInput;
        lookAction.action.canceled -= OnLookInput;
        interactAction.action.started -= OnInteract;
        holdtAction.action.started -= OnHold;
    }

    private void OnHold(InputAction.CallbackContext context)
    {
        if(holding)
        {
           // component = null;
        }
        holding = !holding;
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if(holding)
        {
           // component = null;
        }
        holding = !holding;
        /*if(!holding && interactable != null)
        {
            interactable?.OnInteract();
        }*/
    }

    private void OnMoveInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            _moveDirection = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _moveDirection = Vector2.zero;
        }
        
    }

    private void OnLookInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            _lookDirection = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _lookDirection = Vector2.zero;
        }

    }
}
