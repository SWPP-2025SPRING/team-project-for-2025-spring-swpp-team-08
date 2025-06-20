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

        string playerName = GameManager.Instance.GetCurrentPlayerName();
        float score1 = GameManager.Instance.GetScore1();
        float score2 = GameManager.Instance.GetScore2();
        float score3 = GameManager.Instance.GetScore3();
        if (string.IsNullOrEmpty(playerName))
            playerName = "Anonymous";

        SaveScore(playerName, score1, score2, score3);

        GameManager.LoadScene("LeaderboardScene");
    }

    private void SaveScore(string playerName, float score1, float score2, float score3)
    {
        float finalScore = GameManager.Instance.totalPlayTime;

        var scoreboard = FindObjectOfType<ScoreBoardManager>();
        if (scoreboard != null)
            scoreboard.SaveScore(playerName, score1, score2, score3, finalScore);
    }
}
