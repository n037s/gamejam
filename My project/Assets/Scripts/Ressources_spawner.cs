using UnityEngine;
using Unity.Netcode; 

public class Ressources_spawner : NetworkBehaviour
{

    [Header("Prefab")]
    public GameObject Bois = null;
    public GameObject Pierre = null;
    public GameObject Fer = null;
    public int numberToSpawn_bois = 10;
    public int numberToSpawn_pierre = 10;
    public int numberToSpawn_fer = 10;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnNetworkSpawn()
    {
        // Only the host spawns object and then send them to clients

        if (NetworkManager.Singleton.IsHost)
        {
            Renderer rend = GetComponent<Renderer>();
            Bounds bounds = rend.bounds;
            
            SpawnItems(Bois, numberToSpawn_bois, bounds);
            SpawnItems(Pierre, numberToSpawn_pierre, bounds);
            SpawnItems(Fer, numberToSpawn_fer, bounds);
        }
        else
        {
            Debug.Log("Not the host, do not make spawn items");
        }
    }

    void SpawnItems(GameObject prefab, int count, Bounds bounds)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                transform.position.y,
                Random.Range(bounds.min.z, bounds.max.z)
            );
            GameObject obj = Instantiate(prefab, pos, Quaternion.identity);
            obj.GetComponent<NetworkObject>().Spawn();
        }
    }

    
}
