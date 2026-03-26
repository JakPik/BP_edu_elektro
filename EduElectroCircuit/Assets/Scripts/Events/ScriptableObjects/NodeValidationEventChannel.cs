using System;
using UnityEngine;


[CreateAssetMenu(fileName = "NodeValidationEventChannel", menuName = "Events/NodeValidationEventChannel")]
public class NodeValidationEventChannel : GenericEventChannel<NodeValidationEvent>
{
}