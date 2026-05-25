/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: ReloadEventChannel
 */

using System;
using UnityEngine;


[CreateAssetMenu(fileName = "ReloadEventChannel", menuName = "Events/ReloadEventChannel")]
public class ReloadEventChannel : GenericEventChannel<ReloadEvent>
{
}