using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelControl : MonoBehaviour
{
    [SerializeField] private GameObject[] spawnPoints;
    [SerializeField] private GenericVoidEventChannel[] roomReloadEventChannel;
    [SerializeField] private GenericVoidEventChannel globalReloadEventChannel;
    [SerializeField] private int curSpawnPointIndex = 0;
    [SerializeField] private int numOfRespawns = 0;
    [SerializeField] private float startTime = 0;
    public static LevelControl Instance;
    

    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public (Vector3, Quaternion) RespawnPlayer()
    {
        if(spawnPoints.Length == 0)
        {
            Logger.Log(this.name, "No spawn points set", LogType.ERROR);
            return (Vector3.zero, Quaternion.identity);
        }
        
        return (spawnPoints[curSpawnPointIndex].transform.position, spawnPoints[curSpawnPointIndex].transform.rotation);
    }

    public void SetNextSpawnPoint(GameObject spawnPoint)
    {
        if(spawnPoints.Length == 0)
        {
            Logger.Log(this.name, "No spawn points set", LogType.ERROR);
            return;
        }
        curSpawnPointIndex = System.Array.IndexOf(spawnPoints, spawnPoint);
        if(curSpawnPointIndex == -1)
        {
            Logger.Log(this.name, "Spawn point " + spawnPoint.name + " was not found in spawnPoints array", LogType.ERROR);
            return;
        }
    }

    public IEnumerator Respawn()
    {
        numOfRespawns++;
        yield return StartCoroutine(GameUIControl.Instance.Fade(true));
        globalReloadEventChannel?.RaiseEvent(this.name);
        if(curSpawnPointIndex != 0) roomReloadEventChannel[curSpawnPointIndex]?.RaiseEvent(this.name);

        yield return new WaitForSeconds(0.8f);
        yield return StartCoroutine(GameUIControl.Instance.Fade(false));
    }

    public IEnumerator LevelLoad()
    {
        numOfRespawns = 0;
        curSpawnPointIndex = 0;
        startTime = Time.time * 1000f;
        globalReloadEventChannel?.RaiseEvent(this.name);
        foreach(var channel in roomReloadEventChannel)
        {
            channel?.RaiseEvent(this.name);
        }

        yield return new WaitForSeconds(0.8f);
        yield return StartCoroutine(GameUIControl.Instance.Fade(false));
    }

}
