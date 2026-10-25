/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: Node_GND
 */

public class Node_GND : Node
{
    public void Awake()
    {
        R.Refresh();
        U.Refresh();
        I.Refresh();
    }
    public override void CalculateValues(NodeDataModel passValues, NodeDataModel originValues) { }

    public override ComplexFloat GetResistanceSum(SignalType signalType, float frequency) { return R; }

    public override bool CheckConection() { return _connected; }

    public override void BuildConections(Node branchInRef, int branchId, string containerName) { }
}
