using UnityEngine;

namespace MainScene
{
    public class MainManager : MonoBehaviour
    {
        public string startSceneName;
        public string leaderboardSceneName;

        public void StartGame()
        {
            GameManager.Instance.Initialize();
            GameManager.LoadScene(startSceneName);
        }

        public void LoadLeaderboardScene()
        {
            GameManager.LoadScene(leaderboardSceneName);
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
