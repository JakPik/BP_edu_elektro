using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private Transform orientation;
    [SerializeField] private InputActionReference lookAction;
    [SerializeField] private float sensitivity = 0.01f;

    private Vector2 _lookDirection;
    private float _xRotation = 0f;
    private float _yRotation = 0f;
    private void Update()
    {
        UpdateCameraLook();
    }

    private void UpdateCameraLook()
    {
        _lookDirection *= sensitivity;
        _xRotation -= _lookDirection.y;
        _yRotation += _lookDirection.x;
        _yRotation = _yRotation % 360f;

        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);
        orientation.rotation = Quaternion.Euler(0, _yRotation, 0);
        transform.rotation = Quaternion.Euler(_xRotation, _yRotation, 0);
    }

    public (bool, RaycastHit) CameraRayCast(float distance)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        LayerMask layerMask = ~LayerMask.NameToLayer("Default") & ~LayerMask.NameToLayer("Walls");
        return (Physics.Raycast(ray, out RaycastHit hitInfo, distance*2, layerMask, QueryTriggerInteraction.Ignore), hitInfo);
    }

    public Vector3 GetCameraPosition() => transform.position;
    public Vector3 GetCameraForward() => transform.forward;
    public Transform GetOrientation() => orientation;

    public void InitHandShake(GameObject caller)
    {
        _yRotation = caller.transform.eulerAngles.y;
    }

    private void OnEnable()
    {
        lookAction.action.performed += OnLookInput;
        lookAction.action.canceled += OnLookInput;
    }

    private void OnDisable()
    {
        lookAction.action.performed -= OnLookInput;
        lookAction.action.canceled -= OnLookInput;
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
