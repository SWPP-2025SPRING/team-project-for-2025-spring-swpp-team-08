using NUnit.Framework;
using UnityEngine;
using TMPro;

public class UIUnit
{
    private UIManager _uiManager;
    private GameObject _go;

    [SetUp]
    public void SetUp()
    {
        _go = new GameObject();
        _uiManager = _go.AddComponent<UIManager>();

        _uiManager.playTimeText = new GameObject().AddComponent<TextMeshProUGUI>();
        _uiManager.currentPlayTimeText = new GameObject().AddComponent<TextMeshProUGUI>();
        _uiManager.currentStageText = new GameObject().AddComponent<TextMeshProUGUI>();
        _uiManager.subtitleText = new GameObject().AddComponent<TextMeshProUGUI>();
        _uiManager.percentText = new GameObject().AddComponent<TextMeshProUGUI>();
        _uiManager.progressBar = new GameObject().AddComponent<UnityEngine.UI.Slider>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_go);
    }

    [Test]
    public void UpdatePlayTime_SetsTextCorrectly()
    {
        _uiManager.UpdatePlayTime(65.4321f);
        Assert.AreEqual("1:05.432", _uiManager.playTimeText.text);
    }

    [Test]
    public void UpdateCurrentPlayTime_SetsTextCorrectly()
    {
        _uiManager.UpdateCurrentPlayTime(123.456f);
        Assert.AreEqual("2:03.456", _uiManager.currentPlayTimeText.text);
    }

    [Test]
    public void UpdateStage_SetsStageTextCorrectly()
    {
        _uiManager.UpdateStage("EEC");
        Assert.AreEqual("EEC", _uiManager.currentStageText.text);
    }

    [Test]
    public void SetProgress_FormatsPercentCorrectly()
    {
        _uiManager.SetProgress(0.6789f);
        Assert.AreEqual("67.9", _uiManager.percentText.text);
    }

    [Test]
    public void SetProgressBar_SetsSliderValueCorrectly()
    {
        _uiManager.SetProgressBar(0.42f);
        Assert.AreEqual(0.42f, _uiManager.progressBar.value);
    }

    [Test]
    public void UpdateSubtitle_SetsSubtitleTextAndActivatesObject()
    {
        _uiManager.UpdateStateSubtitle("Hello", 2f);

        Assert.AreEqual("Hello", _uiManager.subtitleText.text);
        Assert.IsTrue(_uiManager.subtitleText.gameObject.activeSelf);
    }


}
