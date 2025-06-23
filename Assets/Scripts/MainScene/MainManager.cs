using System;

using UnityEngine;

namespace MainScene
{
    public class MainManager : MonoBehaviour
    {
        public string startSceneName;
        public string leaderboardSceneName;
        public string[] freeModeSceneNames;

        public SettingsManager settingsManager;

        private void Start()
        {
            Time.timeScale = 1f;
            GameManager.Instance.UnlockCursor();
        }

        public void StartGameStoryMode()
        {
            GameManager.Instance.Initialize();
            GameManager.Instance.LockCursor();
            GameManager.Instance.isStoryMode = true;
            GameManager.Instance.LoadScene(startSceneName);
        }

        public void StartGameFreeMode(int sceneIndex)
        {
            if (sceneIndex < 0 || sceneIndex >= freeModeSceneNames.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            GameManager.Instance.Initialize();
            GameManager.Instance.LockCursor();
            GameManager.Instance.isStoryMode = false;
            GameManager.Instance.LoadScene(freeModeSceneNames[sceneIndex]);
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
