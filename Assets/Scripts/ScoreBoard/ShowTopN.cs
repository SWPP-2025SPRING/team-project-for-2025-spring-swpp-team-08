using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;

public class ShowTopN : MonoBehaviour
{
    [Header("Top 5 Ranks UI")]
    public List<TextMeshProUGUI> rankTexts;
    public List<TextMeshProUGUI> nameTexts;
    public List<TextMeshProUGUI> timeTexts;

    public int topN = 5;
    private string _filePath;

    private void Awake()
    {
        string dirPath = Path.Combine(Application.persistentDataPath, "ScoreBoard");
        _filePath = Path.Combine(dirPath, "scoreboard.json");
    }

    private void Start()
    {
        ShowTopScores(topN);
    }

    private void ShowTopScores(int count)
    {
        List<ScoreData> scores = LoadScores();
        scores.Sort((a, b) => a.score.CompareTo(b.score));

        for (int i = 0; i < topN; i++)
        {
            string rankText = $"{i + 1}.";
            if (i < scores.Count)
            {
                string name = scores[i].playerName;
                string formattedTime = FormatPlayTime(scores[i].score);

                if (rankTexts.Count > i) rankTexts[i].text = rankText;
                if (nameTexts.Count > i) nameTexts[i].text = name;
                if (timeTexts.Count > i) timeTexts[i].text = formattedTime;
            }
            else
            {
                if (rankTexts.Count > i) rankTexts[i].text = rankText;
                if (nameTexts.Count > i) nameTexts[i].text = "-";
                if (timeTexts.Count > i) timeTexts[i].text = "-";
            }
        }
    }

    private string FormatPlayTime(float playtime)
    {
        return TimeSpan.FromSeconds(playtime).ToString(@"mm\:ss\.ff");
    }

    private List<ScoreData> LoadScores()
    {
        if (!File.Exists(_filePath))
            return new List<ScoreData>();

        string json = File.ReadAllText(_filePath);
        ScoreList scoreList = JsonUtility.FromJson<ScoreList>(json);
        return scoreList?.scores ?? new List<ScoreData>();
    }

    [System.Serializable]
    private class ScoreList
    {
        public List<ScoreData> scores;
    }

    [System.Serializable]
    private class ScoreData
    {
        public string playerName;
        public float score;
    }
}
