using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Type of node in the circuit.
/// Determines how the node behaves during value calculation and resistance summation.
/// </summary>
public enum NodeType
{
    SOURCE,
    NODE_ACTIVE,
    MEASURE_POINT,
    NODE_CONTROL,
    BRANCH_OUT,
    BRANCH_IN,
    END,
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
    public float Uc;
    public float Ic;
    public float Rc;

    public NodeDataModel(float Uc, float Ic, float Rc, CircuitValueType sType, SignalType cType)
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
    public void UpdateReduceValues(float U, float R)
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
    public void SetValues(float Uc, float Ic, float Rc)
    {
        this.Uc = Uc;
        this.Ic = Ic;
        this.Rc = Rc;
    }
}