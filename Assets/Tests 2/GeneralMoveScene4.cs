using UnityEngine.TestTools;
using NUnit.Framework;
using UnityEngine;
using System.Collections;
using Stage4;

public class Scene4
{
    [UnityTest]
    public IEnumerator TestPlayerControlDisablesAndReenables()
    {
        // 1. 플레이어 생성 및 컴포넌트 추가
        var player = new GameObject("Player");
        player.tag = "Player";
        var control = player.AddComponent<NewPlayerControl>();
        control.canControl = true;

        // 플레이어에 Rigidbody 및 Collider 추가
        player.AddComponent<Rigidbody>().useGravity = false;
        player.AddComponent<CapsuleCollider>();

        // 2. 트리거 오브젝트 생성
        var triggerObject = new GameObject("TriggerZone");
        var triggerCollider = triggerObject.AddComponent<BoxCollider>();
        triggerCollider.isTrigger = true;
        triggerObject.AddComponent<DisablePlayerControlTrigger>().duration = 0.1f;

        // 트리거 위치 조정 (플레이어 위치와 겹치도록)
        triggerObject.transform.position = player.transform.position;

        // 3. 다음 프레임까지 대기해서 Awake 호출
        yield return null;

        // 4. 충돌 감지를 위해 FixedUpdate 타이밍까지 대기
        yield return new WaitForFixedUpdate();

        // 5. 제어 비활성화 확인
        Assert.IsFalse(control.canControl, "Player control should be disabled");

        // 6. duration 이후 다시 활성화되는지 확인
        yield return new WaitForSeconds(0.2f);
        Assert.IsTrue(control.canControl, "Player control should be re-enabled after delay");
    }
}
