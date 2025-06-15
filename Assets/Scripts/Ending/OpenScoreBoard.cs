using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro; 

public class OpenScoreBoard : MonoBehaviour
{
    public TextMeshProUGUI topScoresTMP;
    public int topN = 3;
    public GameObject scoreButton; 
    private string _filePath;

    private void Awake()
    {
        string dirPath = Path.Combine(Application.persistentDataPath, "ScoreBoard");
        _filePath = Path.Combine(dirPath, "scoreboard.json");
    }

    private void Start()
    {
        if (topScoresTMP != null)
        {
            topScoresTMP.gameObject.SetActive(false);
        }
    }

    public void OnScoreButtonClicked()
    {
        ShowTopScores(topN);
        if (scoreButton != null)
        {
            scoreButton.SetActive(false); 
        }
    }


    private void ShowTopScores(int count)
    {
        List<ScoreData> scores = LoadScores();
        scores.Sort((a, b) => b.score.CompareTo(a.score));

        int displayCount = Mathf.Min(count, scores.Count);
        string displayText = "";

        for (int i = 0; i < displayCount; i++)
        {
            displayText += $"{i + 1}. {scores[i].playerName}: {scores[i].score:F2}\n";
        }

        if (topScoresTMP != null)
        {
            topScoresTMP.text = displayText;
            topScoresTMP.gameObject.SetActive(true); 
        }
        else
        {
            Debug.Log(displayText); 
        }
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
