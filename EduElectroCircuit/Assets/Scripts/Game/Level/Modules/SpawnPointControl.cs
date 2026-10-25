/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: SpawnPointControl
 */

using System.Text.RegularExpressions;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class SpawnPointControl : ColliderDrawUtil
{
    [SerializeField] private int roomID;
    [SerializeField] private GenericEventChannel<ReloadEvent> reload;
    [SerializeField] private bool multiTrigger = false;

    void Awake()
    {
        try {
            roomID = int.Parse(Regex.Match(this.transform.parent.name, @"-?\d+").Value);
        }
        catch
        {
            roomID = 0;
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LevelControl.Instance.SetNextSpawnPoint(this.gameObject,roomID);
            if(!multiTrigger) {
                this.GetComponent<BoxCollider>().enabled = false;
            }
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
