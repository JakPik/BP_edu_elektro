using UnityEngine;

public enum Node_Type
{
    SOURCE,
    NODE_ACTIVE,
    NODE_PASSIVE,
    BRANCH_OUT,
    BRANCH_IN,
    END
}

public enum Source_Type
{
    VOLTAGE,
    CURRENT
}

public enum Circuit_Type
{
    DC,
    AC
}
public class NodeDataModel
{
    public Source_Type source_Type;
    public Circuit_Type circuit_Type;
    public float Uc;
    public float Ic;
    public float Rc;

    public NodeDataModel(float Uc, float Ic, float Rc, Source_Type sType, Circuit_Type cType)
    {
        this.Uc = Uc;
        this.Ic = Ic;
        this.Rc = Rc;

        source_Type = sType;
        circuit_Type = cType;
    }

    public void UpdateReduceValues(float Uc, float Ic, float Rc)
    {
        this.Uc -= Uc;
        this.Ic -= Ic;
        this.Rc -= Rc;
    }

    public void SetValues(float Uc, float Ic, float Rc)
    {
        this.Uc = Uc;
        this.Ic = Ic;
        this.Rc = Rc;
    }
}
