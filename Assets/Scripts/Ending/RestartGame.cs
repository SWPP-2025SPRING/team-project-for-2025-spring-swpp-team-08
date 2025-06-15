using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour
{
    public string firstSceneName = "SampleScene";

    public void LoadFirstScene()
    {
        SceneManager.LoadScene(firstSceneName);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == firstSceneName && GameManager.Instance != null)
        {
            GameManager.Instance.Initialize();
            Debug.Log("GameManager 초기화 완료");
        }
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
