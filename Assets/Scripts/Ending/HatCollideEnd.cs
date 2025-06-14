using UnityEngine;

public class HatCollideEnd : MonoBehaviour
{
    private bool _triggered = false;

    public InputName inputUIManager;

    private void Start()
    {
        if (inputUIManager == null)
        {
            Debug.LogError("NameInputUIManager가 씬에 없습니다!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_triggered) return; 
        if (other.CompareTag("Player"))
        {
            _triggered = true;
            inputUIManager.Show(OnNameEntered);
        }
    }

    private void OnNameEntered(string playerName)
    {
        //float finalScore = GameManager.Instance.totalPlayTime; 나중에 이거로 바꾸기
        float finalScore = UnityEngine.Random.Range(0f, 100f);

        var scoreboard = FindObjectOfType<ScoreBoardManager>();
        if (scoreboard != null)
        {
            scoreboard.SaveScore(playerName, finalScore);
            Debug.Log("Saved!");
        }
        else
        {
            Debug.LogError("ScoreBoardManager를 씬에서 찾을 수 없습니다!");
        }
    }
}
