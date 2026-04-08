using UnityEngine;

public interface IGrabable
{
    public bool CanGrab();
    public void LockGrab(bool locked);
    public void OnGrab(bool grabbed, GameObject caller);
    string GetGrabInfo();
}