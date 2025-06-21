using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowNearMeNew : MonoBehaviour
{
    [Header("Rank Panel (5 total):")]
    public List<TextMeshProUGUI> rankTexts;
    public List<TextMeshProUGUI> nameTexts;
    public List<TextMeshProUGUI> timeTexts;

    private string _filePath;

    private void Awake()
    {
        _filePath = ScoreBoardHelper.GetScoreboardFilePath();
    }

    private void Start()
    {
        ShowMySurroundingScores();
    }

    private void ShowMySurroundingScores()
    {
        List<ScoreBoardHelper.ScoreData> scores = ScoreBoardHelper.LoadScores(_filePath);
        //string currentPlayer = GameManager.Instance.GetCurrentPlayerName();
        //float myScore = GameManager.Instance.totalPlayTime;
        string currentPlayer = "갯";
        float myScore = 99.9f;

        scores.Sort((a, b) => a.score.CompareTo(b.score));
        int myIndex = scores.FindIndex(s => s.playerName == currentPlayer && Mathf.Abs(s.score - myScore) < 0.01f);

        if (myIndex == -1)
        {
            Debug.LogWarning("현재 플레이어 점수를 찾을 수 없습니다. 상위 5명을 표시합니다.");
            ScoreBoardHelper.ShowScores(scores.GetRange(0, Mathf.Min(5, scores.Count)), 0, rankTexts, nameTexts, timeTexts, 5);
            return;
        }

        int total = scores.Count;
        int desiredCount = 5;
        int start = Mathf.Max(0, Mathf.Min(myIndex - 2, total - desiredCount));
        List<ScoreBoardHelper.ScoreData> nearScores = scores.GetRange(start, Mathf.Min(desiredCount, total - start));

        ScoreBoardHelper.ShowScores(nearScores, start, rankTexts, nameTexts, timeTexts, desiredCount);
    }
}
