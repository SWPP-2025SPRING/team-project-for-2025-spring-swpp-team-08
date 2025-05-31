using NUnit.Framework;
using UnityEngine;
using TMPro;

public class UIUnit
{
    private UIManager uiManager;
    private GameObject go;

    [SetUp]
    public void SetUp()
    {
        go = new GameObject();
        uiManager = go.AddComponent<UIManager>();

        uiManager.playTimeText = new GameObject().AddComponent<TextMeshProUGUI>();
        uiManager.currentPlayTimeText = new GameObject().AddComponent<TextMeshProUGUI>();
        uiManager.currentStageText = new GameObject().AddComponent<TextMeshProUGUI>();
        uiManager.subtitleText = new GameObject().AddComponent<TextMeshProUGUI>();
        uiManager.percentText = new GameObject().AddComponent<TextMeshProUGUI>();
        uiManager.progressBar = new GameObject().AddComponent<UnityEngine.UI.Slider>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(go);
    }

    [Test]
    public void UpdatePlayTime_SetsTextCorrectly()
    {
        uiManager.UpdatePlayTime(65.4321f);
        Assert.AreEqual("Playtime...01:05.43", uiManager.playTimeText.text);
    }

    [Test]
    public void UpdateCurrentPlayTime_SetsTextCorrectly()
    {
        uiManager.UpdateCurrentPlayTime(123.456f);
        Assert.AreEqual("On current stage...02:03.46", uiManager.currentPlayTimeText.text);
    }

    [Test]
    public void UpdateStage_SetsStageTextCorrectly()
    {
        uiManager.UpdateStage("EEC");
        Assert.AreEqual("EEC", uiManager.currentStageText.text);
    }

    [Test]
    public void SetProgress_FormatsPercentCorrectly()
    {
        uiManager.SetProgress(0.6789f);
        Assert.AreEqual("67.9", uiManager.percentText.text);
    }

    [Test]
    public void SetProgressBar_SetsSliderValueCorrectly()
    {
        uiManager.SetProgressBar(0.42f);
        Assert.AreEqual(0.42f, uiManager.progressBar.value);
    }

    [Test]
    public void UpdateSubtitle_SetsSubtitleTextAndActivatesObject()
    {
        uiManager.UpdateSubtitle("Hello", 2f);

        Assert.AreEqual("Hello", uiManager.subtitleText.text);
        Assert.IsTrue(uiManager.subtitleText.gameObject.activeSelf);
    }


}
