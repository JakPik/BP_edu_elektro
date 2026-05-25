/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: ReloadRequestEventChannel
 */

using System;
using UnityEngine;


[CreateAssetMenu(fileName = "ReloadRequestEventChannel", menuName = "Events/ReloadRequestEventChannel")]
public class ReloadRequestEventChannel : GenericEventChannel<ReloadRequestEvent>
{
}