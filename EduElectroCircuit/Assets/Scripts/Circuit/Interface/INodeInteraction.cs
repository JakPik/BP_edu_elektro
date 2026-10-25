/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: INodeInteraction
 */

using UnityEngine;

/// <summary>
/// Defines the contract for interacting with a node.
/// </summary>
public interface INodeInteraction
{
    /// <summary>
    /// Gets the node´s position and transform.
    /// </summary>
    /// <returns>
    /// A tuple containing the anchor position for attaching an object
    /// and the node's transform.
    /// </returns>
    public (Vector3 AnchorPosition, Transform NodeTransform) GetTransform();

    /// <summary>
    /// Sets the component data used for calculations on the node.
    /// Accepts instances of <see cref="ComponentData"/> or its derived types.
    /// </summary>
    /// <param name="componentData">
    /// The component data to assign to the node, such as
    /// <see cref="ComponentData"/> or a derived type.
    /// </param>
    /// <param name="circuitComponent">
    /// A reference to the circuit component providing the data.
    /// </param>
    public void SetComponentData(ComponentData componentData, GameObject circuitComponent);

    /// <summary>
    /// Determines whether a component can connect to the node.
    /// </summary>
    /// <param name="type">
    /// The type of component to check for connection eligibility.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the component can connect; otherwise, <see langword="false"/>.
    /// </returns>
    public bool CanConnect(ComponentType type);

    /// <summary>
    /// Determines whether a component can be grabed from the Node.
    /// <returns>
    /// <see langword="true"/> if the component can be grabed; otherwise, <see langword="false"/>.
    /// </returns>
    public bool CanGrab();
}