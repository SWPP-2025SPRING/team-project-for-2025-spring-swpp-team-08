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
        if (string.IsNullOrEmpty(playerName))
            playerName = "Anonymous";

        SaveScore(playerName);
        GameManager.Instance.LoadScene("LeaderboardScene");
    }

    private void SaveScore(string playerName)
    {
        float finalScore = GameManager.Instance.totalPlayTime;

        var scoreboard = FindObjectOfType<ScoreBoardManager>();
        if (scoreboard != null)
            scoreboard.SaveScore(playerName, finalScore);
    }
}
