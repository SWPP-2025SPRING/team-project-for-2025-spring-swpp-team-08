using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private const float BgmVolumeMultiplier = 0.4f;
    private const float MouseSensitivityMultiplier = 6f;

    public static GameManager Instance { get; private set; }

    [HideInInspector] public PlayManager playManager;

    [Header("Values")]
    public float totalPlayTime;
    public bool isStoryMode;

    [Header("Settings")]
    public float bgmVolume;
    public float mouseSensitivity;

    [Header("Transition")]
    [SerializeField] private GameObject transitionCanvas;
    [SerializeField] private float transitionDurationSeconds;

    private AudioSource _bgmSource;
    private AudioSource _sfxSource;

    private string _currentPlayerName;
    private float[] _scores = new float[3];

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
        Physics.gravity = new Vector3(0, -30f, 0);
        SetBgmVolume(bgmVolume);
        SetMouseSensitivity(mouseSensitivity);
        Initialize();

        StartCoroutine(InitialTransitionCoroutine());
        return;

        IEnumerator InitialTransitionCoroutine()
        {
            var transition = Instantiate(Instance.transitionCanvas);
            var transitionBehaviour = transition.GetComponent<TransitionBehaviour>();

            transitionBehaviour.EndTransition(transitionDurationSeconds);
            yield return new WaitWhile(() => transitionBehaviour.isTransitioning);

            Destroy(transition);
        }
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
    public void LoadScene(string sceneName = null)
    {
        sceneName ??= SceneManager.GetActiveScene().name;

        StartCoroutine(TransitionCoroutine(sceneName));
    }

    public IEnumerator InitiateTransition()
    {
        StartCoroutine(TransitionCoroutine(null));
        yield return new WaitForSeconds(transitionDurationSeconds);
    }

    private IEnumerator TransitionCoroutine(string nextSceneName)
    {
        var transition = Instantiate(Instance.transitionCanvas);
        var transitionBehaviour = transition.GetComponent<TransitionBehaviour>();

        DontDestroyOnLoad(transition);

        transitionBehaviour.StartTransition(transitionDurationSeconds);
        yield return new WaitWhile(() => transitionBehaviour.isTransitioning);

        if (nextSceneName != null)
        {
            SceneManager.LoadScene(nextSceneName);
        }

        transitionBehaviour.EndTransition(transitionDurationSeconds);
        yield return new WaitWhile(() => transitionBehaviour.isTransitioning);

        Destroy(transition);
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
    /// Plays a sound effect at default volume.
    /// </summary>
    public void PlaySfx(AudioClip clip)
    {
        _sfxSource.PlayOneShot(clip);
    }
    public void PlaySfx(AudioClip clip, float volumeScale = 1f)
    {
        _sfxSource.PlayOneShot(clip, volumeScale);
    }
    public void SetBgmLoop(bool shouldLoop)
    {
        _bgmSource.loop = shouldLoop;
    }

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

    public void SetCurrentPlayerName(string playerName)
    {
        _currentPlayerName = playerName;
    }

    public void SetScore(float score, int x)
    {
        if (x <= 0) return;
        _scores[x] = score;
    }

    public string GetCurrentPlayerName()
    {
        return _currentPlayerName;
    }

    public float[] GetScores()
    {
        return _scores;
    }

    private void ResumeBgm()
    {
        if (_bgmSource.clip != null)
            _bgmSource.UnPause();
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    public void SetBgmVolume(float volume)
    {
        bgmVolume = volume;
        _bgmSource.volume = bgmVolume * BgmVolumeMultiplier;
    }

    public void SetMouseSensitivity(float sensitivity)
    {
        mouseSensitivity = sensitivity;
        ApplyMouseSensitivity();
    }

    public void ApplyMouseSensitivity()
    {
        playManager?.SetMouseSensitivity(mouseSensitivity * MouseSensitivityMultiplier);
    }
}
