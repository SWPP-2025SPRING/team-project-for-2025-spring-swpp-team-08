using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

using UnityEngine;

public class ScoreBoardManager : MonoBehaviour
{
    private string _filePath;

    private void Awake()
    {
        string dirPath = Path.Combine(Application.persistentDataPath, "ScoreBoard");
        Directory.CreateDirectory(dirPath);
        _filePath = Path.Combine(dirPath, "scoreboard.json");
        Debug.Log(_filePath);
    }

    public void SaveScore(string name, float score1, float score2, float score3, float score)
    {
        List<ScoreData> scores = LoadScores();
        scores.Add(new ScoreData { playerName = name, stage1Score = score1, stage2Score = score2, stage3Score = score3, score = score });
        scores.Sort((a, b) => a.score.CompareTo(b.score));

        string json = JsonUtility.ToJson(new ScoreList { scores = scores }, true);
        File.WriteAllText(_filePath, json);
        IndexedDBSync.FlushFileSystem();
        Debug.Log(_filePath);
    }

    public List<ScoreData> LoadScores()
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
}
