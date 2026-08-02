
using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    public event Action OnReset;

    public int level = 1;
    public int bossLevel;
    SettingsManager settingsManager;

    public float[] startingTimes;
    bool startedGame;
    [HideInInspector] public float time;

    bool completedTutorial = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

    }

    void Start()
    {
        settingsManager = FindFirstObjectByType<SettingsManager>();
        if (!startedGame)
        {
            time = startingTimes[settingsManager.difficulty];
            startedGame = true;
            level = 1;
        }

    }


    public void Retry()
    {
        OnReset?.Invoke();
        startedGame = false;
        time = startingTimes[settingsManager.difficulty];
        level = 1;
        SceneManager.LoadScene("SampleScene");

    }

    public void RetryTutorial()
    {
        startedGame = false;
        time = startingTimes[settingsManager.difficulty];
        level = 1;
        SceneManager.LoadScene("Tutorial");
    }

    public void Play()
    {
        if (completedTutorial)
        {
            Retry();
        }
        else
        {
            SceneManager.LoadScene("Cutscene");
        }
    }

    public void ContinueFromShop()
    {
        time = FindFirstObjectByType<ShopManager>().time;
        level++;

        if (level == bossLevel)
        {
            SceneManager.LoadScene("Boss Fight");
        }
        else
        {
            SceneManager.LoadScene("SampleScene");
        }
    }

    public void ContinueFromLevel()
    {
        if (SceneManager.GetActiveScene().name == "Tutorial")
        {
            completedTutorial = true;
            Retry();
            return;
        }

        Player player = FindFirstObjectByType<Player>();

        List<IOnRoundEnd> items = InventoryManager.Instance.GetInterfaceList<IOnRoundEnd>();
        foreach (IOnRoundEnd item in items)
        {
            item.OnRoundEnd(player);
        }

        time = player.GetTime();
        SceneManager.LoadScene("Shop");
    }

    public void Menu()
    {
        OnReset?.Invoke();
        startedGame = false;
        time = startingTimes[settingsManager.difficulty];
        level = 1;

        SceneManager.LoadScene("Menu");
    }


}
