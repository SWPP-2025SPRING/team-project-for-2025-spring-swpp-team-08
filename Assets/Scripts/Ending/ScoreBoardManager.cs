using System.Collections.Generic;
using System.IO;

using UnityEngine;

public class ScoreBoardManager : MonoBehaviour
{
    private string filePath;

    private void Awake()
    {
        filePath = Path.Combine(Application.dataPath, "ScoreBoard", "scoreboard.json");
        Debug.Log(filePath);
    }

    public void SaveScore(string name, float score)
    {
        List<ScoreData> scores = LoadScores();
        scores.Add(new ScoreData { playerName = name, score = score });
        scores.Sort((a, b) => a.score.CompareTo(b.score)); 

        string json = JsonUtility.ToJson(new ScoreList { scores = scores }, true);
        File.WriteAllText(filePath, json);
        Debug.Log(filePath);
    }

    public List<ScoreData> LoadScores()
    {
        if (!File.Exists(filePath))
            return new List<ScoreData>();

        string json = File.ReadAllText(filePath);
        ScoreList scoreList = JsonUtility.FromJson<ScoreList>(json);
        return scoreList?.scores ?? new List<ScoreData>();
    }

    [System.Serializable]
    private class ScoreList
    {
        public List<ScoreData> scores;
    }
}
