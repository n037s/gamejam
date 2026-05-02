using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(NetworkObject))]
public class PlayerManager : NetworkBehaviour
{
    // Boolean value to know if the player is the current player or another guy
    public bool isCurrentPlayer = false;

    public int armor = 0;

    public NetworkVariable<int> life = new NetworkVariable<int>(
        100,
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
        if (isCurrentPlayer)
        {
            GetComponent<Renderer>().material.color = Color.blue;
        }
        else
        {
            GetComponent<Renderer>().material.color = Color.red;
        }
    }

    void Update()
    {

    }

    public void SetLife(int value)
    {
        if (IsServer)
            life.Value = Mathf.Clamp(value, 0, 100);
        else
            SetLifeServerRpc(value);
    }

    public void TakeDamage(int rawAmount)
    {
        int amount = (int)(((100 - armor)/100.0f) * rawAmount);
        AddLife(-amount);
    }

    public void AddLife(int amount)
    {
        if (IsServer)
            life.Value = Mathf.Clamp(life.Value + amount, 0, 100);
        else
            AddLifeServerRpc(amount);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetLifeServerRpc(int value)
    {
        life.Value = Mathf.Clamp(value, 0, 100);
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddLifeServerRpc(int amount)
    {
        life.Value = Mathf.Clamp(life.Value + amount, 0, 100);
    }
}
