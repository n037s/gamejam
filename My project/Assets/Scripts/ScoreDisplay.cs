using UnityEngine;
using UnityEngine.UIElements;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] public UIDocument timerLabel = null;
    public int score;

    private PlayerManager playerManager;

    public void onPrefabCreated()
    {
        Debug.Log("LEO - prefab registery");
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            PlayerManager playerManagerRef = player.GetComponent<PlayerManager>();
            if (playerManagerRef.isCurrentPlayer)
            {
                playerManager = playerManagerRef;
            }
        }

        if (playerManager != null)
        {
            playerManager.score.OnValueChanged += OnScoreChanged;
            UpdateLabel(playerManager.score.Value);
        }
    }

    void onDestroy()
    {
        if (playerManager != null)
            playerManager.score.OnValueChanged -= OnScoreChanged;
    }

    private void OnScoreChanged(int previousValue, int newValue)
    {
        score = newValue;
        UpdateLabel(newValue);
    }

    private void UpdateLabel(int score)
    {
        if (timerLabel == null) return;

        var root = timerLabel.rootVisualElement;
        
        if (root != null)
        {
            var timerText = root.Q<Label>("ScoreLabel");

            if (timerText != null)
            {
                timerText.text = $"{score}";
            }
        }
    }
}