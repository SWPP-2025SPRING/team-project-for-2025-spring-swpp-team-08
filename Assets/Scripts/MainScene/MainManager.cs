using UnityEngine;

namespace MainScene
{
    public class MainManager : MonoBehaviour
    {
        public string startSceneName;
        public string leaderboardSceneName;

        public SettingsManager settingsManager;

        public void StartGame()
        {
            GameManager.Instance.Initialize();
            GameManager.Instance.LockCursor();
            GameManager.Instance.LoadScene(startSceneName);
        }

        public void ShowSettings()
        {
            settingsManager.OpenSettingsPanel();
        }

        public void LoadLeaderboardScene()
        {
            GameManager.Instance.LoadScene(leaderboardSceneName);
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
