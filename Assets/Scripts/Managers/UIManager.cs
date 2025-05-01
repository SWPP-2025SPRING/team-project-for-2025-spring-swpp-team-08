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

    //�� ������ UIManager�� component�� ���� object ����� �ΰ�
    //������ UI �����ص� ��
    //�� ��ȯ �� UI component�� �ڵ� ����. ���� ���� �� �̷��� �ߴµ�
    //�� ���� ����� ������ ���� ���� ����
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        playtimeText = GameObject.Find("PlayTime")?.GetComponent<TextMeshProUGUI>();
        currentStageText = GameObject.Find("Stage")?.GetComponent<TextMeshProUGUI>();
        UpdatePlayTime();
        //UpdateStage();
    }

    //playtime UI �����ϴ� �Լ�
    //��:��:�и���
    public void UpdatePlayTime()
    {
        float playtime = GameManager.Instance.playManager.GetPlaytime();
        int minutes = Mathf.FloorToInt(playtime / 60); 
        int seconds = Mathf.FloorToInt(playtime % 60); 
        int milliseconds = Mathf.FloorToInt((playtime * 100) % 100); 

        playtimeText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", minutes, seconds, milliseconds);
    }

    //�������� �ؽ�Ʈ ����
    //�̻���� - �ڷᱸ�� - ��������ȸ�� - �Ұ�����
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