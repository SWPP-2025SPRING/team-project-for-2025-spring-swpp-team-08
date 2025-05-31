using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using TMPro;
using UnityEngine.UI;

public class UIIntegration
{
    private UIManager uiManager;
    private GameObject go;

    [SetUp]
    public IEnumerator SetUp()
    {
        go = new GameObject("UIManagerGO");
        uiManager = go.AddComponent<UIManager>();

        uiManager.playTimeText = new GameObject("PlayTimeText").AddComponent<TextMeshProUGUI>();
        uiManager.currentPlayTimeText = new GameObject("CurrentPlayTimeText").AddComponent<TextMeshProUGUI>();
        uiManager.currentStageText = new GameObject("StageText").AddComponent<TextMeshProUGUI>();
        uiManager.subtitleText = new GameObject("SubtitleText").AddComponent<TextMeshProUGUI>();
        uiManager.percentText = new GameObject("PercentText").AddComponent<TextMeshProUGUI>();
        uiManager.progressBar = new GameObject("ProgressBar").AddComponent<Slider>();

        yield return null;
    }

    [Test]
    public IEnumerator ProgressBar_Updates_Visually()
    {
        float[] steps = { 0f, 0.25f, 0.5f, 0.75f, 1f };

        foreach (float p in steps)
        {
            uiManager.SetProgressBar(p);
            Debug.Log($"Progress set to: {p}");
            yield return new WaitForSeconds(0.5f);
        }

        Assert.AreEqual(1f, uiManager.progressBar.value);
    }

    [Test]
    public IEnumerator Subtitle_Appears_And_Disappears()
    {
        uiManager.UpdateSubtitle("Hello World", 1f);
        Assert.IsTrue(uiManager.subtitleText.gameObject.activeSelf);

        yield return new WaitForSeconds(1.1f);

        Assert.IsFalse(uiManager.subtitleText.gameObject.activeSelf);
    }

    [TearDown]
    public IEnumerator TearDown()
    {
        Object.Destroy(go);
        yield return null;
    }
}
