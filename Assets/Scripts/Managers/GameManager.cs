using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [HideInInspector] public PlayManager playManager;

    [Header("Values")]
    public float totalPlayTime = 0;

    [Header("Transition")]
    [SerializeField] private GameObject transitionCanvas;
    [SerializeField] private float transitionDurationSeconds;

    private AudioSource _bgmSource;
    private AudioSource _sfxSource;

    private string _currentPlayerName;

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
    public void LoadScene(string sceneName = null)
    {
        sceneName ??= SceneManager.GetActiveScene().name;

        StartCoroutine(TransitionCoroutine());
        return;

        IEnumerator TransitionCoroutine()
        {
            var transition = Instantiate(Instance.transitionCanvas);
            var transitionImage = transition.GetComponentInChildren<Image>();
            var elapsedTime = 0f;

            DontDestroyOnLoad(transition);

            while (elapsedTime < transitionDurationSeconds)
            {
                var progress = Mathf.Lerp(0f, 1f, elapsedTime / transitionDurationSeconds);
                transitionImage.color = new Color(0f, 0f, 0f, progress);
                _bgmSource.volume = 1f - progress;

                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }

            transitionImage.color = Color.black;
            _bgmSource.volume = 0f;

            SceneManager.LoadScene(sceneName);
            elapsedTime = 0f;

            while (elapsedTime < transitionDurationSeconds)
            {
                var progress = Mathf.Lerp(0f, 1f, elapsedTime / transitionDurationSeconds);
                transitionImage.color = new Color(0f, 0f, 0f, 1f - progress);
                _bgmSource.volume = progress;

                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }

            _bgmSource.volume = 1f;
            Destroy(transition);
        }
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

    public string GetCurrentPlayerName()
    {
        return _currentPlayerName;
    }
}
