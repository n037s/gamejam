using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Unity.Services.Multiplayer;
using System.Threading.Tasks; 

namespace menu
{
    [RequireComponent(typeof(UIDocument))]
    public class SessionMenuController : MonoBehaviour
    {
        [SerializeField] private string gameSceneName = "NGO_Setup";
        [SerializeField] private string sessionType = "default-session";

        private const string k_GameStateKey = "GameState";
        private const string k_GameStateStaring = "Starting";

        private Button m_StartButton;
        private ISession m_Session;

        void OnEnable()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            m_StartButton = root.Q<Button>("StartButton");
            if (m_StartButton != null)
            {
                m_StartButton.clicked += OnStartClicked;
            }

            _ = InitSessionAsync();
        }

        void OnDisable()
        {
            if (m_StartButton != null)
            {
                m_StartButton.clicked -= OnStartClicked;
            }

            if (m_Session != null)
            {
                m_Session.Changed -= OnSessionChanged;
                m_Session = null;
            }
        }

        private async Task InitSessionAsync()
        {
            while (MultiplayerService.Instance == null ||
                    !MultiplayerService.Instance.Sessions.ContainsKey(sessionType))
            {
                await Task.Yield();
            }

            m_Session = MultiplayerService.Instance.Sessions[sessionType];
            m_Session.Changed += OnSessionChanged;

            UpdateStartButtonVisibility();
        }

        private void UpdateStartButtonVisibility()
        {
            if (m_StartButton == null || m_Session == null) return;

            bool isHost = m_Session.IsHost;
            m_StartButton.style.display = isHost ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private async void OnStartClicked()
        {
            if (m_Session == null || !m_Session.IsHost)
            {
                Debug.LogWarning("Only the host can start the game");
                return; 
            }

            // Send Start to all clients
            try
            {
                var hostSession = m_Session.AsHost();
                hostSession.SetProperty(k_GameStateKey, new SessionProperty(k_GameStateStaring, VisibilityPropertyOptions.Member));
                await hostSession.SavePropertiesAsync();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to set a session property: {e.Message}");
                return;
            }

            LoadGameScene();
        }

        private volatile bool m_ShouldLoadScene = false;

        void Update()
        {
            if (m_ShouldLoadScene)
            {
                m_ShouldLoadScene = false;
                LoadGameScene();
            }
        }

        private void OnSessionChanged()
        {
            var session = m_Session;
            if (session == null) return;

            if (session.Properties.TryGetValue(k_GameStateKey, out var prop) && 
                prop.Value == k_GameStateStaring)
            {
                m_ShouldLoadScene = true;
            }
        }

        public void LoadGameScene()
        {

            // Loads the scene of the game
            SceneManager.LoadScene(gameSceneName);
        }
    }
}