/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: UIPanel
 */

using UnityEngine;

public abstract class UIPanel : MonoBehaviour
{
    public abstract void SetData<T>(T data);
}