/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: UIPanel
 */

using UnityEngine;
using UnityEngine.UIElements;

public abstract class UIPanel : MonoBehaviour
{
    [SerializeField] protected UIDocument display;
    public abstract bool SetData<T>(T data);    
}