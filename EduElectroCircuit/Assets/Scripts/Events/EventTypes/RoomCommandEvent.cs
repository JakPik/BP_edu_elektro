/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: RoomCommandEvent
 */

public class RoomCommandEvent : EventBaseType
{
    public RoomCommand Command;
    public int RoomID;

    public RoomCommandEvent(RoomCommand command, int roomID)
    {
        Command = command;
        RoomID = roomID;
    }

    public override string DisplayData()
    {
        return "(Room " + RoomID + " Command: " + Command.ToString() + ")";
    }
}