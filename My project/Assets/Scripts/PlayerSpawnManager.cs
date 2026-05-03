using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;

public class PlayerSpawnManager : NetworkBehaviour
{
    private readonly List<int> _quadrantQueue = new List<int>();
    private int _queueIndex = 0;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        RefillShuffledQueue();
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
    }

    public void OnClientConnected(ulong clientId)
    {
        StartCoroutine(AssignSpawnPositionCoroutine(clientId));
    }

    private IEnumerator AssignSpawnPositionCoroutine(ulong clientId)
    {
        yield return null;

        if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var client) || 
            client.PlayerObject == null )
        {
            Debug.LogWarning($"[PlayerSpawnManager] Playerbject for client {clientId} not found.");
            yield break;
        }

        Renderer rend = gameObject.GetComponent<Renderer>();
        if (rend == null)
        {
            Debug.LogError("[PlayerSpawnManager] renderer not found attached.");
            yield break;
        }

        Bounds bounds = rend.bounds;
        int quadrant = NextQuadrant();
        Vector3 spawnPos = GetRandomPositionInQuadrant(bounds, quadrant, transform.position.y + 0.5f);

        client.PlayerObject.transform.position = spawnPos;
        Debug.Log($"[PlayerSpawnManager] client {clientId} assigned to quadrant {quadrant} at {spawnPos}");
    }

    private int NextQuadrant()
    {
        if (_queueIndex >= _quadrantQueue.Count)
        {
            RefillShuffledQueue();
        }
        return _quadrantQueue[_queueIndex++];
    }

    private void RefillShuffledQueue()
    {
        _quadrantQueue.Clear();
        _queueIndex = 0;

        List<int> pool = new List<int> { 0, 1, 2, 3 };
        while (pool.Count > 0)
        {
            int i = Random.Range(0, pool.Count);
            _quadrantQueue.Add(pool[i]);
            pool.RemoveAt(i);
        }
    }

    private Vector3 GetRandomPositionInQuadrant(Bounds bounds, int quadrant, float spawnY)
    {
        float cx = bounds.center.x;
        float cz = bounds.center.z;

        float minX = 0, maxX = 0, minZ = 0, maxZ = 0;
        switch (quadrant)
        {
            case 0: // Top Left quadrant
                minX = bounds.min.x; 
                maxX = cx;
                minZ = cz;
                maxZ = bounds.max.z;
                break;
            case 1: // Top Right quadrant
                minX = cx; 
                maxX = bounds.max.x;
                minZ = cz;
                maxZ = bounds.max.z;
                break;
            case 2: // Bottom Riht quadrant
                minX = cx; 
                maxX = bounds.max.x;
                minZ = bounds.min.z;
                maxZ = cz;
                break;
            default: // Bottom Left quadrant
                minX = bounds.min.x; 
                maxX = cx;
                minZ = bounds.min.z;
                maxZ = cz;
                break;
        }

        return new Vector3(
            Random.Range(minX, maxX),
            spawnY,
            Random.Range(minZ, maxZ)
        );
    }
}