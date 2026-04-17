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

    [Tooltip("Sets camera sensitivity")]

    [Space(10)]
    [Header("Refferences")]
    [SerializeField] private Rigidbody rb;


    [Header("Player actions")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference respawnAction;

    [Header("Raised Events")]
    [SerializeField] private GenericEventChannel<ReloadRequestEvent> reloadRequest;

    [Header("Event Channels")]
    [SerializeField] private GenericEventChannel<ReloadEvent> reload;

    private Vector2 _moveDirection;
    private float _speedCorrection = 10f;

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
        float y_velocity = rb.linearVelocity.y;
        rb.linearVelocity += new Vector3(move.x, 0f, move.z);

        if (rb.linearVelocity.magnitude > moveSpeed)
        {
            rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, moveSpeed);
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

    private void Start()
    {
        cameraControl.InitHandShake(this.gameObject);
        reloadRequest?.RaiseEvent(new ReloadRequestEvent(true), this.name);
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        moveAction.action.performed += OnMoveInput;
        moveAction.action.canceled += OnMoveInput;
        respawnAction.action.started += OnRespawn;
        reload.OnEventRaised += OnRoomReload;
    }


    private void OnDisable()
    {
        moveAction.action.performed -= OnMoveInput;
        moveAction.action.canceled -= OnMoveInput;
        respawnAction.action.started -= OnRespawn;
        reload.OnEventRaised -= OnRoomReload;
    }

    private void OnRoomReload(ReloadEvent @event)
    {
        rb.position = @event.SpawnTransform.position;
        rb.rotation = @event.SpawnTransform.rotation;
        rb.linearVelocity = Vector3.zero;
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
