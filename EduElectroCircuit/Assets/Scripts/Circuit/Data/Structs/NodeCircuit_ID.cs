using System;

[System.Serializable]
public struct NodeCircuit_ID
{
    public int roomID;
    public int circuitID;

    public NodeCircuit_ID(int roomID, int circuitID = 0)
    {
        this.roomID = roomID;
        this.circuitID = circuitID;
    }

    public static bool operator ==(NodeCircuit_ID a, NodeCircuit_ID b)
    => a.Equals(b);

    public static bool operator !=(NodeCircuit_ID a, NodeCircuit_ID b)
    => !a.Equals(b);

    public override bool Equals(object obj)
    {
        if (obj is not NodeCircuit_ID other)
            return false;

        return
            roomID == other.roomID && circuitID == other.circuitID;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(roomID, circuitID);
    }

    public bool RoomMatch(NodeCircuit_ID obj)
    => roomID == obj.roomID;
}