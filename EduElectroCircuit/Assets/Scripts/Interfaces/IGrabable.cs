/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: IGrabable
 */

using System;
using UnityEngine;

public interface IGrabable
{
    public bool CanGrab();
    public void LockGrab(bool locked);
    public void OnGrab(bool grabbed, GameObject caller);
    void DisplayInfo(bool display);
}