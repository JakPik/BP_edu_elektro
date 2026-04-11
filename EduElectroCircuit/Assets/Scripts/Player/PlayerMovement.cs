using System;
using System.Collections;
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
    [SerializeField] private InputActionReference respawnAction;
    
    public float drag;

    /* Helper variables to store action data and state flags*/
    private Vector2 _moveDirection;

    /* Runtime helper variables */
    private float _speedCorrection = 10f;   // DeltaTime correction for movement speed

    private Coroutine _respawnCoroutine;
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
        
        rb.linearVelocity += new Vector3(move.x, 0f, move.z);

        if(rb.linearVelocity.magnitude > moveSpeed)
        {
            rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, moveSpeed);
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

    private IEnumerator Respawn(bool fadeIn)
    {
        yield return StartCoroutine(LevelControl.Instance.Respawn(rb, cameraControl, fadeIn));
        _respawnCoroutine = null;
    }

    private void Start()
    {
        cameraControl.InitHandShake(this.gameObject);
        _respawnCoroutine = StartCoroutine(Respawn(false));
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        moveAction.action.performed += OnMoveInput;
        moveAction.action.canceled += OnMoveInput;
        respawnAction.action.started += OnRespawn;
        
    }

    
    private void OnDisable()
    {
        moveAction.action.performed -= OnMoveInput;
        moveAction.action.canceled -= OnMoveInput;
        respawnAction.action.started -= OnRespawn;
    }

    private void OnRespawn(InputAction.CallbackContext context)
    {
        if(_respawnCoroutine == null)
        {
            _respawnCoroutine = StartCoroutine(Respawn(true));
        }
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
