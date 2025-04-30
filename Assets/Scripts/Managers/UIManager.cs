using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI playtimeText;
    public TextMeshProUGUI currentStageText;


    //UImanager for each scene, UImanager as prefab

    //playtime UI 관리하는 함수
    //분:초:밀리초
    public void UpdatePlayTime(float playtime)
    {
        int minutes = Mathf.FloorToInt(playtime / 60); 
        int seconds = Mathf.FloorToInt(playtime % 60); 
        int milliseconds = Mathf.FloorToInt((playtime * 100) % 100); 

        playtimeText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", minutes, seconds, milliseconds);
    }

    //set stage text at each scene UI
    //stage text:  "DM", "DS", "EEC", "SWPP"
    //thus no need for Updatestage as it will be set in scene

}