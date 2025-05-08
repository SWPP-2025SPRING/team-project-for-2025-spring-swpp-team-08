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

    private Coroutine _coroutine;

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
        playTimeText.text = $"Playtime...{FormatPlayTime(playTime)}";
    }

    public void UpdateCurrentPlayTime(float playTime)
    {
        currentPlayTimeText.text = $"On current stage...{FormatPlayTime(playTime)}";
    }

    public void UpdateStage(string stageName)
    {
        currentStageText.text = stageName;
    }

    public void UpdateSubtitle(string subtitle, float duration)
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
        subtitleText.text = subtitle;
        subtitleText.gameObject.SetActive(true);
        _coroutine = StartCoroutine(HideSubtitleAfterDelay(duration));
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

    private IEnumerator HideSubtitleAfterDelay(float duration)
    {
        yield return new WaitForSeconds(duration);
        subtitleText.gameObject.SetActive(false);
    }

    //set stage text at each scene UI
    //stage text:  "DM", "DS", "EEC", "SWPP"
    //thus no need for Updatestage as it will be set in scene

}
