using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] public UIDocument timerLabel = null;
    
    private PlayerManager _currentPlayer;
    private readonly List<PlayerManager> _allPlayers = new List<PlayerManager>();

    private Label _myScoreLabel; 
    private Label _scoreboardLabel;

    public void RegisterPlayer(PlayerManager pm)
    {
        if (pm == null || _allPlayers.Contains(pm)) return;

        _allPlayers.Add(pm);
        pm.score.OnValueChanged += OnAnyScoreChanged;
        if (pm.isCurrentPlayer)
            _currentPlayer = pm;
        RefreshUI();
    }

    public void UnregisterPlayer(PlayerManager pm)
    {
        if (pm == null) return;
        pm.score.OnValueChanged -= OnAnyScoreChanged;
        _allPlayers.Remove(pm);
        if (pm == _currentPlayer)
            _currentPlayer = null;
        RefreshUI();
    }

    public void onPrefabCreated()
    {
        foreach (var pm in _allPlayers)
            if (pm != null)
                pm.score.OnValueChanged -= OnAnyScoreChanged;
        _allPlayers.Clear();
        _currentPlayer = null;

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            PlayerManager pm = player.GetComponent<PlayerManager>();
            if (pm == null) continue;
            _allPlayers.Add(pm);
            pm.score.OnValueChanged += OnAnyScoreChanged;
            if (pm.isCurrentPlayer)
                _currentPlayer = pm;
        }

        RefreshUI();
    }

    void OnDestroy()
    {
        foreach (var pm in _allPlayers)
            if (pm != null)
                pm.score.OnValueChanged -= OnAnyScoreChanged;
    }

    private void OnAnyScoreChanged(int previousValue, int newValue)
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (timerLabel == null) return;

        if (_myScoreLabel == null || _scoreboardLabel == null)
        {
            var root = timerLabel.rootVisualElement;
            _myScoreLabel = root.Q<Label>("MyScoreLabel");
            _scoreboardLabel = root.Q<Label>("ScoreboardLabel");
        }

        int myScore = _currentPlayer != null ? _currentPlayer.score.Value : 0;
        if (_myScoreLabel != null)
            _myScoreLabel.text = $"Score: {myScore}";

        if (_scoreboardLabel == null) return;

        var sorted = new List<PlayerManager>(_allPlayers);
        sorted.RemoveAll(p => p == null);
        sorted.Sort((a, b) => b.score.Value.CompareTo(a.score.Value));

        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < sorted.Count; i++)
        {
            PlayerManager pm = sorted[i];
            string line = $"{i+1}. {pm.networkPlayerName.Value} - {pm.score.Value}";
            if (pm == _currentPlayer)
                line = $"<b>{line}</b>";
            sb.AppendLine(line);
        }

        _scoreboardLabel.text = sb.ToString().TrimEnd();
    }
}