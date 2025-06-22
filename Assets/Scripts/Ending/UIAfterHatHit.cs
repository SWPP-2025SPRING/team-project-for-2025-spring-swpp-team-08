using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIAfterHatHit : MonoBehaviour
{
    public GameObject endingCreditPanel;
    public EndingCreditScript creditRoll;

    private void Awake()
    {
        endingCreditPanel.SetActive(false);
    }

    public void Show()
    {
        endingCreditPanel.SetActive(true);
        creditRoll.onCreditEnd = OnCreditEnd;
        creditRoll.StartRoll();
    }

    private void OnCreditEnd()
    {
        endingCreditPanel.SetActive(false);

        /*string playerName = GameManager.Instance.GetCurrentPlayerName();
        float score1 = GameManager.Instance.GetScores()[0];
        float score2 = GameManager.Instance.GetScores()[1];
        float score3 = GameManager.Instance.GetScores()[2];
        if (string.IsNullOrEmpty(playerName))
            playerName = "Anonymous";*/

        string playerName = "갯2"; // 소수점 없이 정수 부분만 표시

        float score1 = UnityEngine.Random.Range(10f, 100f);
        float score2 = UnityEngine.Random.Range(10f, 100f);
        float score3 = UnityEngine.Random.Range(10f, 100f);
        SaveScore(playerName, score1, score2, score3);
        Debug.Log("saved22");
        GameManager.Instance.LoadScene("LeaderboardMyRankScene");
    }

    private void SaveScore(string playerName, float score1, float score2, float score3)
    {
        //float finalScore = GameManager.Instance.totalPlayTime;
        float finalScore = 0.9f;
        var scoreboard = FindObjectOfType<ScoreBoardManager>();
        if (scoreboard != null)
            scoreboard.SaveScore(playerName, score1, score2, score3, finalScore);
    }
}
