using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomProgressControl : MonoBehaviour
{
    public int saveID;
    private RoomDTO roomData = null;
    [SerializeField] private RoomControl[] rooms;
    public IEnumerator LoadRoomData(int? loadID = null)
    {
        roomData = RoomJsonManager.LoadJsonFile(SceneManager.GetActiveScene().name, loadID);
        if(roomData != null)
        {
            int roomLoadId = 0;
            foreach(ComponentsDTO componentsDTO in roomData.components)
            {
                if(loadID == null)
                {
                    rooms[roomLoadId].LoadComponentSetUp(componentsDTO);
                }
                else
                {
                    rooms[roomLoadId].LoadSetUp(componentsDTO);
                }
                roomLoadId++;
                yield return null;
            }
        }
    }

    public IEnumerator ReloadRooms(int roomID)
    {
        if(roomID != 0)
        {
            for(int i = roomID - 1; i >= 0; i--)
            {
                if(i != roomID - 1 && !rooms[i].GetIsReturnable()) break;
                rooms[i].ReloadComponets(i != roomID - 1);
                yield return null;
            }
        }
    }

    public void SaveRoomStates(int previousRoomID, int saveRoomID, int numberOfActivations, int numberOfRespawns, bool levelEnd = false)
    {
        if(levelEnd)
        {
            rooms[saveRoomID-1].UpdateGradeValues(numberOfActivations, numberOfRespawns);
            rooms[saveRoomID-1].SaveComponents();
            return;
        }
        if(saveRoomID != 0)
        {
            for(int i = saveRoomID - 1; i >= 0; i--)
            {
                if(i == previousRoomID - 1)
                {
                    rooms[i].UpdateGradeValues(numberOfActivations, numberOfRespawns);
                    rooms[i].SaveComponents();
                }
                if(i != saveRoomID - 1 &&!rooms[i].GetIsReturnable()) break;
                rooms[i].SaveComponents();
            }
        }
    }

    public void SaveRoomData()
    {
        int roomSaveId = 0;
        roomData = new RoomDTO{
            components = new ComponentsDTO[rooms.Length]
        };
        foreach(RoomControl room in rooms)
        {
            roomData.components[roomSaveId] = room.SaveComponentSetUp();
            roomSaveId++;
        }
        RoomJsonManager.SaveJsonFile(SceneManager.GetActiveScene().name, roomData, saveID);
    }

    public EvaluationGrade[] GetRoomsGrades()
    {
        List<EvaluationGrade> roomGrades = new List<EvaluationGrade>();
        foreach(RoomControl room in rooms)
        {
            roomGrades.Add(room.GetRoomGrade());
        }
        return roomGrades.ToArray();
    }

    [ContextMenu("Save Components")]
    public void SaveCurrentRoomData()
    {
        SaveRoomData();
    }

    [ContextMenu("Load Components")]
    public void LoadSpecificRoomData()
    {
        StartCoroutine(LoadRoomData(saveID));
    }
}
