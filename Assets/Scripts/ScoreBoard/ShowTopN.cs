using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowTopN : MonoBehaviour
{
    [Header("Top 5 Ranks UI")]
    public List<TextMeshProUGUI> rankTexts;
    public List<TextMeshProUGUI> nameTexts;
    public List<TextMeshProUGUI> timeTexts;

    public int topN = 5;
    //private string _filePath;

    private void Start()
    {
        GameManager.Instance.UnlockCursor();
        ShowTopScores(topN);
    }

    private void ShowTopScores(int count)
    {
        List<ScoreBoardHelper.ScoreData> scores = ScoreBoardHelper.LoadScores();
        scores.Sort((a, b) => a.score.CompareTo(b.score));
        ScoreBoardHelper.ShowScores(scores.GetRange(0, Mathf.Min(count, scores.Count)), 0, rankTexts, nameTexts, timeTexts, count);
    }
}
