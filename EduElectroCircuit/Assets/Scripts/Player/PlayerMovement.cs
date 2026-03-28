using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement: MonoBehaviour
{
    [SerializeField] private CameraControl cameraControl;
    #region Variables
    [Header("Player control variables")]
    [SerializeField] private float moveSpeed;

    [Tooltip("Sets camera sensitivity")]

    [Space(10)]
    [Header("Refferences")]
    [SerializeField] private Rigidbody rb;
    

    [Header("Player actions")]
    [SerializeField] private InputActionReference moveAction;
    
    public float drag;

    /* Helper variables to store action data and state flags*/
    private Vector2 _moveDirection;

    /* Runtime helper variables */
    private float _speedCorrection = 10f;   // DeltaTime correction for movement speed
    #endregion

    private void Update()
    {
        UpdatePlayeMovement();
    }

    private void UpdatePlayeMovement()
    {
        float speed = _speedCorrection * moveSpeed * Time.deltaTime;
        Transform movementTransform = cameraControl.GetOrientation();
        Vector3 move = movementTransform.transform.forward * _moveDirection.y + movementTransform.transform.right * _moveDirection.x;
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
        cameraControl.InitHandShake(this.gameObject);
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        moveAction.action.performed += OnMoveInput;
        moveAction.action.canceled += OnMoveInput;
        
    }

    private void OnDisable()
    {
        moveAction.action.performed -= OnMoveInput;
        moveAction.action.canceled -= OnMoveInput;
        
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
}
