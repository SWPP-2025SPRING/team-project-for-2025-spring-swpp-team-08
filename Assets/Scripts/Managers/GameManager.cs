using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public PlayManager playManager;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Loads a specified scene.
    /// If not specified, reload current scene.
    /// </summary>
    /// <param name="sceneName">Scene name string</param>
    public static void LoadScene(string sceneName = null)
    {
        sceneName ??= SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);
    }

    void Start()
    {
        Physics.gravity = new Vector3(0, -30f, 0);
    }
}
