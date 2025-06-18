using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class EndingScenePlayModeTest
{
    [UnityTest]
    public IEnumerator PlayerHitsHat_TriggersNicknameInputAndEndSequence()
    {
        // 씬 로드 및 초기화 코드 작성 (필요하면 SceneManager 사용)

        // 플레이어를 WASD 이동시키고 충돌 검사 시뮬레이션
        // 예: player.transform.position += Vector3.forward * Time.deltaTime;

        // 닉네임 입력창, 플레이어 상승, 엔딩 크레딧 등 로직 테스트

        yield return null; // 프레임 기다림
    }
}
