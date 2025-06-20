using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI playTimeText;
    public TextMeshProUGUI currentPlayTimeText;
    public TextMeshProUGUI currentStageText;
    public TextMeshProUGUI subtitleText;
    public TextMeshProUGUI percentText;
    public Slider progressBar;
    public TMP_FontAsset stateSubtitleFont;
    public TMP_FontAsset storySubtitleFont;
    public TMP_FontAsset playerLineSubtitleFont;

    private Coroutine _subtitleCoroutine;

    private void Start()
    {
        subtitleText.gameObject.SetActive(false);
    }

    //UImanager for each scene, UImanager as prefab

    //playtime UI
    //sec:min:millisec
    //overall play time

    private string FormatPlayTime(float playtime)
    {
        return TimeSpan.FromSeconds(playtime).ToString(@"mm\:ss\.ff");
    }

    public void UpdatePlayTime(float playTime)
    {
        playTimeText.text = $"Playtime... {FormatPlayTime(playTime)}";
    }

    public void UpdateCurrentPlayTime(float playTime)
    {
        currentPlayTimeText.text = $"On current stage... {FormatPlayTime(playTime)}";
    }

    public void UpdateStage(string stageName)
    {
        currentStageText.text = stageName;
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
}
