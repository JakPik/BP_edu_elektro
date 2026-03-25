using System;
using System.Threading;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement: MonoBehaviour
{
    [Header("Player control variables")]
    [SerializeField] private float moveSpeed;

    [Tooltip("Sets camera sensitivity")]

    [SerializeField] private float sensitivity = 0.01f;

    [Space(10)]
    [Header("Refferences")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Camera cam;
    

    [Header("Player actions")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference lookAction;
    public float drag;

    /* Helper variables to store action data and state flags*/
    private Vector2 _moveDirection;
    private Vector2 _lookDirection;

    /* Runtime helper variables */
    [SerializeField] private float xRotation = 0f;           // camera Vertical rotation
    [SerializeField] private float yRotation = 0f;           // camera Horizontal rotation
    private float _speedCorrection = 10f;   // DeltaTime correction for movement speed
    [SerializeField] Transform orientation;

    private Vector3 XZVectorNormalize(Vector3 input)
    {
        return new Vector3(input.x, 0, input.z).normalized;
    }

    private void Update()
    {
        _lookDirection *= sensitivity;
        xRotation -= _lookDirection.y;
        yRotation += _lookDirection.x;
        //yRotation = yRotation % 360f;


        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        cam.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);


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
    }

    private void OnDisable()
    {
        moveAction.action.performed -= OnMoveInput;
        moveAction.action.canceled -= OnMoveInput;
        lookAction.action.performed -= OnLookInput;
        lookAction.action.canceled -= OnLookInput;
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
