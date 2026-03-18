using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement: MonoBehaviour
{
    [Header("Player control variables")]
    [Range(10f, 50f)]
    [SerializeField] private float moveSpeed;

    [Tooltip("Sets camera sensitivity")]
    [Range(1f,10f)]
    [SerializeField] private float sensitivity = 0.01f;

    [Space(10)]
    [Header("Refferences")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Camera cam;
    

    [Header("Player actions")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference lookAction;

    /* Helper variables to store action data and state flags*/
    private Vector2 _moveDirection;
    private Vector2 _lookDirection;

    /* Runtime helper variables */
    private float xRotation = 0f;           // camera Vertical rotation
    private float yRotation = 0f;           // camera Horizontal rotation
    private float _speedCorrection = 10f;   // DeltaTime correction for movement speed

    private void Update()
    {
        float speed = _speedCorrection * moveSpeed * Time.deltaTime;
        Vector3 move = rb.transform.forward * _moveDirection.y * speed + rb.transform.right * _moveDirection.x * speed;
        rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);

        _lookDirection *= sensitivity * Time.deltaTime;
        xRotation -= _lookDirection.y;
        yRotation += _lookDirection.x;
        yRotation = yRotation % 360f;


        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        rb.rotation = Quaternion.Euler(0, yRotation, 0);
        cam.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
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
