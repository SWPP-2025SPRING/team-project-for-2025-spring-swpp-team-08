using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using Stage4;

public class Scene4
{
    [UnityTest]
    public IEnumerator TestDisablePlayerControlTrigger()
    {
        //disableplayercontroltrigger test
        var asyncLoad = SceneManager.LoadSceneAsync("Stage4Scene");
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        var parent = GameObject.Find("Player");
        Assert.IsNotNull(parent, "No player in scene");

        var player = parent.transform.Find("Ball")?.gameObject;
        Assert.IsNotNull(player, "No ball in player");

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
    public IEnumerator TestReverseMoveBehaviour()
    {
        //reverse test
        var go = new GameObject("TempBall");
        var rb = go.AddComponent<Rigidbody>();
        rb.useGravity = false;

        var reverse = go.AddComponent<ReverseMoveBehaviour>();
        reverse.initialVelocity = Vector3.right * 3f;
        reverse.initialAngularVelocity = Vector3.zero;
        reverse.duration = 0.5f;

        reverse.Perform();
        var startX = go.transform.position.x;

        yield return new WaitForSeconds(0.25f);
        var midX = go.transform.position.x;
        Assert.Greater(midX, startX, "Object should move forward initially.");

        yield return new WaitForSeconds(0.4f);
        var endX = go.transform.position.x;
        Assert.Less(endX, midX, "Object should have reversed and moved backward.");

        Debug.Log($"Start: {startX}, Mid: {midX}, End: {endX}");

        Object.DestroyImmediate(go);
    }

    [UnityTest]
    public IEnumerator TestStopMoveBehaviour()
    {
        var go = new GameObject("TempBall");
        var rb = go.AddComponent<Rigidbody>();
        rb.useGravity = false;

        var stopMove = go.AddComponent<StopMoveBehaviour>();
        stopMove.initialVelocity = Vector3.right * 3f;
        stopMove.initialAngularVelocity = Vector3.zero;
        stopMove.duration = 1.0f;

        stopMove.Perform();
        var startX = go.transform.position.x;

        yield return new WaitForSeconds(0.5f);
        var midX = go.transform.position.x;

        Assert.Greater(midX, startX, "Object should move initially.");

        yield return new WaitForSeconds(0.5f); 
        var postStop1 = go.transform.position.x;

        Assert.Greater(midX- startX, postStop1-midX ,"Object should slow down");
        yield return new WaitForSeconds(0.2f);

        var postStop2 = go.transform.position.x;

        var drift = Mathf.Abs(postStop2 - postStop1);
        Debug.Log($"Start: {startX}, Mid: {midX}, AfterStop1: {postStop1}, AfterStop2: {postStop2}, Drift: {drift}");

        Assert.Less(drift, 0.05f, "Object should have fully stopped after the duration.");

        Object.DestroyImmediate(go);
    }

    [UnityTest]
    public IEnumerator TestDisappearAfterDelayTrigger()
    {
        var asyncLoad = SceneManager.LoadSceneAsync("Stage4Scene");
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        var target = GameObject.CreatePrimitive(PrimitiveType.Cube);
        target.name = "TargetObject";
        target.SetActive(false);

        var triggerObject = new GameObject("TriggerZone");
        var triggerCollider = triggerObject.AddComponent<BoxCollider>();
        triggerCollider.isTrigger = true;
        var triggerRb = triggerObject.AddComponent<Rigidbody>();
        triggerRb.isKinematic = true;

        var trigger = triggerObject.AddComponent<DisappearAfterDelayTrigger>();
        trigger.targetObjects = new[] { target };
        trigger.delay = 0.5f;

        var parent = GameObject.Find("Player");
        Assert.IsNotNull(parent, "No player in scene");

        var player = parent.transform.Find("Ball")?.gameObject;
        Assert.IsNotNull(player, "No ball in player");

        if (player.GetComponent<NewPlayerControl>() == null)
            player.AddComponent<NewPlayerControl>();

        if (player.GetComponent<Collider>() == null)
            player.AddComponent<SphereCollider>();

        var playerRb = player.GetComponent<Rigidbody>();
        if (playerRb == null)
            playerRb = player.AddComponent<Rigidbody>();
        playerRb.useGravity = false;
        player.tag = "Player";

        player.transform.position = Vector3.zero;
        triggerObject.transform.position = Vector3.zero;

        yield return new WaitForSeconds(0.1f);

        Assert.IsTrue(target.activeSelf, "Target object should be active after trigger");

        yield return new WaitForSeconds(0.6f);

        Assert.IsFalse(target.activeSelf, "Target object should be inactive after delay");

    }

    [UnityTest]
    public IEnumerator TestSimpleMoveBehaviour()
    {
        // GameObject 생성
        var go = new GameObject("SimpleMover");
        var rb = go.AddComponent<Rigidbody>();
        rb.useGravity = false;

        var behaviour = go.AddComponent<SimpleMoveBehaviour>();
        behaviour.destination = Vector3.forward * 2f;
        behaviour.duration = 1.0f;
        behaviour.isDestinationRelative = true;

        var startPos = go.transform.position;
        var expectedEnd = startPos + Vector3.forward * 2f;
        var expectedMid = Vector3.Lerp(startPos, expectedEnd, 0.5f);

        behaviour.Perform();

        yield return new WaitForSeconds(0.5f);
        var midPos = go.transform.position;

        var midDist = Vector3.Distance(midPos, expectedMid);
        Debug.Log($"[Mid] Expected: {expectedMid}, Actual: {midPos}");

        Assert.That(midDist, Is.LessThan(0.2f), "Object did not reach the expected midpoint");

        yield return new WaitForSeconds(0.6f);
        var endPos = go.transform.position;
        var totalDist = Vector3.Distance(endPos, expectedEnd);

        Debug.Log($"[End] Expected: {expectedEnd}, Actual: {endPos}");

        Assert.That(totalDist, Is.LessThan(0.1f), "Object did not reach the final destination");
    }

    [UnityTest]
    public IEnumerator TestSimpleMoveRestoreBehaviour()
    {
        var go = new GameObject("MoveRestorer");
        var rb = go.AddComponent<Rigidbody>();
        rb.useGravity = false;

        var behaviour = go.AddComponent<SimpleMoveRestoreBehaviour>();
        behaviour.destination = Vector3.right * 3f;
        behaviour.destinationRotation = new Vector3(0, 90, 0);
        behaviour.duration = 0.5f;
        behaviour.delayBeforeMove = 0.1f;
        behaviour.delayAfterMove = 0.2f;
        behaviour.isDestinationRelative = true;

        var startPos = go.transform.position;
        var startRot = go.transform.rotation;

        behaviour.Perform();

        yield return new WaitForSeconds(0.9f);
        var midPos = go.transform.position;

        yield return new WaitForSeconds(0.6f);
        var endPos = go.transform.position;
        var endRot = go.transform.rotation;

        Debug.Log($"Start: {startPos}, Mid: {midPos}, End: {endPos}");

        Assert.That(Vector3.Distance(midPos, startPos), Is.GreaterThan(2.45f), "Did not move far enough");
        Assert.That(Vector3.Distance(endPos, startPos), Is.LessThan(0.1f), "Did not return to start");
        Assert.That(Quaternion.Angle(endRot, startRot), Is.LessThan(1f), "Rotation not restored");
    }

    private class MockBehaviour : PredefinedBehaviour
    {
        public bool wasPerformed = false;

        protected override void PerformInternal()
        {
            wasPerformed = true;
            performed = true;
        }
    }

    [UnityTest]
    public IEnumerator TestActivateButton()
    {
        var asyncLoad = SceneManager.LoadSceneAsync("Stage4Scene");
        while (!asyncLoad.isDone)
            yield return null;

        var parent = GameObject.Find("Player");
        Assert.IsNotNull(parent, "No player in scene");

        var player = parent.transform.Find("Ball")?.gameObject;
        Assert.IsNotNull(player, "No ball in player");

        var control = player.GetComponent<NewPlayerControl>();
        if (control == null)
            control = player.AddComponent<NewPlayerControl>();

        control.canControl = true;

        var go = new GameObject("ActivateButtonTestObject");
        var meshRenderer = go.AddComponent<MeshRenderer>();
        var activateButton = go.AddComponent<ActivateButton>();

        var collider = go.AddComponent<BoxCollider>();
        collider.isTrigger = true;

        var rb = go.AddComponent<Rigidbody>();
        rb.isKinematic = true;

        var mockBehaviour = go.AddComponent<MockBehaviour>();
        activateButton.behaviours = new PredefinedBehaviour[] { mockBehaviour };

        Assert.IsFalse(activateButton.isActivated);
        Assert.IsTrue(meshRenderer.enabled);
        Assert.IsFalse(mockBehaviour.wasPerformed);

        var playerRb = player.GetComponent<Rigidbody>();
        if (playerRb == null)
            playerRb = player.AddComponent<Rigidbody>();
        playerRb.isKinematic = true;

        var playerCollider = player.GetComponent<Collider>();
        if (playerCollider == null)
            playerCollider = player.AddComponent<SphereCollider>();

        go.transform.position = player.transform.position;

        yield return new WaitForFixedUpdate();
        Assert.IsTrue(activateButton.isActivated, "ActivateButton should be activated on trigger");
        Assert.IsFalse(meshRenderer.enabled, "MeshRenderer should be disabled after activation");
        Assert.IsTrue(mockBehaviour.wasPerformed, "MockBehaviour should have been performed");

        Object.Destroy(go);
    }

}
