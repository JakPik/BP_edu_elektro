/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: NodeValidationEventChannel
 */

using System;
using UnityEngine;


[CreateAssetMenu(fileName = "NodeValidationEventChannel", menuName = "Events/NodeValidationEventChannel")]
public class NodeValidationEventChannel : GenericEventChannel<NodeValidationEvent>
{
}