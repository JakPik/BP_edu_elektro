using System;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class SpawnPointControl : ColliderDrawUtil
{
    [SerializeField] private GenericVoidEventChannel levelReload;
    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            LevelControl.Instance.SetNextSpawnPoint(this.gameObject);
            this.GetComponent<BoxCollider>().enabled = false;
        }
    }

    void OnEnable()
    {
        if(levelReload != null) levelReload.OnEventRaised += OnReload;
    }

     void OnDisable() 
    {
        if(levelReload != null) levelReload.OnEventRaised -= OnReload;
    }

    private void OnReload()
    {
        this.GetComponent<BoxCollider>().enabled = true;
    }
}
