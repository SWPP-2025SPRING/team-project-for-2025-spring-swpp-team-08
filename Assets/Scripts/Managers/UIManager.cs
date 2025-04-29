using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public TextMeshProUGUI playtimeText;
    public TextMeshProUGUI currentStageText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        UpdatePlayTime();
    }

    //각 씬마다 UIManager를 component로 갖는 object 만들어 두고
    //씬마다 UI 복붙해둔 뒤
    //씬 전환 시 UI component와 코드 연결. 저는 과제 때 이렇게 했는데
    //더 나은 방법이 있으면 추후 수정 예정
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        playtimeText = GameObject.Find("PlayTime")?.GetComponent<TextMeshProUGUI>();
        currentStageText = GameObject.Find("Stage")?.GetComponent<TextMeshProUGUI>();
        UpdatePlayTime();
        //UpdateStage();
    }

    //playtime UI 관리하는 함수
    //분:초:밀리초
    public void UpdatePlayTime()
    {
        float playtime = GameManager.Instance.playManager.GetPlaytime();
        int minutes = Mathf.FloorToInt(playtime / 60); 
        int seconds = Mathf.FloorToInt(playtime % 60); 
        int milliseconds = Mathf.FloorToInt((playtime * 100) % 100); 

        playtimeText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", minutes, seconds, milliseconds);
    }

    //스테이지 텍스트 관리
    //이산수학 - 자료구조 - 전기전자회로 - 소개원실
    /*
    private readonly string[] stageNames = { "DM", "DS", "EEC", "SWPP" }; //TBD
    public void UpdateStage()
    {
        int stageIndex = GameManager.Instance.playManager.stageNo - 1;

        if (stageIndex >= 0 && stageIndex < stageNames.Length)
        {
            currentStageText.text = stageNames[stageIndex];
        }
        else
        {
            currentStageText.text = "???";
        }
    }
    */

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}