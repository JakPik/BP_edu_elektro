using UnityEngine;
using System.Collections;

public class Door_segmented : DoorControl
{
    [SerializeField] private GameObject _doorL;
    [SerializeField] private GameObject _doorR;

    protected float CLOSED_OFFSET = 0;
    protected float OPEN_OFFSET = 1.2f;

    

    public override IEnumerator OpenDoor()
    {
        while (_doorR.transform.localPosition.x < OPEN_OFFSET)
        {
            float anim = _animSpeed * Time.deltaTime;
            _doorL.transform.localPosition -= new Vector3(anim, 0.0f, 0.0f);
            _doorR.transform.localPosition += new Vector3(anim, 0.0f, 0.0f);

            yield return null;
        }
        _doorL.transform.localPosition = new Vector3(-OPEN_OFFSET, _doorL.transform.localPosition.y, _doorL.transform.localPosition.z);
        _doorR.transform.localPosition = new Vector3(OPEN_OFFSET, _doorR.transform.localPosition.y, _doorR.transform.localPosition.z);
        _curCoroutine = null;
    }

    public override IEnumerator CloseDoor()
    {
        while (_doorR.transform.localPosition.x > CLOSED_OFFSET)
        {
            float anim = _animSpeed * Time.deltaTime;
            _doorL.transform.localPosition += new Vector3(anim, 0.0f, 0.0f);
            _doorR.transform.localPosition -= new Vector3(anim, 0.0f, 0.0f);
            yield return null;
        }
        _doorL.transform.localPosition = new Vector3(CLOSED_OFFSET, _doorL.transform.localPosition.y, _doorL.transform.localPosition.z);
        _doorR.transform.localPosition = new Vector3(CLOSED_OFFSET, _doorR.transform.localPosition.y, _doorR.transform.localPosition.z);
        _curCoroutine = null;
    }
}
