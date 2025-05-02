using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI playtimeText;
    public TextMeshProUGUI currentPlayTimeText;
    public TextMeshProUGUI currentStageText;
    public TextMeshProUGUI subtitle;

    private void Start()
    {
        subtitle.gameObject.SetActive(false);
    }

    //UImanager for each scene, UImanager as prefab

    //playtime UI
    //sec:min:millisec
    //overall play time

    private string FormatPlayTime(float playtime)
    {
        return TimeSpan.FromSeconds(playtime).ToString(@"mm\:ss\.ff");
    }

    public void UpdatePlayTime(float playtime)
    {
        playtimeText.text = $"Playtime...{FormatPlayTime(playtime)}";
    }

    public void UpdateCurrentPlayTime(float playtime)
    {
        currentPlayTimeText.text = $"On current stage...{FormatPlayTime(playtime)}";
    }

    public void UpdateStage(string stageName)
    {
        currentStageText.text = stageName;
    }

    public void UpdateSubtitle(string subtitle)
    {

    }

    //set stage text at each scene UI
    //stage text:  "DM", "DS", "EEC", "SWPP"
    //thus no need for Updatestage as it will be set in scene

}