using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using Stage4;

public class Scene4
{
    [UnityTest]
    public IEnumerator TestPlayerControlDisablesAndReenables()
    {
        var asyncLoad = SceneManager.LoadSceneAsync("Stage4Scene");
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        var parent = GameObject.Find("Player");
        Assert.IsNotNull(parent, "부모 오브젝트가 씬에 존재하지 않습니다.");

        var player = parent.transform.Find("Ball")?.gameObject;
        Assert.IsNotNull(player, "부모의 자식 중 Ball 오브젝트가 존재하지 않습니다.");

        var control = player.GetComponent<NewPlayerControl>();
        if (control == null)
        {
            control = player.AddComponent<NewPlayerControl>();
        }

        control.canControl = true;

        var triggerObject = new GameObject("TriggerZone");
        var triggerCollider = triggerObject.AddComponent<BoxCollider>();
        triggerCollider.isTrigger = true;
        triggerObject.AddComponent<DisablePlayerControlTrigger>().duration = 0.1f;

        triggerObject.transform.position = player.transform.position;

        yield return null;
        yield return new WaitForFixedUpdate();

        Assert.IsFalse(control.canControl, "Player control should be disabled");

        yield return new WaitForSeconds(0.2f);
        Assert.IsTrue(control.canControl, "Player control should be re-enabled after delay");
    }

    [UnityTest]
    public IEnumerator TestReverseMoveBehaviour_Perform_ReversesVelocity()
    {
        var asyncLoad = SceneManager.LoadSceneAsync("Stage4Scene");
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        var parent = GameObject.Find("Player");
        Assert.IsNotNull(parent, "부모 오브젝트가 씬에 존재하지 않습니다.");

        var ball = parent.transform.Find("Ball")?.gameObject;
        Assert.IsNotNull(ball, "Ball 오브젝트가 존재하지 않습니다.");

        var rb = ball.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = ball.AddComponent<Rigidbody>();
        }

        /*rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;*/

        var reverseMove = ball.GetComponent<ReverseMoveBehaviour>();
        if (reverseMove == null)
        {
            reverseMove = ball.AddComponent<ReverseMoveBehaviour>();
        }

        reverseMove.initialVelocity = new Vector3(1f, 0f, 0f);
        reverseMove.initialAngularVelocity = Vector3.zero;
        reverseMove.duration = 0.5f;

        yield return null;

        reverseMove.Perform();

        var startPos = ball.transform.position;

        yield return new WaitForSeconds(0.25f);
        var midPos = ball.transform.position;
        Assert.Greater(midPos.x, startPos.x, "Ball should have moved forward.");

        yield return new WaitForSeconds(0.4f);
        var finalPos = ball.transform.position;
        Assert.Less(finalPos.x, midPos.x, "Ball should have moved backward after reversal.");

        Debug.Log($"Start: {startPos.x}, Mid: {midPos.x}, Final: {finalPos.x}");
    }
}
