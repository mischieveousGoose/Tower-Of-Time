using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{

    [Header("UI")]
    public Slider sfxSlider;
    public Slider musicSlider;

    public Image[] difficultyButtons;
    public GameObject settings;

    [Header("Settings")]
    public float sfxVolume = 1;
    public float musicVolume = 1;
    public int difficulty = 1;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        GameObject[] objs = GameObject.FindGameObjectsWithTag("SettingManager");
        for (int i = 0; i < objs.Length; i++)
        {
            if (i != 0) Destroy(objs[i]);
        }

    }

    public void CloseSettings()
    {
        sfxVolume = sfxSlider.value;
        musicVolume = musicSlider.value;
        settings.SetActive(false);

    }

    void RenderDifficulty()
    {
        for (int i = 0; i < 3; i++)
        {
            if (difficulty == i)
                difficultyButtons[i].color = Color.yellow;
            else
                difficultyButtons[i].color = Color.white;

        }
    }

    public void ChangeDifficulty(int val)
    {
        difficulty = val;
        RenderDifficulty();
    }

    public void OpenSettings()
    {
        settings.SetActive(true);
        sfxSlider.value = sfxVolume;
        musicSlider.value = musicVolume;

        RenderDifficulty();
    }
}
