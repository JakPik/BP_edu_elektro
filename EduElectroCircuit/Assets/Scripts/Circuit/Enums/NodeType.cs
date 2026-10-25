/// <summary>
/// Type of node in the circuit.
/// Determines how the node behaves during value calculation and resistance summation.
/// </summary>
public enum NodeType
{
    SOURCE,
    INTERACTIVE,
    MEASUREMENT,
    VALIDATION,
    BRANCH_OUT,
    BRANCH_IN,
    GND,
}