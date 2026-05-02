using UnityEngine;
using Unity.Netcode; 
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Multiplayer;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class GameStartManager : NetworkBehaviour
{
    [SerializeField] private string endgameSceneName = "JoinSessionMenu";

    private const float GameDuration = 30f; // 5 minutes

    public NetworkVariable<float> timeRemaining = new NetworkVariable<float>(
        GameDuration, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Server
    );

    private bool gameEnded = false;

    [SerializeField] private string sessionType = "default-session";
    [SerializeField] private int maxRelayConnections = 10;

    private const string k_RelayJoinCodeKey = "RelayJoinCode";

    public bool isDebugMode = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        if (isDebugMode)
        {
            Debug.Log("Starting as host for debugging");
            NetworkManager.Singleton.StartHost();
        }
        else
        {
            // Wait for Unity Service to be fully ready (important for MPPM virtual players tool)
            float timeout = 10f;
            float elapsed = 0f;
            while (RelayService.Instance == null && elapsed < timeout)
            {
                await Task.Yield();
                elapsed += Time.deltaTime;
            }

            if (RelayService.Instance == null)
            {
                Debug.LogError("[GameStartManager] RelayService.Instance is null - Unity Services may not be initialized.");
                return;
            }

            if (NetworkManager.Singleton == null)
            {
                Debug.LogError("[GameStartManager] NetworkManager.Singleton is null - make sure a network manager object is in the scene !");
                return;
            }

            if (MultiplayerService.Instance == null || 
                !MultiplayerService.Instance.Sessions.ContainsKey(sessionType))
            {
                Debug.LogError("[GameStartManager] No active session found ! ");
                // Maybe auto create for debugging
                return;
            }
            
            var session = MultiplayerService.Instance.Sessions[sessionType];

            if (session.IsHost)
                await StartAsHostAsync(session);
            else
                await StartAsClientAsync(session);
        }
    }

    private async Task StartAsHostAsync(ISession session)
    {
        try
        {
            // 1. Clear any stale relay code from a previous run so clients don't pick it up
            var hostSession = session.AsHost();
            hostSession.SetProperty(k_RelayJoinCodeKey, new SessionProperty("", VisibilityPropertyOptions.Member));
            await hostSession.SavePropertiesAsync();
            
            // 2. Allocate relay server
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxRelayConnections);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log($"[GameStartManager] Relay join code: {joinCode}");

            // 3. Configure UTP with relay data and start as Host
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(new RelayServerData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.ConnectionData,
                allocation.ConnectionData,
                allocation.Key,
                isSecure: false));
            NetworkManager.Singleton.OnClientConnectedCallback += id => Debug.Log($"[GameStartManager] Client connected {id}");
            NetworkManager.Singleton.OnClientDisconnectCallback += id => Debug.Log($"[GameStartManager] Client disconnected {id}");
            NetworkManager.Singleton.StartHost();

            Debug.Log("[GameStartManager] Started as NGO Host.");

            // 4. Store join code in session so all clients can ead it
            hostSession.SetProperty(k_RelayJoinCodeKey, new SessionProperty(joinCode, VisibilityPropertyOptions.Member));
            await hostSession.SavePropertiesAsync();
            Debug.Log("[GameStartManager] Key publicated to clients");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[GameStartManager] Failed to start as host: {e.GetType().Name}: {e.Message}\n{e.StackTrace}");
        }
    }

    private async Task StartAsClientAsync(ISession session)
    {
        try
        {
            // 1. Wait for the host to publish the relay join code
            var tcs = new TaskCompletionSource<string>();

            void OnSessionChanged()
            {
                var code = GetRelayCode(session);
                if (code != null)
                    tcs.TrySetResult(code);
            }

            session.Changed += OnSessionChanged;

            var immediate = GetRelayCode(session);
            if (immediate != null)
            {
                tcs.TrySetResult(immediate);
            }

            string joinCode = await tcs.Task;
            session.Changed -= OnSessionChanged;
            
            await Task.Yield();

            Debug.Log($"[GameStartManager] Got relay join code: {joinCode}");

            // 2. Join the relay allocation
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            // 3. Configure UTP with relay data and start as Client
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData( new RelayServerData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData,
                joinAllocation.Key,
                isSecure: false
            ));
            NetworkManager.Singleton.OnClientConnectedCallback += id => Debug.Log($"[GameStartManager] client connected to host with id {id}");
            NetworkManager.Singleton.OnClientDisconnectCallback += id => Debug.Log($"[GameStartManager] client diconnected to host with id {id}");
            NetworkManager.Singleton.StartClient();

            Debug.Log("[GameStartManager] Started as NGO Client.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[GameStartManager] Failed to start as client: {e.GetType().Name}: {e.Message}\n{e.StackTrace}");
        }
    }

    private string GetRelayCode(ISession session)
    {
        if (session.Properties.TryGetValue(k_RelayJoinCodeKey, out var prop) && 
            !string.IsNullOrEmpty(prop.Value))
        {
            return prop.Value;
        }
        return null;
    }

    void Update()
    {
        if (!IsServer || gameEnded) return;

        timeRemaining.Value -= Time.deltaTime;
        if (timeRemaining.Value <= 0f)
        {
            timeRemaining.Value = 0f;
            gameEnded = true;
            EndGame();
        }
    }

    private void EndGame()
    {
        Debug.Log("[GameStartManager] Game over !");
        EndGameClientRpc();
    }

    [ClientRpc]
    private void EndGameClientRpc()
    {
        Debug.Log("[GameStartManager] Game ended !");
        
        ScoreboardData.Clear();
        foreach (var player in FindObjectsByType<PlayerManager>(FindObjectsSortMode.None))
        {
            ScoreboardData.Results.Add(new ScoreboardData.PlayerResult
            {
                PlayerName = player.gameObject.name,
                Score = player.score.Value
            });
        }

        SceneManager.LoadScene(endgameSceneName);
    }
}
