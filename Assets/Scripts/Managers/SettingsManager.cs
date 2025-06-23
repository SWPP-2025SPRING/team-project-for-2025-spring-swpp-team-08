using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    private const int SliderMaxValue = 10;

    public Slider bgmVolumeSlider;
    public Slider mouseSensitivitySlider;
    public bool lockAfterSettings;

    private void LoadSettings()
    {
        bgmVolumeSlider.value = Mathf.Round(GameManager.Instance.bgmVolume * SliderMaxValue);
        mouseSensitivitySlider.value = Mathf.Round(GameManager.Instance.mouseSensitivity * SliderMaxValue);
    }

    private void OnEnable()
    {
        LoadSettings();
    }

    public void OpenSettingsPanel()
    {
        GameManager.Instance.UnlockCursor();
        LoadSettings();
        gameObject.SetActive(true);
    }

    public void CloseSettingsPanel()
    {
        if (lockAfterSettings)
        {
            GameManager.Instance.LockCursor();
        }
        gameObject.SetActive(false);
    }

    public void OnChangeBgmVolume()
    {
        var value = bgmVolumeSlider.value / SliderMaxValue;
        GameManager.Instance.SetBgmVolume(value);
    }

    public void OnChangeMouseSensitivity()
    {
        var value = mouseSensitivitySlider.value / SliderMaxValue;
        GameManager.Instance.SetMouseSensitivity(value);
    }
}
