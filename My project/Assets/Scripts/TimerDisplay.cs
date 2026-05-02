using UnityEngine;
using UnityEngine.UIElements;

public class TimerDisplay : MonoBehaviour
{
    [SerializeField] public UIDocument timerLabel = null;

    private GameStartManager gameStartManager;

    void Start()
    {
        GameObject managerObject = GameObject.FindWithTag("GameManager");
        if (managerObject != null)
            gameStartManager = managerObject.GetComponent<GameStartManager>();
        
        if (gameStartManager == null)
        {
            Debug.LogWarning("[TimerDisplay] GameStartManager not found");
            return;
        }

        gameStartManager.timeRemaining.OnValueChanged += OnTimeChanged;
        UpdateLabel(gameStartManager.timeRemaining.Value);
    }

    void onDestroy()
    {
        if (gameStartManager != null)
            gameStartManager.timeRemaining.OnValueChanged -= OnTimeChanged;
    }

    private void OnTimeChanged(float previousValue, float newValue)
    {
        UpdateLabel(newValue);
    }

    private void UpdateLabel(float seconds)
    {
        if (timerLabel == null) return;

        var root = timerLabel.rootVisualElement;
        
        if (root != null)
        {
            var timerText = root.Q<Label>("TimerLabel");

            if (timerText != null)
            {
                int m = Mathf.FloorToInt(seconds / 60f);
                int s = Mathf.FloorToInt(seconds % 60f);
                timerText.text = $"{m:00}:{s:00}";
            }
        }
    }
}