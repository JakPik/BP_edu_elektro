using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelControl : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private GameObject[] spawnPoints;
    [SerializeField] private GenericVoidEventChannel[] roomReloadEventChannel;
    [SerializeField] private int curSpawnPointIndex = 0;
    [SerializeField] private int numOfRespawns = 0;
    [SerializeField] private int startTime = 0;
    [SerializeField] private float animSpeedIn = 3.5f;
    [SerializeField] private float animSpeedOut = 1.0f;
    [SerializeField] public ColorSchemeSO colorScheme;
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
        numOfRespawns++;
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

    public IEnumerator Fade(bool fadeIn)
    {
        float progress = 0f;
        VisualElement panel = uiDocument.rootVisualElement.Q<VisualElement>("Fade");
        float alpha = fadeIn ? 0f : 1f;
        while (progress < 1f)
        {
            progress += Time.deltaTime * (fadeIn ? animSpeedIn : animSpeedOut);
            panel.style.backgroundColor = new Color(0f, 0f, 0f, alpha + progress * (fadeIn ? 1 : -1));
            yield return null;
        }
    }

    public IEnumerator Respawn()
    {
        yield return StartCoroutine(Fade(true));

        roomReloadEventChannel[curSpawnPointIndex]?.RaiseEvent(this.name);

        yield return new WaitForSeconds(0.8f);
        yield return StartCoroutine(Fade(false));
    }

    public IEnumerator LevelLoad()
    {
        
        roomReloadEventChannel[curSpawnPointIndex]?.RaiseEvent(this.name);

        yield return new WaitForSeconds(0.8f);
        yield return StartCoroutine(Fade(false));
    }

}
