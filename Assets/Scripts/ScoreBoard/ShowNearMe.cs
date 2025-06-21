using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;

public class ShowNearMe : MonoBehaviour
{
    [Header("Rank Panel (5 total):")]
    public List<TextMeshProUGUI> rankTexts;
    public List<TextMeshProUGUI> nameTexts;
    public List<TextMeshProUGUI> timeTexts;

    private string _filePath;

    private void Awake()
    {
        string dirPath = Path.Combine(Application.persistentDataPath, "ScoreBoard");
        _filePath = Path.Combine(dirPath, "scoreboard.json");
    }

    private void Start()
    {
        ShowMySurroundingScores();
    }

    private void ShowMySurroundingScores()
    {
        List<ScoreData> scores = LoadScores();
        string currentPlayer = GameManager.Instance.GetCurrentPlayerName();
        float myScore = GameManager.Instance.totalPlayTime;
        //string currentPlayer = "갯";
        //float myScore = 99.9f;

        scores.Sort((a, b) => a.score.CompareTo(b.score));

        int myIndex = scores.FindIndex(s => s.playerName == currentPlayer && Mathf.Abs(s.score - myScore) < 0.01f);

        if (myIndex == -1)
        {
            Debug.LogWarning("현재 플레이어 점수를 찾을 수 없습니다. 상위 5명을 표시합니다.");
            ShowTopN(scores, 0);
            return;
        }

        int total = scores.Count;
        int desiredCount = 5;
        int start = myIndex - 2;
        if (start < 0)
            start = 0;
        if (start + desiredCount > total)
            start = Mathf.Max(0, total - desiredCount);

        List<ScoreData> nearScores = scores.GetRange(start, Mathf.Min(desiredCount, total - start));
        ShowScores(nearScores, start);
    }

    private void ShowScores(List<ScoreData> scoreList, int startRankIndex)
    {
        for (int i = 0; i < 5; i++)
        {
            string rankStr = $"{startRankIndex + i + 1}.";

            if (i < scoreList.Count)
            {
                var s = scoreList[i];
                rankTexts[i].text = rankStr;
                nameTexts[i].text = s.playerName;
                timeTexts[i].text = FormatPlayTime(s.score);
            }
            else
            {
                rankTexts[i].text = rankStr;
                nameTexts[i].text = "-";
                timeTexts[i].text = "-";
            }
        }
    }

    private void ShowTopN(List<ScoreData> scores, int startIndex)
    {
        List<ScoreData> topScores = scores.GetRange(startIndex, Mathf.Min(5, scores.Count));
        ShowScores(topScores, startIndex);
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
