using System;
using System.Collections;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Play UI")]
    public GameObject playUI;
    public Animator playUIAnimator;

    [Space(10)]
    public TextMeshProUGUI playTimeText;
    public TextMeshProUGUI currentPlayTimeText;
    public TextMeshProUGUI currentStageText;
    public TextMeshProUGUI subtitleText;
    public TextMeshProUGUI percentText;
    public Slider progressBar;
    public TMP_FontAsset stateSubtitleFont;
    public TMP_FontAsset storySubtitleFont;
    public TMP_FontAsset playerLineSubtitleFont;

    [Header("Countdown UI")]
    public GameObject countdownUI;
    public Animator countdownUIAnimator;
    public TextMeshProUGUI countdownText;

    [Header("Result UI")]
    public GameObject resultUI;
    public Animator resultUIAnimator;
    public TextMeshProUGUI resultStageText;
    public TextMeshProUGUI resultTimeCurrentText;
    public TextMeshProUGUI resultTimeTotalText;
    public TextMeshProUGUI resultRankingText;

    [Header("Pause UI")]
    public GameObject pauseUI;

    private Coroutine _subtitleCoroutine;

    private void Start()
    {
        subtitleText.gameObject.SetActive(false);
    }

    private string FormatPlayTime(float playtime)
    {
        return TimeSpan.FromSeconds(playtime).ToString(@"m\:ss\.fff");
    }

    public void UpdatePlayTime(float playTime)
    {
        playTimeText.text = FormatPlayTime(playTime);
    }

    public void UpdateCurrentPlayTime(float playTime)
    {
        currentPlayTimeText.text = FormatPlayTime(playTime);
    }

    public void UpdateStage(string stageName)
    {
        currentStageText.text = stageName;
        resultStageText.text = stageName;
    }

    public void UpdateStateSubtitle(string subtitle, float totalDurationSeconds)
    {
        UpdateSubtitle(subtitle, totalDurationSeconds, Color.red, stateSubtitleFont);
    }

    public void UpdateStorySubtitle(string subtitle, float totalDurationSeconds)
    {
        UpdateSubtitle(subtitle, totalDurationSeconds, Color.black, storySubtitleFont);
    }

    public void UpdatePlayerLineSubtitle(string subtitle, float totalDurationSeconds)
    {
        UpdateSubtitle(subtitle, totalDurationSeconds, Color.black, playerLineSubtitleFont);
    }

    private void UpdateSubtitle(string subtitle, float totalDurationSeconds, Color color, TMP_FontAsset font)
    {
        if (_subtitleCoroutine != null)
        {
            StopCoroutine(_subtitleCoroutine);
        }

        subtitleText.font = font;
        subtitleText.text = subtitle;
        subtitleText.color = color;
        subtitleText.gameObject.SetActive(true);
        _subtitleCoroutine = StartCoroutine(HideSubtitleAfterDelay(totalDurationSeconds));
    }

    public void SetProgress(float progress)
    {
        float percent = progress * 100;
        percentText.text = percent.ToString("F1");
    }

    public void SetProgressBar(float progress)
    {
        progressBar.value = progress;
    }

    private IEnumerator HideSubtitleAfterDelay(float totalDurationSeconds)
    {
        yield return new WaitForSeconds(totalDurationSeconds);
        subtitleText.gameObject.SetActive(false);
    }

    public void HideAllUIs()
    {
        playTimeText.gameObject.SetActive(false);
        currentPlayTimeText.gameObject.SetActive(false);
        currentStageText.gameObject.SetActive(false);
        subtitleText.gameObject.SetActive(false);
    }

    public void ShowPlayUI()
    {
        playUIAnimator.SetBool("IsShown", true);
    }

    public void HidePlayUI()
    {
        playUIAnimator.SetBool("IsShown", false);
    }

    public void ShowResultUI(float playTimeCurrent, float playTimeTotal, int resultRanking)
    {
        resultTimeCurrentText.text = FormatPlayTime(playTimeCurrent);
        resultTimeTotalText.text = FormatPlayTime(playTimeTotal);
        // TODO: Show result ranking

        resultUI.SetActive(true);
    }

    public void ShowCountdownText(string text, float duration)
    {
        countdownText.text = text;
        StartCoroutine(CountdownCoroutine());
        return;

        IEnumerator CountdownCoroutine()
        {
            countdownUIAnimator.SetBool("IsShown", true);
            yield return new WaitForSeconds(duration);
            countdownUIAnimator.SetBool("IsShown", false);
        }
    }

    public void HideCountdownUI()
    {
        countdownUI.SetActive(false);
    }

    public void ShowPauseUI()
    {
        pauseUI.SetActive(true);
    }

    public void HidePauseUI()
    {
        pauseUI.SetActive(false);
    }

    //set stage text at each scene UI
    //stage text:  "DM", "DS", "EEC", "SWPP"
    //thus no need for Updatestage as it will be set in scene
}
