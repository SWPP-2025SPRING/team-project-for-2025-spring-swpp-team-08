using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public void ResumeGame()
    {
        GameManager.Instance.playManager.ResumeGame();
    }

    public void RetryStage()
    {
        GameManager.Instance.playManager.RetryStage();
    }

    public void QuitToMain()
    {
        GameManager.Instance.playManager.QuitToMain();
    }
}
