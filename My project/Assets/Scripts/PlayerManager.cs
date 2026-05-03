using UnityEngine;
using Unity.Netcode;
using UnityEngine.UIElements;
using System.Collections;
using Unity.Multiplayer.Center.NetcodeForGameObjectsExample;
using Unity.Collections;

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

    public NetworkVariable<FixedString64Bytes> networkPlayerName = new NetworkVariable<FixedString64Bytes>(
        new FixedString64Bytes("Player"),
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public NetworkVariable<int> canonLevel = new NetworkVariable<int>(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> muraillesLevel = new NetworkVariable<int>(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> moteurLevel = new NetworkVariable<int>(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public NetworkVariable<int> networkMaxLife = new NetworkVariable<int>(
        maxLifeStart, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        isCurrentPlayer = IsOwner;

        score.OnValueChanged += OnScoreChanged;
        networkPlayerName.OnValueChanged += OnPlayerNameChanged;
        canonLevel.OnValueChanged += OnCanonLevelChanged;
        muraillesLevel.OnValueChanged += OnMuraillesLevelChanged;
        moteurLevel.OnValueChanged += OnMoteurLevelChanged;
        networkMaxLife.OnValueChanged += OnNetworkMaxLifeChanged;

        if (canonLevel.Value > 0) GetComponent<UpgradeVisible>()?.UpdateAttackVisibility(canonLevel.Value);
        if (muraillesLevel.Value > 0) GetComponent<UpgradeVisible>()?.UpdateDefenseVisibility(muraillesLevel.Value);
        if (moteurLevel.Value > 0) GetComponent<UpgradeVisible>()?.UpdateSpeedVisibility(moteurLevel.Value);
        maxLife = networkMaxLife.Value;

        // Apply name that is already synced. 
        gameObject.name = networkPlayerName.Value.ToString();

        Transform lifeBar = transform.Find("LifeBarCanvas");
        if (lifeBar != null)
        {
            lifeBar.GetComponent<LifeBar>().UpdateBarColor(isCurrentPlayer);
        }
        else
        {
            Debug.LogWarning("LifeBar child not found.");
        }

        if (IsOwner)
        {
            string playerName = PlayerPrefs.GetString("PlayerName", $"Player {OwnerClientId}");
            Debug.Log($"LEO - retrieve the player name {playerName}");
            SetNameServerRpc(playerName);
        }

        GameObject scorePanel = GameObject.FindWithTag("ScorePanel");
        if (scorePanel != null)
        {
            ScoreDisplay sd = scorePanel.GetComponent<ScoreDisplay>();
            if (sd != null)
                sd.RegisterPlayer(this);
        }
    }

    public override void OnNetworkDespawn()
    {
        score.OnValueChanged -= OnScoreChanged;
        networkPlayerName.OnValueChanged -= OnPlayerNameChanged;
        canonLevel.OnValueChanged -= OnCanonLevelChanged;
        muraillesLevel.OnValueChanged -= OnMuraillesLevelChanged;
        moteurLevel.OnValueChanged -= OnMoteurLevelChanged;
        networkMaxLife.OnValueChanged -= OnNetworkMaxLifeChanged;
        
        GameObject scorePanel = GameObject.FindWithTag("ScorePanel");
        if (scorePanel != null)
        {
            ScoreDisplay sd = scorePanel.GetComponent<ScoreDisplay>();
            if (sd != null)
                sd.UnregisterPlayer(this);
        }
    }
    
    private void OnScoreChanged(int previousValue, int newValue)
    {
    }

    private void OnCanonLevelChanged(int prev, int next)
    {
        GetComponent<UpgradeVisible>()?.UpdateAttackVisibility(next);
    }

    private void OnMuraillesLevelChanged(int prev, int next)
    {
        GetComponent<UpgradeVisible>()?.UpdateDefenseVisibility(next);
    }

    private void OnMoteurLevelChanged(int prev, int next)
    {
        GetComponent<UpgradeVisible>()?.UpdateSpeedVisibility(next);
    }

    private void OnNetworkMaxLifeChanged(int prev, int next)
    {
        maxLife = next;
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
            life.Value = Mathf.Clamp(value, 0, networkMaxLife.Value);
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
            life.Value = Mathf.Clamp(life.Value + amount, 0, networkMaxLife.Value);

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
            {
                killerManager.score.Value += 10;
            }
        }

        SkinSelector skinSelectorRef = transform.GetChild(0).GetComponent<SkinSelector>();
        skinSelectorRef.setDeadSkin();

        rb = GetComponent<Rigidbody>();
        if (!isFrozen)
        {
            Debug.Log("Player is notfrozen");
            FreezePlayerClientRpc();
        }
     

    }

    IEnumerator FreezeFor10Seconds()
    {
        Debug.Log("Coroutine lancee");
        isFrozen = true;


        // Immobilise l'objet

        ClientAuthoritativeMovement ClientAuthoritativeMovementInstance = GetComponent<ClientAuthoritativeMovement>();
        int TempSpeed = (int) ClientAuthoritativeMovementInstance.Speed;
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
        if (IsServer)
        {
            life.Value = networkMaxLife.Value;

            // Restore normal skin
            SkinSelector skinSelectorRef = transform.GetChild(0).GetComponent<SkinSelector>();
            skinSelectorRef.setNormalSkin();

        }


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

    private void OnPlayerNameChanged(FixedString64Bytes previous, FixedString64Bytes current)
    {
        gameObject.name = current.ToString();

        GameObject scorePanel = GameObject.FindWithTag("ScorePanel");
        if (scorePanel != null)
        {
            ScoreDisplay sd = scorePanel.GetComponent<ScoreDisplay>();
            if (sd != null)
                sd.RefreshUI();
        }
    }

    [ClientRpc]
    private void FreezePlayerClientRpc()
    {
        if (!isFrozen)
            StartCoroutine(FreezeFor10Seconds());
    }

    [ServerRpc]
    private void SetNameServerRpc(string playerName)
    {
        networkPlayerName.Value = new FixedString64Bytes(playerName);
        gameObject.name = playerName;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetLifeServerRpc(int value)
    {
        life.Value = Mathf.Clamp(value, 0, networkMaxLife.Value);
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddLifeServerRpc(int amount, ulong sourceNetworkObjectId = ulong.MaxValue)
    {
        life.Value = Mathf.Clamp(life.Value + amount, 0, networkMaxLife.Value);
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
