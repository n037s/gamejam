using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonSpawnAndLoadScene : MonoBehaviour
{
    [Header("Prefab Spawn")]
    public GameObject prefabToSpawn;
    public Transform spawnPoint;
    public bool keepSpawnedObjectAcrossScenes = false;

    [Header("Scene Load")]
    public string sceneName;

    public void OnClickSpawnAndLoadScene()
    {
        if (prefabToSpawn != null)
        {
            GameObject spawned = Instantiate(prefabToSpawn, spawnPoint ? spawnPoint.position : Vector3.zero, spawnPoint ? spawnPoint.rotation : Quaternion.identity);
            if (keepSpawnedObjectAcrossScenes)
            {
                DontDestroyOnLoad(spawned);
            }
        }
        else
        {
            Debug.LogWarning("ButtonSpawnAndLoadScene: prefabToSpawn is not assigned.");
        }

        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("ButtonSpawnAndLoadScene: sceneName is not assigned.");
        }
    }
}
