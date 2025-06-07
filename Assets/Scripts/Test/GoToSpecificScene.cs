using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToSpecificScene : MonoBehaviour
{
    // 전환할 씬 이름을 인스펙터에서 설정할 수 있도록 함
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
