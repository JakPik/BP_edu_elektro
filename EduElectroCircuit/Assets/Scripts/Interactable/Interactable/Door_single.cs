/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: DoorControl
 */

using UnityEngine;
using System.Collections;

public class Door_single : DoorControl
{
    [SerializeField] private GameObject _door;
    private float OPEN_HEIGHT = 3.0f;
    private float CLOSED_HEIGHT = 0.0f;

    private void Start()
    {
         _forward =_door.transform.position - _enterPoint.transform.position;
    }
    public override IEnumerator OpenDoor()
    {
        while (_door.transform.position.y < OPEN_HEIGHT)
        {
            _door.transform.localPosition += new Vector3(0.0f, _animSpeed * Time.deltaTime, 0.0f);
            yield return null;
        }
        _door.transform.localPosition = new Vector3(0.0f, OPEN_HEIGHT, 0.0f);
        _curCoroutine = null;
    }

    public override IEnumerator CloseDoor()
    {
        while (_door.transform.position.y > CLOSED_HEIGHT)
        {
            _door.transform.localPosition += new Vector3(0.0f, -_animSpeed * Time.deltaTime, 0.0f);
            yield return null;
        }
        _door.transform.localPosition = new Vector3(0.0f, CLOSED_HEIGHT, 0.0f);
        _curCoroutine = null;
    }
}
