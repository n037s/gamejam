using UnityEngine;
using Unity.Netcode;
using System.Collections;
using Unity.Multiplayer.Center.NetcodeForGameObjectsExample;

[RequireComponent(typeof(NetworkObject))]
public class PlayerManager : NetworkBehaviour
{
    // Boolean value to know if the player is the current player or another guy
    public bool isCurrentPlayer = false;

    public int armor = 0;

    public int maxLife = 100;
    private static int maxLifeStart = 100;

    private Rigidbody rb;
    private bool isFrozen = false;

    public NetworkVariable<int> life = new NetworkVariable<int>(
        maxLifeStart,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public NetworkVariable<int> score = new NetworkVariable<int>(
        0, 
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public override void OnNetworkSpawn()
    {
        isCurrentPlayer = IsOwner;

        Transform lifeBar = transform.Find("LifeBarCanvas");
        if (lifeBar != null)
        {
            lifeBar.GetComponent<LifeBar>().UpdateBarColor(isCurrentPlayer);
        }
        else
        {
            Debug.LogWarning("LifeBar child not found.");
        }
    }
    
    void Start()
    {
    }

    void Update()
    {

    }

    public void SetLife(int value)
    {
        if (IsServer)
            life.Value = Mathf.Clamp(value, 0, maxLife);
        else
            SetLifeServerRpc(value);
    }

    public void TakeDamage(int rawAmount, GameObject takenFrom)
    {
        int amount = (int)(((100 - armor)/100.0f) * rawAmount);
        AddLife(-amount, takenFrom);
    }

    public void AddLife(int amount, GameObject takenFrom = null)
    {
        if (IsServer)
        {
            life.Value = Mathf.Clamp(life.Value + amount, 0, maxLife);

            if (life.Value == 0)
                Death(takenFrom);
        }
        else
        {
            ulong sourceId = (takenFrom != null && takenFrom.TryGetComponent<NetworkObject>(out var no)) 
                ? no.NetworkObjectId
                : ulong.MaxValue;
            
            AddLifeServerRpc(amount, sourceId);
        }
    }

    private void Death(GameObject killer)
    {
        Debug.Log("Player is dead");

        if (killer != null)
        {
            PlayerManager killerManager = killer.GetComponent<PlayerManager>();
            if (killerManager != null)
                killerManager.score.Value += 10;
        }

        rb = GetComponent<Rigidbody>();
        if (!isFrozen)
        {
            Debug.Log("Player is notfrozen");
            StartCoroutine(FreezeFor10Seconds());
        }
     

    }

    IEnumerator FreezeFor10Seconds()
    {
        Debug.Log("Coroutine lancee");
        isFrozen = true;


        // Immobilise l'objet

        ClientAuthoritativeMovement ClientAuthoritativeMovementInstance = GetComponent<ClientAuthoritativeMovement>();
        int TempSpeed = ClientAuthoritativeMovementInstance.Speed;
        ClientAuthoritativeMovementInstance.Speed = 0; 

        //Bloque les tirs
        PlayerShoot PlayerShootInstance = GetComponent<PlayerShoot>();
        PlayerShootInstance.isdead = true;
        GetComponent<Collider>().enabled = false; // Désactive le collider pour éviter les interactions pendant la mort

        yield return new WaitForSeconds(10f);

      
        //Remet la vitesse d'origine, débloque les tirs et réactive le collider

        ClientAuthoritativeMovementInstance.Speed = TempSpeed;
        PlayerShootInstance.isdead = false;
        GetComponent<Collider>().enabled = true;

        //On remet la vie au max
        life.Value = maxLife; 


        isFrozen = false;
    }
    public void SetScore(int value)
    {
        if (IsServer)
            score.Value = value;
        else
            SetScoreServerRpc(value);
    }

    public void AddScore(int amount)
    {
        if (IsServer)
            score.Value += amount;
        else
            AddScoreServerRpc(amount);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetLifeServerRpc(int value)
    {
        life.Value = Mathf.Clamp(value, 0, maxLife);
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddLifeServerRpc(int amount, ulong sourceNetworkObjectId = ulong.MaxValue)
    {
        life.Value = Mathf.Clamp(life.Value + amount, 0, maxLife);
        if (life.Value == 0)
        {
            GameObject source = null;
            if (sourceNetworkObjectId != ulong.MaxValue && 
                NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(sourceNetworkObjectId, out var sourceNo))
            {
                source = sourceNo.gameObject;
            }
            Death(source);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetScoreServerRpc(int value)
    {
        score.Value = value;
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddScoreServerRpc(int amount)
    {
        score.Value += amount;
    }
}
