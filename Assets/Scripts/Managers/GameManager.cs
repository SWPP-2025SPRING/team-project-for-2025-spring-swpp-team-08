using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public PlayManager playManager;
    public float totalPlayTime = 0;

    private AudioSource _bgmSource;
    private AudioSource _sfxSource;

    private string _currentPlayerName;
    private float _score1 = 0;
    private float _score2 = 0;
    private float _score3 = 0;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        /* Set AudioSource */
        var audioSources = GetComponents<AudioSource>();
        _bgmSource = audioSources[0];
        _sfxSource = audioSources[1];
    }

    private void Start()
    {
        Initialize();
        Physics.gravity = new Vector3(0, -30f, 0);
    }

    public void Initialize()
    {
        totalPlayTime = 0;
        SetCurrentPlayerName(null);
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

    /// <summary>
    /// Plays a background music.
    /// </summary>
    public void PlayBgm(AudioClip clip)
    {
        _bgmSource.Stop();
        _bgmSource.clip = clip;
        _bgmSource.Play();
    }

    /// <summary>
    /// Stops playing the background music.
    /// </summary>
    public void StopBgm()
    {
        _bgmSource.Stop();
    }

    /// <summary>
    /// Plays a sound effect.
    /// </summary>
    public void PlaySfx(AudioClip clip)
    {
        _sfxSource.PlayOneShot(clip);
    }

    public void SetCurrentPlayerName(string playerName)
    {
        _currentPlayerName = playerName;
    }

    public void SetScore1(float score)
    {
        _score1 = score;
    }

    public void SetScore2(float score)
    {
        _score2 = score;
    }

    public void SetScore3(float score)
    {
        _score3 = score;
    }

    public string GetCurrentPlayerName()
    {
        return _currentPlayerName;
    }

    public float GetScore1()
    {
        return _score1;
    }

    public float GetScore2()
    {
        return _score2;
    }

    public float GetScore3()
    {
        return _score3;
    }
}
