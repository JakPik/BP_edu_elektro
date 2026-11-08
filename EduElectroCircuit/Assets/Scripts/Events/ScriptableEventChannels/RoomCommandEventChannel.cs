/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: RoomCommandEventChannel
 */

using System;
using UnityEngine;


[CreateAssetMenu(fileName = "RoomCommandEventChannel", menuName = "Events/RoomCommandEventChannel")]
public class RoomCommandEventChannel : GenericEventChannel<RoomCommandEvent>
{
}