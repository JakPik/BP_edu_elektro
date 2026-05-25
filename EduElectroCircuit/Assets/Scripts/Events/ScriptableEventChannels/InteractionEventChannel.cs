/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: InteractionEventChannel
 */

using System;
using UnityEngine;


[CreateAssetMenu(fileName = "InteractionEventChannel", menuName = "Events/InteractionEventChannel")]
public class InteractionEventChannel : GenericEventChannel<ButtonPressedEvent>
{
}