using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using TMPro;

public class EndingScenePlayModeTest
{
    GameObject player;
    GameObject hatTrigger;
    GameObject newPlayer;
    InputName inputNameScript;
    EndingCreditScript creditScript;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        // (1) 씬 로드 생략 (필요 시 LoadSceneAsync 등으로 추가)

        yield return null;

        // (2) 플레이어 생성
        player = new GameObject("Player");
        player.tag = "Player";
        player.AddComponent<CharacterController>();
        player.transform.position = Vector3.zero;

        // (3) 모자 Trigger 생성
        hatTrigger = new GameObject("HatTrigger");
        var collider = hatTrigger.AddComponent<BoxCollider>();
        collider.isTrigger = true;
        hatTrigger.transform.position = new Vector3(0, 0, 1);
        var hatCollideEnd = hatTrigger.AddComponent<HatCollideEnd>();

        // (4) newPlayer 생성
        newPlayer = new GameObject("NewPlayer");
        newPlayer.SetActive(false);
        newPlayer.AddComponent<NewPlayerMoveToHeaven>();

        // (5) CreditManager 생성 + EndingCreditScript 연결
        var creditManagerGO = new GameObject("CreditManager");
        creditScript = creditManagerGO.AddComponent<EndingCreditScript>();
        creditScript.creditText = new GameObject("CreditText").AddComponent<RectTransform>();
        creditScript.creditText.SetParent(creditManagerGO.transform);

        // (6) NameInput 생성 + InputName 연결
        var nameInputGO = new GameObject("NameInput");
        inputNameScript = nameInputGO.AddComponent<InputName>();
        inputNameScript.creditRoll = creditScript;

        inputNameScript.nameInputPanel = new GameObject("NameInputPanel");
        inputNameScript.nameInputPanel.SetActive(false);

        var inputFieldGO = new GameObject("InputField");
        inputNameScript.nameInputField = inputFieldGO.AddComponent<TMP_InputField>();
        inputNameScript.nameInputField.text = "";

        var submitButtonGO = new GameObject("SubmitButton");
        inputNameScript.submitButton = submitButtonGO.AddComponent<Button>();

        inputNameScript.endingCreditPanel = new GameObject("CreditPanel");
        inputNameScript.endingCreditPanel.SetActive(false);

        inputNameScript.scoreButton = new GameObject("ScoreButton").AddComponent<Button>();
        inputNameScript.restartButton = new GameObject("RestartButton").AddComponent<Button>();

        inputNameScript.scoreButton.gameObject.SetActive(false);
        inputNameScript.restartButton.gameObject.SetActive(false);

        // (7) 연결
        hatCollideEnd.currentPlayer = player;
        hatCollideEnd.newPlayer = newPlayer;
        hatCollideEnd.originalHat = hatTrigger;
        hatCollideEnd.inputUIManager = inputNameScript;
        hatCollideEnd.liftSpeed = 2f;

        yield return null;
    }

    [UnityTest]
    public IEnumerator PlayerCollidesWithHat_ShowsNameInputAndStartsCreditRoll()
    {
        // 충돌 유도
        player.transform.position = hatTrigger.transform.position;
        yield return new WaitForFixedUpdate();

        // 1. 닉네임 입력 UI 켜졌는지 확인
        Assert.IsTrue(inputNameScript.nameInputPanel.activeSelf, "Name Input Panel should be active after collision");

        // 2. 닉네임 입력 후 제출
        inputNameScript.nameInputField.text = "TestPlayer";
        inputNameScript.submitButton.onClick.Invoke();

        yield return null;

        // 3. 크레딧 패널이 켜졌는지 확인
        Assert.IsTrue(inputNameScript.endingCreditPanel.activeSelf, "Credit panel should be active after submitting name");
        Assert.IsNotNull(creditScript, "Credit script should exist");

        // 4. 크레딧 초기 위치 확인
        Assert.AreEqual(-100f, creditScript.creditText.anchoredPosition.y, 0.01f);

        // 5. 크레딧 롤이 끝날 때까지 기다리기
        float timeout = 5f;
        while (creditScript.creditText.anchoredPosition.y < creditScript.endY && timeout > 0)
        {
            yield return null;
            timeout -= Time.deltaTime;
        }

        // 6. 점수/재시작 버튼 보이는지 확인
        Assert.IsTrue(inputNameScript.scoreButton.gameObject.activeSelf, "Score button should be visible after credits");
        Assert.IsTrue(inputNameScript.restartButton.gameObject.activeSelf, "Restart button should be visible after credits");
    }
}
