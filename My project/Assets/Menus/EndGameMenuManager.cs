using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using Unity.Services.Multiplayer;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class EndGameMenuManager : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "NGO_Setup";
    [SerializeField] private string sessionType = "default-session";

    private const string k_GameStateKey = "GameState";
    private const string k_GameStateRestarting = "Restarting";

    private Label m_ScoreboardLabel;
    private Button m_RestartButton;
    private ISession m_Session;
    private volatile bool m_ShouldRestart = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        m_ScoreboardLabel = root.Q<Label>("ScoreboardLabel");
        m_RestartButton = root.Q<Button>("StartButton");

        UpdateDisplay();
        _ = InitSessionAsync();
    }

    void OnDisable()
    {
        if (m_RestartButton != null)
            m_RestartButton.clicked -= OnRestartClicked;
        
        if (m_Session != null)
        {
            m_Session.Changed -= OnSessionChanged;
            m_Session = null;
        }
    }

    void Update()
    {
        if (m_ShouldRestart)
        {
            m_ShouldRestart = false;
            StartCoroutine(RestartCoroutine());
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

        if (m_RestartButton != null)
        {
            m_RestartButton.style.display = m_Session.IsHost ? DisplayStyle.Flex : DisplayStyle.None;
            m_RestartButton.clicked += OnRestartClicked;
        }
    }

    private async void OnRestartClicked()
    {
        if (m_Session == null || !m_Session.IsHost) return;

        try
        {
            var hostSession = m_Session.AsHost();
            hostSession.SetProperty(k_GameStateKey, new SessionProperty(k_GameStateRestarting, VisibilityPropertyOptions.Member));
            await hostSession.SavePropertiesAsync();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[EndGameManager] Failed to set restart property {e.Message}");
            return;
        }

        m_ShouldRestart = true;
    }

    private void OnSessionChanged()
    {
        if (m_Session == null) return;

        if (m_Session.Properties.TryGetValue(k_GameStateKey, out var prop) && 
            prop.Value == k_GameStateRestarting)
        {
            m_ShouldRestart = true;
        }
    }

    private IEnumerator RestartCoroutine()
    {
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
            NetworkManager.Singleton.Shutdown();
        
        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(gameSceneName);
    }

    public void UpdateDisplay()
    {
        if (m_ScoreboardLabel == null) return;

        if (!ScoreboardData.HasResults)
        {
            m_ScoreboardLabel.text = "No results";
            return;
        }

        // Sort result by descending score
        var sorted = new List<ScoreboardData.PlayerResult>(ScoreboardData.Results);
        sorted.Sort((a, b) => b.Score.CompareTo(a.Score));

        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < sorted.Count; i++)
        {
            sb.AppendLine($"{i+1} - {sorted[i].PlayerName} - {sorted[i].Score}");
        }

        m_ScoreboardLabel.text = sb.ToString().TrimEnd();
    }
}
