using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public PlayManager playManager;
    public float totalPlayTime = 0;

    private AudioSource _bgmSource;
    private AudioSource _sfxSource;

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
    /// <summary>
/// Plays a sound effect at default volume.
/// </summary>
public void PlaySfx(AudioClip clip)
{
    _sfxSource.PlayOneShot(clip);
}

/// <summary>
/// Plays a sound effect at custom volume.
/// </summary>

/// <summary>
/// Plays a sound effect at custom volume, temporarily pausing the BGM.
/// </summary>
public void PlaySfxWithBgmPause(AudioClip clip, float volumeScale = 1f, float resumeDelay = 2f)
{
    if (_bgmSource.isPlaying)
    {
        _bgmSource.Pause(); // Pause the BGM
        Invoke(nameof(ResumeBgm), resumeDelay);
    }

    _sfxSource.PlayOneShot(clip, volumeScale);
}

private void ResumeBgm()
{
    if (_bgmSource.clip != null)
        _bgmSource.UnPause();
}


}
