using System;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class SpawnPointControl : ColliderDrawUtil
{
    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            LevelControl.Instance.SetNextSpawnPoint(this.gameObject);
            this.GetComponent<BoxCollider>().enabled = false;
        }
    }
}
