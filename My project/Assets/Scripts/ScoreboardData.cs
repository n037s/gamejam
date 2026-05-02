using System.Collections.Generic;

public static class ScoreboardData
{
    public struct PlayerResult
    {
        public string PlayerName;
        public int Score;
    }

    public static List<PlayerResult> Results = new();

    public static bool HasResults => Results.Count > 0;

    public static void Clear() => Results.Clear();
}