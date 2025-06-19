using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class ToMain : MonoBehaviour
{
    public string sceneName = "MainScene";

    public void ResetGame()
    {
        string targetScene = string.IsNullOrEmpty(sceneName)
            ? SceneManager.GetActiveScene().name
            : sceneName;

        SceneManager.LoadScene(targetScene);
    }
}
