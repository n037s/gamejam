using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class EndGameMenuManager : MonoBehaviour
{
    private Label m_ScoreboardLabel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        m_ScoreboardLabel = root.Q<Label>("ScoreboardLabel");
        UpdateDisplay();
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
