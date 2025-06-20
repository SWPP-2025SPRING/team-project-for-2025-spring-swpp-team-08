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

        // 정렬 (오름차순: 시간이 짧을수록 높은 순위)
        scores.Sort((a, b) => a.score.CompareTo(b.score));

        // 현재 플레이어의 인덱스 찾기
        int myIndex = scores.FindIndex(s => s.playerName == currentPlayer && Math.Abs(s.score - myScore) < 0.01f);

        // 못 찾았으면 그냥 처음 5명 보여주기 (예외 처리)
        if (myIndex == -1)
        {
            Debug.LogWarning("현재 플레이어 점수를 찾을 수 없습니다. 상위 5명을 표시합니다.");
            ShowTopN(scores, 0);
            return;
        }

        int start = Mathf.Max(0, myIndex - 2);
        int end = Mathf.Min(scores.Count - 1, myIndex + 2);

        // 최대 5개까지만 보여주기
        List<ScoreData> nearScores = scores.GetRange(start, end - start + 1);
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
