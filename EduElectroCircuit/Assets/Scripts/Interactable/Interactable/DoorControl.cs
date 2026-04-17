using UnityEngine;
using System.Collections;

public class DoorControl : ColliderDrawUtil
{
    [SerializeField] private GameObject door;
    [SerializeField] private GameObject enterPoint;
    [SerializeField] private float openHeight;
    [SerializeField] private float animSpeed;
    [SerializeField] private bool twoWay = false;
    [SerializeField] private bool locked = false;
    [SerializeField] private GenericEventChannel<NodeValidationEvent> nodeValidation;
    private float _closedHeight;
    private Coroutine _curCoroutine = null;
    private Vector3 _forward;

    private void Start()
    {
        _forward = door.transform.position - enterPoint.transform.position;
        _closedHeight = door.transform.position.y;
        openHeight = _closedHeight + 4.0f;
    }

    private void OnEnable()
    {
        if (nodeValidation == null)
        {
            Logger.Log(this, "DOOR", "nodeValidation is null", LogType.WARNING);
            return;
        }
        nodeValidation.OnEventRaised += LockState;
    }

    private void OnDisable()
    {
        if (nodeValidation == null) return;
        nodeValidation.OnEventRaised -= LockState;
    }

    public void LockState(NodeValidationEvent @event)
    {
        locked = !@event.Valid;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (locked) return;

        Vector3 position = door.transform.position - other.transform.position;
        if (!twoWay && Vector3.Dot(position, _forward) < 0) return;

        if (_curCoroutine != null) StopCoroutine(_curCoroutine);

        _curCoroutine = StartCoroutine(OpenDoor());
    }

    public IEnumerator OpenDoor()
    {
        while (door.transform.position.y < openHeight)
        {
            door.transform.position += new Vector3(0.0f, animSpeed, 0.0f);
            yield return null;
        }
        _curCoroutine = null;
    }

    public IEnumerator CloseDoor()
    {
        while (door.transform.position.y > _closedHeight)
        {
            door.transform.position += new Vector3(0.0f, -animSpeed, 0.0f);
            yield return null;
        }
        _curCoroutine = null;
    }

    private void OnTriggerExit(Collider other)
    {
        if (_curCoroutine != null) StopCoroutine(_curCoroutine);

        _curCoroutine = StartCoroutine(CloseDoor());
    }

}
