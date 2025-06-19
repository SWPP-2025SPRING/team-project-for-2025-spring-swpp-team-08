using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;

public class ShowTopN : MonoBehaviour
{
    public List<TextMeshProUGUI> topScoreTexts; 
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
        scores.Sort((a, b) => a.score.CompareTo(b.score)); // 작은 시간이 상위

        for (int i = 0; i < topScoreTexts.Count; i++)
        {
            if (i < scores.Count)
            {
                string formattedTime = FormatPlayTime(scores[i].score);
                topScoreTexts[i].text = $"{i + 1}. {scores[i].playerName}: {formattedTime}";
            }
            else
            {
                topScoreTexts[i].text = $"{i + 1}. -";
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
