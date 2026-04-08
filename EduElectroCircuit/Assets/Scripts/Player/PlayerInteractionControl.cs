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
        if (!found || hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            if(!holding)
            {
                grabbedObject.Refresh();
            }
            interactable = null;
        }
        else
        {
            GameObject obj = hitInfo.collider.gameObject;
            if (obj.TryGetComponent(out IInteractable interact))
            {
                interactable = interact;
                Logger.Log(this.name, interactable.GetInteractionInfo(), LogType.INFO);
            }
            else if(!holding)
            {
                interactable = null;
            }
            if(obj != grabbedObject.component && obj.TryGetComponent(out IGrabable grabable))
            {
                grabbedObject.component = hitInfo.collider.gameObject;
                grabbedObject.grabable = grabable;
            }
            else if(!holding)
            {
                grabbedObject.Refresh();
            }
        }
    }

    private void OnEnable()
    {
        interactAction.action.started += OnInteract;
        holdtAction.action.started += OnHold;
    }

    private void OnDisable()
    {
        interactAction.action.started -= OnInteract;
        holdtAction.action.started -= OnHold;
    }

    private void OnHold(InputAction.CallbackContext context)
    {
        if(!grabbedObject.grabable?.CanGrab() ?? true) return;
        try {
            grabbedObject.grabable.OnGrab(!holding, this.gameObject);
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
