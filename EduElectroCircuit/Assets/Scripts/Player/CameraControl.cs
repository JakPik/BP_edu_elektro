/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: CameraControl
 */

using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControl : MonoBehaviour
{
    [Header("Control Variables")]
    [SerializeField] private Transform orientation;
    [SerializeField] private InputActionReference lookAction;
    [SerializeField] private float sensitivity = 0.01f;
    [SerializeField] private bool _smoothing = false;
    [SerializeField] private float _smoothness = 0.25f;

    [Header("Event Channels")]
    [SerializeField] private GenericEventChannel<ReloadEvent> reload;

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
        _xRotation -= _lookDirection.y * (_smoothing? _smoothness : 1f);
        _yRotation += _lookDirection.x * (_smoothing? _smoothness : 1f);
        
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);


        if(_smoothing)
        {
            orientation.rotation = Quaternion.Lerp(
                orientation.rotation, 
                Quaternion.Euler(0, _yRotation, 0),
                _smoothness * Time.fixedDeltaTime);

            transform.rotation = Quaternion.Lerp(
                Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0), 
                Quaternion.Euler(_xRotation, _yRotation, 0),
                _smoothness * Time.fixedDeltaTime);
            //transform.rotation = Quaternion.Euler(_xRotation, transform.rotation.y, transform.rotation.z);
        }
        else
        {
            _yRotation = _yRotation % 360f;
            orientation.rotation = Quaternion.Euler(0, _yRotation, 0);
            transform.rotation = Quaternion.Euler(_xRotation, _yRotation, 0);  
        }
    }

    public (bool, RaycastHit) CameraRayCast(float distance)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        LayerMask layerMask = ~LayerMask.NameToLayer("Default") & ~LayerMask.NameToLayer("Walls");
        return (Physics.Raycast(ray, out RaycastHit hitInfo, distance, layerMask, QueryTriggerInteraction.Ignore), hitInfo);
    }

    public Vector3 GetCameraPosition() => transform.position;
    public Vector3 GetCameraForward() => transform.forward;
    public Transform GetOrientation() => orientation;

    public void ResetView(Quaternion rotation)
    {
        transform.rotation = rotation;
        _yRotation = transform.eulerAngles.y;
        _xRotation = 0f;
    }

    public void InitHandShake(GameObject caller, bool cinematicMove)
    {
        _smoothing = cinematicMove;
        _yRotation = caller.transform.eulerAngles.y;
    }

    private void OnEnable()
    {
        lookAction.action.performed += OnLookInput;
        lookAction.action.canceled += OnLookInput;
        reload.OnEventRaised += OnRoomReload;
    }

    private void OnDisable()
    {
        lookAction.action.performed -= OnLookInput;
        lookAction.action.canceled -= OnLookInput;
        reload.OnEventRaised -= OnRoomReload;
    }

    private void OnRoomReload(ReloadEvent @event)
    {
        ResetView(@event.SpawnTransform.rotation);
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

    public Camera GetCamera() => this.GetComponent<Camera>();
}
