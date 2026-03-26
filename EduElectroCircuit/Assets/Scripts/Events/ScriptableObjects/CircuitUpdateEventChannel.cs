using System;
using UnityEngine;


[CreateAssetMenu(fileName = "CircuitUpdateEventChannel", menuName = "Events/CircuitUpdateEventChannel")]
public class CircuitUpdateEventChannel : GenericEventChannel<CircuitUpdateEvent>
{
}