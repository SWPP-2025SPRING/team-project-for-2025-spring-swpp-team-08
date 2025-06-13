using UnityEngine;

public class HatCollideEnd : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Debug.Log("Graduation Hat triggered by Player!");
            // GameManager.Instance.playManager.uiManager.UpdateSubtitle("Congratulations!", 3f);

            string playerName = "dummy";
            float finalScore = GameManager.Instance.totalPlayTime;

            var scoreboard = FindObjectOfType<ScoreBoardManager>();
            if (scoreboard != null)
            {
                scoreboard.SaveScore(playerName, finalScore);
            }
            else
            {
                Debug.LogError("ScoreBoardManager를 씬에서 찾을 수 없습니다!");
            }
        }
    }
}
