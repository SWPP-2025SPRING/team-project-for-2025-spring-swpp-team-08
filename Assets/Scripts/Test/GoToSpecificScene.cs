using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToSpecificScene : MonoBehaviour
{

    public string sceneName;

    public void LoadSceneByName()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("씬 이름이 비어 있습니다!");
        }
    }
}
