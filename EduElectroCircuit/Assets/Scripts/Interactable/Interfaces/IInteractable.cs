/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: IInteractable
 */

using UnityEngine;

public interface IInteractable
{
    void OnInteract(bool resetInteraction = false);
    bool CanInteract();
    void DisplayInfo(bool display, RaycastHit hit = new RaycastHit(), Camera camera = null);
}
