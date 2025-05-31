using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using TMPro;
using UnityEngine.UI;

public class UIIntegration
{
    private UIManager _uiManager;
    private GameObject _go;

    [SetUp]
    public IEnumerator SetUp()
    {
        _go = new GameObject("UIManagerGO");
        _uiManager = _go.AddComponent<UIManager>();

        _uiManager.playTimeText = new GameObject("PlayTimeText").AddComponent<TextMeshProUGUI>();
        _uiManager.currentPlayTimeText = new GameObject("CurrentPlayTimeText").AddComponent<TextMeshProUGUI>();
        _uiManager.currentStageText = new GameObject("StageText").AddComponent<TextMeshProUGUI>();
        _uiManager.subtitleText = new GameObject("SubtitleText").AddComponent<TextMeshProUGUI>();
        _uiManager.percentText = new GameObject("PercentText").AddComponent<TextMeshProUGUI>();
        _uiManager.progressBar = new GameObject("ProgressBar").AddComponent<Slider>();

        yield return null;
    }

    [Test]
    public IEnumerator ProgressBar_Updates_Visually()
    {
        float[] steps = { 0f, 0.25f, 0.5f, 0.75f, 1f };

        foreach (float p in steps)
        {
            _uiManager.SetProgressBar(p);
            Debug.Log($"Progress set to: {p}");
            yield return new WaitForSeconds(0.5f);
        }

        Assert.AreEqual(1f, _uiManager.progressBar.value);
    }

    [Test]
    public IEnumerator Subtitle_Appears_And_Disappears()
    {
        _uiManager.UpdateSubtitle("Hello World", 1f);
        Assert.IsTrue(_uiManager.subtitleText.gameObject.activeSelf);

        yield return new WaitForSeconds(1.1f);

        Assert.IsFalse(_uiManager.subtitleText.gameObject.activeSelf);
    }

    [TearDown]
    public IEnumerator TearDown()
    {
        Object.Destroy(go);
        yield return null;
    }
}
