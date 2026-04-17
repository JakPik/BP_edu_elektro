using UnityEngine;

public abstract class UIPanel : MonoBehaviour
{
    public abstract void SetData<T>(T data);
}