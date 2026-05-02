using UnityEngine;

public class Ressources_spawner : MonoBehaviour
{

    [Header("Prefab")]
    public GameObject Bois = null;
    public GameObject Pierre = null;
    public GameObject Fer = null;
    public int numberToSpawn_bois = 10;
    public int numberToSpawn_pierre = 10;
    public int numberToSpawn_fer = 10;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
   public void Start()
    {
        Renderer rend = GetComponent<Renderer>();
        Bounds bounds = rend.bounds;
        for (int i = 0; i < numberToSpawn_bois; i++)
        {
            float randomX_bois = Random.Range(bounds.min.x, bounds.max.x);
            float randomZ_bois = Random.Range(bounds.min.z, bounds.max.z);

            Vector3 spawnPosition = new Vector3(
                randomX_bois,
                transform.position.y,
                randomZ_bois
            );
            Instantiate(Bois, spawnPosition, Quaternion.identity);
        }

        for (int i = 0; i < numberToSpawn_pierre; i++)
        {
            float randomX_pierre = Random.Range(bounds.min.x, bounds.max.x);
            float randomZ_pierre = Random.Range(bounds.min.z, bounds.max.z);

            Vector3 spawnPosition = new Vector3(
                randomX_pierre,
                transform.position.y,
                randomZ_pierre
            );
            Instantiate(Pierre, spawnPosition, Quaternion.identity);
        }

        for (int i = 0; i < numberToSpawn_fer; i++)
        {
            float randomX_fer = Random.Range(bounds.min.x, bounds.max.x);
            float randomZ_fer = Random.Range(bounds.min.z, bounds.max.z);

            Vector3 spawnPosition = new Vector3(
                randomX_fer,
                transform.position.y,
                randomZ_fer
            );
            Instantiate(Fer, spawnPosition, Quaternion.identity);
        }
    }


    
}
