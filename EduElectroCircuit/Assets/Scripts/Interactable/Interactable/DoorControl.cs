using UnityEngine;
using System.Collections;

public class DoorControl : ColliderDrawUtil
{
    [SerializeField] GameObject door;
    [SerializeField] GameObject enterPoint;
    [SerializeField] float openHeight;
    [SerializeField] float animSpeed;
    [SerializeField] bool twoWay = false;
    [SerializeField] bool locked = false;
    [SerializeField] private GenericEventChannel<NodeValidationEvent> nodeValidation;
    private float closedHeight;
    private Coroutine curCoroutine = null;
    private Vector3 forward;

    private void Start()
    {
        forward = door.transform.position - enterPoint.transform.position;
        closedHeight = door.transform.position.y;
        openHeight = closedHeight + 4.0f;
    }

    private void OnEnable()
    {
        if(nodeValidation == null) return;
        nodeValidation.OnEventRaised += LockState;
    }

    private void OnDisable()
    {
        if(nodeValidation == null) return;
        nodeValidation.OnEventRaised -= LockState;
    }

    public void LockState(NodeValidationEvent @event)
    {
        locked = !@event.Valid;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(locked)
        {
            return;
        }
        Vector3 position = door.transform.position - other.transform.position;
        if(twoWay || Vector3.Dot(position, forward) >= 0)
        {
            if (curCoroutine != null)
            {
                StopCoroutine(curCoroutine);
            }
            curCoroutine = StartCoroutine(OpenDoor());
        }
    }

    public IEnumerator OpenDoor()
    {
        while(door.transform.position.y < openHeight)
        {
            door.transform.position += new Vector3(0.0f, animSpeed, 0.0f);
            yield return null;
        }
        curCoroutine = null;
    }

    public IEnumerator CloseDoor()
    {
        while (door.transform.position.y > closedHeight)
        {
            door.transform.position += new Vector3(0.0f, -animSpeed, 0.0f);
            yield return null;
        }
        curCoroutine = null;
    }

    private void OnTriggerExit(Collider other)
    {
        if (curCoroutine != null)
        {
            StopCoroutine(curCoroutine);
        }
        curCoroutine = StartCoroutine(CloseDoor());
    }

}
