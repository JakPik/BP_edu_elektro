using System.Dynamic;
using UnityEngine;

[System.Serializable]
public abstract class ComponentDTO
{
    public string name;
    public Vector3 position;

    public GameObject ToGameObject(Transform componentsParent)
    {
        GameObject obj = componentsParent.Find(name)?.gameObject;
        if (obj == null)
        {
            obj = CreateInstance(componentsParent);
        }
        else
        {
            ResetInstancePosition(obj, componentsParent);
            ResetComponent(obj);
        }
        return obj;
    }

    protected abstract GameObject CreateInstance(Transform componentsParent);

    protected GameObject CreateComponent<T>(Transform componentsParent) where T:CircuitComponent
    {
        Logger.Log(this, this.name, "Creating new " + typeof(T).Name +": " + name, LogType.INFO);

        GameObject resistorPrefab = Resources.Load<GameObject>("Prefabs/Components/" + typeof(T).Name);
        GameObject obj = Object.Instantiate(resistorPrefab, componentsParent.position + position, componentsParent.rotation, componentsParent);
        obj.name = name;
        return obj;
    }

    protected abstract void ResetComponent(GameObject obj);

    protected void ResetInstancePosition(GameObject obj, Transform componentsParent)
    {
        Rigidbody rigidbody = obj.GetComponent<Rigidbody>();
        if (rigidbody == null)
        {
            Logger.Log(this, this.name, "Rigidbody component not found on " + obj.name, LogType.ERROR);
            return;
        }
        rigidbody.linearVelocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        rigidbody.constraints = RigidbodyConstraints.None;
        obj.transform.position = position + componentsParent.position;
        obj.transform.rotation = componentsParent.rotation;
    }
}