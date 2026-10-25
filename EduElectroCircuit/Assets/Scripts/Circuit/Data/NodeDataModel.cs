/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: NodeDataModel
 */

/// <summary>
/// Data model for passing values between nodes in a circuit.  
/// Stores voltage, current, and resistance along with node source and circuit type information.
/// </summary>
public class NodeDataModel
{
    public ElectricalQuantityType sourceType;
    public SignalType circuitType;
    public ComplexFloat Uc;
    public ComplexFloat Ic;
    public ComplexFloat Rc;

    public NodeDataModel(ComplexFloat Uc, ComplexFloat Ic, ComplexFloat Rc, ElectricalQuantityType sType, SignalType cType)
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


