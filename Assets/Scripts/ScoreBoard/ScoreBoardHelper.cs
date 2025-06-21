using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;

public static class ScoreBoardHelper
{
    [Serializable]
    public class ScoreData
    {
        public string playerName;
        public float score;
    }

    [Serializable]
    private class ScoreList
    {
        public List<ScoreData> scores;
    }

    public static string GetScoreboardFilePath()
    {
        string dirPath = Path.Combine(Application.persistentDataPath, "ScoreBoard");
        return Path.Combine(dirPath, "scoreboard.json");
    }

    public static List<ScoreData> LoadScores()
    {
        filePath = GetScoreboardFilePath();
        if (!File.Exists(filePath))
            return new List<ScoreData>();

        string json = File.ReadAllText(filePath);
        ScoreList scoreList = JsonUtility.FromJson<ScoreList>(json);
        return scoreList?.scores ?? new List<ScoreData>();
    }

    public static void ShowScores(
        List<ScoreData> scores,
        int startRankIndex,
        List<TextMeshProUGUI> rankTexts,
        List<TextMeshProUGUI> nameTexts,
        List<TextMeshProUGUI> timeTexts,
        int maxCount
    )
    {
        for (int i = 0; i < maxCount; i++)
        {
            string rankStr = $"{startRankIndex + i + 1}.";

            if (i < scores.Count)
            {
                var s = scores[i];
                if (rankTexts.Count > i) rankTexts[i].text = rankStr;
                if (nameTexts.Count > i) nameTexts[i].text = s.playerName;
                if (timeTexts.Count > i) timeTexts[i].text = FormatPlayTime(s.score);
            }
            else
            {
                if (rankTexts.Count > i) rankTexts[i].text = rankStr;
                if (nameTexts.Count > i) nameTexts[i].text = "-";
                if (timeTexts.Count > i) timeTexts[i].text = "-";
            }
        }
    }

    public static string FormatPlayTime(float playtime)
    {
        return TimeSpan.FromSeconds(playtime).ToString(@"mm\:ss\.ff");
    }
}
