/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: NodeDataModel
 */

using UnityEngine;
using UnityEngine.InputSystem;

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

/// <summary>
/// Type of circuit.
/// Determines if circuit is DC or AC.
/// </summary>
public enum SignalType
{
    DC,
    AC
}

/// <summary>
/// Data model for passing values between nodes in a circuit.  
/// Stores voltage, current, and resistance along with node source and circuit type information.
/// </summary>
public class NodeDataModel
{
    public CircuitValueType sourceType;
    public SignalType circuitType;
    public ComplexFloat Uc;
    public ComplexFloat Ic;
    public ComplexFloat Rc;

    public NodeDataModel(ComplexFloat Uc, ComplexFloat Ic, ComplexFloat Rc, CircuitValueType sType, SignalType cType)
    {
        this.Uc = Uc;
        this.Ic = Ic;
        this.Rc = Rc;

        sourceType = sType;
        circuitType = cType;
    }

    /// <summary>
    /// Decremets Voltage and Resistance by given values.
    /// </summary>
    /// <param name="U">Node Voltage</param>
    /// <param name="R">Node Resistance</param>
    public void UpdateReduceValues(ComplexFloat U, ComplexFloat R)
    {
        Uc -= U;
        Rc -= R;
    }

    /// <summary>
    /// Sets the values based on given parameters.
    /// </summary>
    /// <param name="Uc">Voltage value</param>
    /// <param name="Ic">Current value</param>
    /// <param name="Rc">Resistance value</param>
    public void SetValues(ComplexFloat Uc, ComplexFloat Ic, ComplexFloat Rc)
    {
        this.Uc = Uc;
        this.Ic = Ic;
        this.Rc = Rc;
    }
}

public struct PortData
{
    public Vector3 portDirection;
    public Vector3 portPosition;
    public Vector3? portLocalPosition;

    public PortData(Vector3 portDirection, Vector3 portPosition, Vector3? portLocalPosition)
    {
        this.portDirection = portDirection;
        this.portPosition = portPosition;
        this.portLocalPosition = portLocalPosition;
    }
}
