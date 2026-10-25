using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

public static class RoomJsonManager
{
    public static RoomDTO LoadJsonFile(string level, /*string room,*/ int? load_saveID)
    {
        string resourcePath = 
        "Data/" +
        level/* +
        "/" +
        room*/;

        TextAsset[] files = Resources.LoadAll<TextAsset>(resourcePath);

        if (files == null || files.Length == 0)
        {
            Logger.Log(
                "JSON_RoomModule",
                "LoadJsonFile",
                "No JSON files found at Resources/" + resourcePath,
                LogType.WARNING
            );

            return null;
        }

        if(load_saveID != null && load_saveID < files.Length) {
            return JsonUtility.FromJson<RoomDTO>(files[load_saveID.Value].text);
        }
        else
        {
            return JsonUtility.FromJson<RoomDTO>(files[Random.Range(0, files.Length)].text);
        }
    }

     public static void SaveJsonFile(string level, /*string room, */RoomDTO roomData, int load_saveID)
    {
        string json = JsonUtility.ToJson(roomData, true);

        string resourcePath = 
            "Data/" +
            level +
            "/"/* +
            room +
            "/"*/;

        string directory = Path.Combine(
            Application.dataPath,
            "Resources",
            resourcePath
        );

        Directory.CreateDirectory(directory);

        TextAsset[] files = Resources.LoadAll<TextAsset>(resourcePath);
        int saveID = load_saveID == -1? files.Length : load_saveID;
        File.WriteAllText(Application.dataPath + "/Resources/" + resourcePath + "RoomData_" + saveID + ".json", json);
        Logger.Log("JSON_RoomModule", "SaveJsonFile", "Component set up saved to file", LogType.INFO);
    }

}