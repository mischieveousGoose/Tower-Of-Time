using UnityEngine;
using UnityEngine.UI;

public class SettingsGetter : MonoBehaviour
{
    SettingsManager settingsManager;

    [Header("UI")]
    public Slider sfxSlider;
    public Slider musicSlider;

    public Image[] difficultyButtons;
    public GameObject settings;

    void Start()
    {
        settingsManager = FindFirstObjectByType<SettingsManager>();
        Debug.Log(settingsManager);
        settingsManager.sfxSlider = sfxSlider;
        settingsManager.musicSlider = musicSlider;
        settingsManager.difficultyButtons = difficultyButtons;
        settingsManager.settings = settings;
    }

    public void OpenSettings()
    {
        Debug.Log("Open Settings clicked");
        settingsManager.OpenSettings();
    }

    public void CloseSettings()
    {
        settingsManager.CloseSettings();
    }

    public void ChangeDifficulty(int val)
    {
        settingsManager.ChangeDifficulty(val);
    }

}
