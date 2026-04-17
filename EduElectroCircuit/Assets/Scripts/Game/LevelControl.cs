using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelControl : MonoBehaviour
{
    [Header("Spawn points")]
    [SerializeField] private GameObject[] spawnPoints;

    [Header("Raised Events")]
    [SerializeField] private GenericEventChannel<ReloadEvent> reload;

    [Header("Event Channels")]
    [SerializeField] private GenericEventChannel<ReloadRequestEvent> reloadRequest;

    [Header("Control Variables")]
    [SerializeField] private int curSpawnPointIndex = 0;
    [SerializeField] private int numOfRespawns = 0;
    [SerializeField] private float startTime = 0;
    public static LevelControl Instance;

    private Coroutine curCoroutine;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void SetNextSpawnPoint(GameObject spawnPoint)
    {
        if (spawnPoints.Length == 0)
        {
            Logger.Log(this, this.name, "No spawn points set", LogType.ERROR);
            return;
        }
        curSpawnPointIndex = System.Array.IndexOf(spawnPoints, spawnPoint);
        if (curSpawnPointIndex == -1)
        {
            Logger.Log(this, this.name, "Spawn point " + spawnPoint.name + " was not found in spawnPoints array", LogType.ERROR);
            return;
        }
    }

    private IEnumerator Respawn()
    {
        numOfRespawns++;
        yield return StartCoroutine(GameUIControl.Instance.Fade(true));

        reload.RaiseEvent(new ReloadEvent(spawnPoints[curSpawnPointIndex].transform, spawnPoints[curSpawnPointIndex]), this.name);

        yield return new WaitForSeconds(0.8f);
        yield return StartCoroutine(GameUIControl.Instance.Fade(false));
        curCoroutine = null;
    }

    private IEnumerator LoadLevel()
    {
        numOfRespawns = 0;
        curSpawnPointIndex = 0;
        startTime = Time.time * 1000f;

        reload.RaiseEvent(new ReloadEvent(spawnPoints[curSpawnPointIndex].transform, null), this.name);

        yield return new WaitForSeconds(0.8f);
        yield return StartCoroutine(GameUIControl.Instance.Fade(false));
        curCoroutine = null;
    }

    void OnEnable()
    {
        reloadRequest.OnEventRaised += OnReloadRequest;
    }

    void OnDisable()
    {
        reloadRequest.OnEventRaised -= OnReloadRequest;
    }

    private void OnReloadRequest(ReloadRequestEvent @event)
    {
        if (curCoroutine != null) return;
        if (@event.FirstLoad)
        {
            curCoroutine = StartCoroutine(LoadLevel());
        }
        else
        {
            curCoroutine = StartCoroutine(Respawn());
        }
    }

}
