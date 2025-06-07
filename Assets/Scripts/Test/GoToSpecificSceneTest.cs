using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToSpecificSceneTest : MonoBehaviour
{

    public string sceneName;

    public void LoadSceneByName()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            GameManager.Instance.playManager.LoadNextStage();
        }
        else
        {
            Debug.LogError("씬 이름이 비어 있습니다!");
        }
    }
}
