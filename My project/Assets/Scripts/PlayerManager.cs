using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(NetworkObject))]
public class PlayerManager : NetworkBehaviour
{
    // Boolean value to know if the player is the current player or another guy
    public bool isCurrentPlayer = false;

    public override void OnNetworkSpawn()
    {
        isCurrentPlayer = IsOwner;
    }
    
    void Start()
    {
        if (IsOwner)
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
}
