using System;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public struct GrabbedObject
{
    public GameObject component;
    public IGrabable grabable;

    public void Refresh()
    {
        component = null;
        grabable = null;
    }
}

public class PlayerInteractionControl : MonoBehaviour
{
    [SerializeField] private CameraControl cameraControl;
    [SerializeField] private float holdDistance = 2.0f;
    [SerializeField] private bool holding = false;
    [SerializeField] private GrabbedObject grabbedObject = new GrabbedObject();
    [SerializeField] private IInteractable interactable;
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private InputActionReference holdtAction;
    [SerializeField] private GenericVoidEventChannel roomReloadEventChannel;

    private void Update()
    {
        RayCast();
        UpdateHold();
    }

    private void UpdateHold()
    {
        if(holding && grabbedObject.component != null)
        {
            Ray ray = new Ray(cameraControl.GetCameraPosition(), cameraControl.GetCameraForward());
            LayerMask mask = ~LayerMask.GetMask("Grabable");
            float distance = Physics.Raycast(ray, out RaycastHit hitInfo, holdDistance ,mask, QueryTriggerInteraction.Ignore)? hitInfo.distance : holdDistance;
            grabbedObject.component.transform.position = cameraControl.GetCameraForward() * distance + cameraControl.GetCameraPosition();
        }
    }

    private void RayCast()
    {
        var (found, hitInfo) = cameraControl.CameraRayCast(holdDistance);
        if (!found || hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Walls"))
        {
            if(!holding)
            {
                grabbedObject.grabable?.DisplayInfo(false);
                grabbedObject.Refresh();
            }

            interactable?.DisplayInfo(false);
            interactable = null;
        }
        else
        {
            GameObject obj = hitInfo.collider.gameObject;
            if (obj.TryGetComponent(out IInteractable interact) && interact != interactable)
            {
                interactable?.DisplayInfo(false);
                interactable = interact;
                interactable.DisplayInfo(true);
            }
            if(obj != grabbedObject.component && obj.TryGetComponent(out IGrabable grabable))
            {
                grabbedObject.grabable?.DisplayInfo(false);
                grabbedObject.component = hitInfo.collider.gameObject;
                grabbedObject.grabable = grabable;
                grabable.DisplayInfo(true);
            }
        }
    }

    private void OnEnable()
    {
        interactAction.action.started += OnInteract;
        holdtAction.action.started += OnHold;
        roomReloadEventChannel.OnEventRaised += OnRoomReload;
    }

    private void OnDisable()
    {
        interactAction.action.started -= OnInteract;
        holdtAction.action.started -= OnHold;
        roomReloadEventChannel.OnEventRaised -= OnRoomReload;
    }

    private void OnRoomReload()
    {
        if(holding)
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
        if(!grabbedObject.grabable?.CanGrab() ?? true) return;
        try {
            grabbedObject.grabable.OnGrab(!holding, this.gameObject);
            grabbedObject.grabable?.DisplayInfo(false);
        }
        catch(Exception e)
        {
            Logger.Log(this.name, "While calling OnGrab() on " + grabbedObject.component.name + "\n" + e.Message, LogType.EXCEPTION);
        }
        if(holding)
        {
           grabbedObject.Refresh();
        }
        if(!holding && grabbedObject.component == null) return;
        holding = !holding;
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if(!holding && interactable != null && interactable.CanInteract())
        {
            interactable?.OnInteract();
        }
    }
}
