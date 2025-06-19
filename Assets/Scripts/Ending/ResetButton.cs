using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetButton : MonoBehaviour
{
    public string sceneName;

    public void ResetGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.Initialize();
        }

        string targetScene = string.IsNullOrEmpty(sceneName)
            ? SceneManager.GetActiveScene().name
            : sceneName;

        GameManager.LoadScene(targetScene);
    }
}
