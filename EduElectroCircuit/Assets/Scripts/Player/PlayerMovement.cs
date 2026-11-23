/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: PlayerMovement
 */

using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CameraControl cameraControl;
    #region Variables
    [Header("Player control variables")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float drag = 4f;
    [SerializeField] private bool _cinematicMove = false;

    [Tooltip("Sets camera sensitivity")]

    [Space(10)]
    [Header("Refferences")]
    [SerializeField] private Rigidbody rb;

    [SerializeField] private InputActionAsset actionMap;


    [Header("Player actions")]
    [SerializeField] private InputActionReference _moveAction;
    [SerializeField] private InputActionReference _respawnAction;
    [SerializeField] private InputActionReference _sprintAction;
    [SerializeField] private InputActionReference _jumpAction;
    [SerializeField] private InputActionReference _cinematicOnAction;

    [Header("Player Cinematic Control")]
    [SerializeField] private InputActionReference c_moveAction;
    [SerializeField] private InputActionReference c_cinematicOffAction;
    [SerializeField] private InputActionReference c_moveUpAction;
    [SerializeField] private InputActionReference c_moveDownAction;
    

    [Header("Raised Events")]
    [SerializeField] private GenericEventChannel<ReloadRequestEvent> reloadRequest;

    [Header("Event Channels")]
    [SerializeField] private GenericEventChannel<ReloadEvent> reload;

    private Vector2 _moveDirection;
    private float _speedCorrection = 10f;

    #region Cinematic Movement Variables
    private float _groundHeight = 1.32f;
    private float _verticalChange = 0f;
    private float _curMoveSpeed;

    private InputActionMap _gameplayMap;
    private InputActionMap _cinematicMap;

    #endregion
    #endregion

    private void Awake()
    {
        cameraControl.InitHandShake(this.gameObject);
        _gameplayMap = actionMap.FindActionMap("Gameplay");
        _cinematicMap = actionMap.FindActionMap("GameplayCinematic");
        //reloadRequest?.RaiseEvent(new ReloadRequestEvent(true), this.name);
    }

    private void Update()
    {
        UpdatePlayeMovement();
    }

    private void UpdatePlayeMovement()
    {
        float speed = _speedCorrection * _curMoveSpeed * Time.deltaTime;
        Transform movementTransform = cameraControl.GetOrientation();
        Vector3 move = movementTransform.transform.forward * _moveDirection.y + movementTransform.transform.right * _moveDirection.x + movementTransform.transform.up * _verticalChange;
        move = move.normalized * speed;
        float y_velocity = rb.linearVelocity.y;
        rb.linearVelocity += new Vector3(move.x, move.y, move.z);

        if (rb.linearVelocity.magnitude > _curMoveSpeed)
        {
            rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, _curMoveSpeed);
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, y_velocity, rb.linearVelocity.z);
        }

        if (move.magnitude == 0)
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

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _moveAction.action.performed += OnMoveInput;
        _moveAction.action.canceled += OnMoveInput;
        _curMoveSpeed = moveSpeed;
       // c_moveAction.action.performed += OnMoveInput;
       // c_moveAction.action.canceled += OnMoveInput;


        _respawnAction.action.started += OnRespawn;
        reload.OnEventRaised += OnRoomReload;

      //  CinematicSwitch(new InputAction.CallbackContext());

     //   _cinematicOnAction.action.started += CinematicSwitch;
     //   c_cinematicOffAction.action.started += CinematicSwitch;

     //   c_moveUpAction.action.performed += MoveUp;
     //   c_moveDownAction.action.performed += MoveDown;
     //   c_moveUpAction.action.canceled += MoveUp;
     //   c_moveDownAction.action.canceled += MoveDown;

        
    }


    private void OnDisable()
    {
        _moveAction.action.performed -= OnMoveInput;
        _moveAction.action.canceled -= OnMoveInput;
        _respawnAction.action.started -= OnRespawn;
        reload.OnEventRaised -= OnRoomReload;
    }

    private void MoveUp(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            _verticalChange = 1f;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _verticalChange = 0f;
        }
    }

    private void MoveDown(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            _verticalChange = -1f;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _verticalChange = 0f;
        }
    }

    private void CinematicSwitch(InputAction.CallbackContext context)
    {
        _cinematicMove = !_cinematicMove;
        cameraControl.ChangeCinematic(_cinematicMove);
        if(_cinematicMove)
        {
            _cinematicMap.Enable();
            _gameplayMap.Disable();
            _curMoveSpeed = 3f;
            rb.useGravity = false;
        }
        else
        {
            _cinematicMap.Disable();
            _gameplayMap.Enable();
            _verticalChange = 0f;
            _curMoveSpeed = moveSpeed;
            rb.useGravity = true;
        }
    }

    private void OnRoomReload(ReloadEvent @event)
    {
        if(@event.LoadingState == LevelLoadingStates.LOADING_DATA_TRANSFER)
        {
            rb.position = @event.SpawnTransform.position;
            rb.rotation = @event.SpawnTransform.rotation;
            rb.linearVelocity = Vector3.zero;
        }
    }

    private void OnRespawn(InputAction.CallbackContext context)
    {
        reloadRequest?.RaiseEvent(new ReloadRequestEvent(false), this.name);
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
