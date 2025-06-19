using UnityEngine.TestTools;
using NUnit.Framework;
using UnityEngine;
using System.Collections;
using Stage4;

public class Scene4
{
    [UnityTest]
    public IEnumerator TestPlayerMovesRight()
    {
        var player = new GameObject();
        player.tag = "Player";
        player.transform.position = Vector3.zero;

        float moveAmount = 5f * Time.deltaTime;
        player.transform.Translate(moveAmount, 0, 0);

        yield return null;

        Assert.Greater(player.transform.position.x, 0);
    }

    [UnityTest]
    public IEnumerator TestPlayerControlDisablesAndReenables()
    {
        // 1. 플레이어 생성 및 NewPlayerControl 컴포넌트 추가
        var player = new GameObject("Player");
        player.tag = "Player";
        var control = player.AddComponent<NewPlayerControl>();
        control.canControl = true;

        // 2. 트리거 오브젝트 및 스크립트 추가
        var triggerObject = new GameObject("TriggerZone");
        var collider = triggerObject.AddComponent<BoxCollider>();
        collider.isTrigger = true;
        triggerObject.transform.position = player.transform.position;

        var trigger = triggerObject.AddComponent<DisablePlayerControlTrigger>();
        trigger.duration = 0.1f; // 매우 짧게 테스트

        // 3. 트리거가 Awake에서 플레이어를 찾을 수 있도록 씬에 먼저 추가
        yield return null; // 다음 프레임까지 기다림 (Awake 실행됨)

        // 4. 강제로 OnTriggerEnter 호출
        trigger.SendMessage("OnTriggerEnter", player.GetComponent<Collider>(), SendMessageOptions.DontRequireReceiver);

        // 5. 즉시 조작 비활성화됐는지 확인
        Assert.IsFalse(control.canControl, "Player control should be disabled");

        // 6. duration 후 다시 활성화됐는지 확인
        yield return new WaitForSeconds(0.2f);
        Assert.IsTrue(control.canControl, "Player control should be re-enabled after delay");
    }
}

