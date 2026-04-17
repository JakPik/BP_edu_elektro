using System;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class SpawnPointControl : ColliderDrawUtil
{
    [SerializeField] private GenericEventChannel<ReloadEvent> reload;
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LevelControl.Instance.SetNextSpawnPoint(this.gameObject);
            this.GetComponent<BoxCollider>().enabled = false;
        }
    }

    void OnEnable()
    {
        reload.OnEventRaised += OnReload;
    }

    void OnDisable()
    {
        reload.OnEventRaised -= OnReload;
    }

    private void OnReload(ReloadEvent @event)
    {
        if (@event.SpawnReference == null) this.GetComponent<BoxCollider>().enabled = true;
    }
}
