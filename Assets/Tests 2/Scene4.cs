using UnityEngine.TestTools;
using NUnit.Framework;
using UnityEngine;
using System.Collections;

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
}
