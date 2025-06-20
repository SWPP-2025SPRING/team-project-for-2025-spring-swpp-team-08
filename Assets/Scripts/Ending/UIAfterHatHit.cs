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
        /*float score1 = GameManager.Instance.GetScore1();
        float score2 = GameManager.Instance.GetScore2();
        float score3 = GameManager.Instance.GetScore3();
        if (string.IsNullOrEmpty(playerName))
            playerName = "Anonymous";
        */
        float score1 = UnityEngine.Random.Range(10f, 100f); 
        float score2 = UnityEngine.Random.Range(10f, 100f);
        float score3 = UnityEngine.Random.Range(10f, 100f);
        SaveScore(playerName, score1, score2, score3);

        GameManager.LoadScene("LeaderboardMyRankScene");
    }

    private void SaveScore(string playerName, float score1, float score2, float score3)
    {
        float finalScore = GameManager.Instance.totalPlayTime;

        var scoreboard = FindObjectOfType<ScoreBoardManager>();
        if (scoreboard != null)
            scoreboard.SaveScore(playerName, score1, score2, score3, finalScore);
    }
}
