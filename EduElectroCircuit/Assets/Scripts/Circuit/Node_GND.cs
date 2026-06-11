/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: Node_GND
 */

using UnityEngine;
using System;

public class Node_GND : Node
{
    public override void CalculateValues(NodeDataModel passValues, NodeDataModel originValues) { return; }

    public override (float, bool) GetResistanceSum() { return (R, connected); }

    public override void BuildConections(Node branchInRef, int branchId) { return; }
}
