
using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelLoader : MonoBehaviour
{

    public int level = 1;
    float levelTimer;

    public int bossLevel;
    SettingsManager settingsManager;

    public float[] startingTimes;
    bool startedGame;
    [HideInInspector] public float time;
    public List<Item> inventory;

    bool completedTutorial = false;

    void Awake()
    {

        DontDestroyOnLoad(gameObject);
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Level Loader");
        for (int i = 0; i < objs.Length; i++)
        {
            if (i != 0) Destroy(objs[i]);
        }
        levelTimer = 0;



    }

    void Start()
    {
        settingsManager = FindFirstObjectByType<SettingsManager>();
        if (!startedGame)
        {
            inventory = new List<Item>();
            time = startingTimes[settingsManager.difficulty];
            startedGame = true;
            level = 1;
        }

    }

    void Update()
    {
        levelTimer += Time.deltaTime;
    }



    public void LoadLevel(string newSceneName)
    {
        string sceneName = newSceneName;
        string curScene = SceneManager.GetActiveScene().name;

        if (curScene == "SampleScene" && sceneName == "Shop")
        {
            time = FindFirstObjectByType<Timer>().GetTime();
            PlayerItems items = FindFirstObjectByType<PlayerItems>();

            LevelSpawner levelSpawner = FindFirstObjectByType<LevelSpawner>();
            if (levelTimer < ((5 + ((level - 1) * levelSpawner.increaseAmount)) * 12))
            {
                time += 15 * items.CountItem("Stopwatch");
            }

            time -= items.CountItem("Credit Card") * 6;

            time *= 1 + (0.1f * items.CountItem("Piggy Bank"));
        }
        else if (curScene == "Shop")
        {
            time = FindFirstObjectByType<ShopManager>().time;
            inventory = FindFirstObjectByType<ShopManager>().inventory;
            level++;

            if (level == bossLevel && sceneName == "SampleScene") sceneName = "Boss Fight";
        }
        if (sceneName == "Menu" || curScene == "Menu")
        {
            startedGame = false;
            time = startingTimes[settingsManager.difficulty];
            inventory = new List<Item>();
            level = 1;
        }


        if (curScene == "Tutorial" && sceneName == "Shop")
        {
            completedTutorial = true;
            Retry();

        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    public void Retry()
    {
        startedGame = false;
        time = startingTimes[settingsManager.difficulty];
        inventory = new List<Item>();
        level = 1;
        SceneManager.LoadScene("SampleScene");

    }

    public void RetryTutorial()
    {
        startedGame = false;
        time = startingTimes[settingsManager.difficulty];
        inventory = new List<Item>();
        level = 1;
        SceneManager.LoadScene("Tutorial");
    }

    public void Play()
    {
        LoadLevel(completedTutorial ? "SampleScene" : "Cutscene");
    }


}
