using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class EndPoint : ColliderDrawUtil
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameUIControl.Instance.OnMainMenu();
        }
    }
}
