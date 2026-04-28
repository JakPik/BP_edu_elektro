using System;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public struct GrabbedObject
{
    public GameObject component;
    public Rigidbody object_rb;
    public IGrabable grabable;

    public void Refresh()
    {
        component = null;
        grabable = null;
        object_rb = null;
    }
}

public class PlayerInteractionControl : MonoBehaviour
{
    [SerializeField] private CameraControl cameraControl;
    [Header("Hold/Grab params")]
    [SerializeField] private float followSpeed = 10f;
    [SerializeField] private float smoothness = 15f;
    [SerializeField] private float dropDistance = 1f;
    [SerializeField] private float holdDistance = 2.0f;
    [Space(10)]
    [SerializeField] private bool holding = false;
    [SerializeField] private GrabbedObject grabbedObject = new GrabbedObject();
    [Space(10)]
    [SerializeField] private IInteractable interactable;

    [Space(10)]
    [Header("Player actions")]
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private InputActionReference holdtAction;
    [Header("Event Channels")]
    [SerializeField] private GenericEventChannel<ReloadEvent> reload;


    private void Update()
    {
        RayCast();
        UpdateHold();
    }


    private void UpdateHold()
    {
        if (holding && grabbedObject.component != null)
        {
            Vector3 newPos = cameraControl.GetCameraForward() * holdDistance + cameraControl.GetCameraPosition();
            Vector3 direction = newPos - grabbedObject.component.transform.position;
            Vector3 targetVelocity = direction * followSpeed;

            if (Math.Abs(direction.magnitude) > dropDistance)
            {
                Drop();
                return;
            }

            grabbedObject.object_rb.linearVelocity = Vector3.Lerp(
                grabbedObject.object_rb.linearVelocity,
                targetVelocity,
                smoothness * Time.deltaTime
            );
        }
    }

    private void RayCast()
    {
        var (found, hitInfo) = cameraControl.CameraRayCast(holdDistance);
        if (!found || hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Walls"))
        {
            if (!holding)
            {
                grabbedObject.grabable?.DisplayInfo(false);
                grabbedObject.Refresh();
            }

            interactable?.DisplayInfo(false);
            interactable = null;
            return;
        }

        GameObject obj = hitInfo.collider.gameObject;
        if (obj.TryGetComponent(out IInteractable interact) && interact != interactable)
        {
            interactable?.DisplayInfo(false);
            interactable = interact;
            interactable.DisplayInfo(true);
        }
        if (!holding && obj != grabbedObject.component && obj.TryGetComponent(out IGrabable grabable))
        {
            grabbedObject.grabable?.DisplayInfo(false);
            grabbedObject.component = hitInfo.collider.gameObject;
            grabbedObject.grabable = grabable;
            grabbedObject.object_rb = hitInfo.collider.gameObject.GetComponent<Rigidbody>();
            if (grabbedObject.object_rb == null)
            {
                grabbedObject.Refresh();
            }
            else
            {
                grabable.DisplayInfo(true);
            }
        }

    }

    private void OnEnable()
    {
        interactAction.action.started += OnInteract;
        holdtAction.action.started += OnHold;
        reload.OnEventRaised += OnRoomReload;
    }

    private void OnDisable()
    {
        interactAction.action.started -= OnInteract;
        holdtAction.action.started -= OnHold;
        reload.OnEventRaised -= OnRoomReload;
    }

    private void OnRoomReload(ReloadEvent @event)
    {
        if (holding)
        {
            grabbedObject.grabable.OnGrab(false, this.gameObject);
            grabbedObject.grabable?.DisplayInfo(false);
            grabbedObject.Refresh();
            holding = false;
        }
        interactable?.DisplayInfo(false);
        interactable = null;
    }

    private void OnHold(InputAction.CallbackContext context)
    {
        if ((!holding && grabbedObject.component == null) || (!grabbedObject.grabable?.CanGrab() ?? true)) return;
        if (!holding)
        {
            Grab();
        }
        else
        {
            Drop();
        }
    }

    private void Grab()
    {
        holding = !holding;
        try
        {
            grabbedObject.grabable.OnGrab(true, this.gameObject);
            grabbedObject.grabable?.DisplayInfo(false);
        }
        catch (Exception e)
        {
            holding = !holding;
            Logger.Log(this, this.name, "While calling OnGrab() on " + grabbedObject.component.name + "\n" + e.Message, LogType.EXCEPTION);
        }
    }

    private void Drop()
    {
        holding = !holding;
        grabbedObject.grabable.OnGrab(false, this.gameObject);
        grabbedObject.Refresh();
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (!holding && interactable != null && interactable.CanInteract())
        {
            interactable?.OnInteract();
        }
    }
}
