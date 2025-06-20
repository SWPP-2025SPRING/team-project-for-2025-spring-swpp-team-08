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


    [Header("Result UI")]
    public GameObject resultUI;
    public Animator resultUIAnimator;

    private Coroutine _coroutine;

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
    }

    public void UpdateSubtitle(string subtitle, float totalDurationSeconds)
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
        subtitleText.text = subtitle;
        subtitleText.gameObject.SetActive(true);
        _coroutine = StartCoroutine(HideSubtitleAfterDelay(totalDurationSeconds));
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

    public void ShowPlayUI()
    {
        playUIAnimator.SetBool("IsShown", true);
    }

    public void HidePlayUI()
    {
        playUIAnimator.SetBool("IsShown", false);
    }

    public void ShowResultUI()
    {
        resultUI.SetActive(true);
    }

    public void StartCountdown()
    {
        // TODO
    }

    //set stage text at each scene UI
    //stage text:  "DM", "DS", "EEC", "SWPP"
    //thus no need for Updatestage as it will be set in scene

}
