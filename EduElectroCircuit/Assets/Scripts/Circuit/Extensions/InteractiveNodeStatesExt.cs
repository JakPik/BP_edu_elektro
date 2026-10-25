using UnityEngine;

public static class InteractiveNodeStatesExt
{
    public static Color ToColor(NodeConectionStates nodeState)
    {
        switch (nodeState)
        {
            case NodeConectionStates.PLACED_COMPONENT:
                return Color.white;
            case NodeConectionStates.ACTIVE_OPEN_CIRCUIT:
                return new Color(1f,0.5f,0f);
            case NodeConectionStates.ACTIVE_CLOSED_CIRCUIT:
                return Color.blue;
            case NodeConectionStates.ACTIVE_ON_INTERACTION:
                return Color.red;
            case NodeConectionStates.EMPTY:
            default:
                return new Color(0f,0f,0f);
        }
    }
}