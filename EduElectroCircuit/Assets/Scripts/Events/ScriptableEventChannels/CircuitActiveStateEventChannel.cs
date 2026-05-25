/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: CircuitActiveStateEventChannel
 */

using System;
using UnityEngine;


[CreateAssetMenu(fileName = "CircuitActiveStateEventChannel", menuName = "Events/CircuitActiveStateEventChannel")]
public class CircuitActiveStateEventChannel : GenericEventChannel<CircuitActiveStateEvent>
{
}