
using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{

    public GameObject wp;
    public static GameManager Instance;
    public event Action OnReset;

    public int bossLevel;
    SettingsManager settingsManager;

    bool completedTutorial = false;
    public RunData runData;

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

    void Update()
    {
        if (runData != null)
            runData.levelTimer += Time.deltaTime;
    }

    void Start()
    {
        settingsManager = FindFirstObjectByType<SettingsManager>();
    }


    public void Retry()
    {
        OnReset?.Invoke();
        runData = new RunData(StatsManager.Instance.startStats.maxTime, wp);
        SceneManager.LoadScene("SampleScene");

    }

    public void RetryTutorial()
    {
        runData = new RunData(StatsManager.Instance.startStats.maxTime, wp);
        SceneManager.LoadScene("Tutorial");
    }

    public void ContinueFromRoadmap(string sceneName)
    {
        Roadmap roadmap = FindFirstObjectByType<Roadmap>();
        runData.rootNode = roadmap.rootNode;
        runData.currentNode = roadmap.currentNode;
        runData.level++;
        runData.levelTimer = 0;
        SceneManager.LoadScene(sceneName);
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
        ShopManager shopManager = FindFirstObjectByType<ShopManager>();
        runData.time = shopManager.time;
        runData.primaryWeapon = shopManager.primaryWeapon;
        runData.secondaryWeapon = shopManager.secondaryWeapon;

        Time.timeScale = 1;
        SceneManager.LoadScene("Roadmap");
    }

    public void ContinueFromLevel(float timeReward = 0f)
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

        runData.time = player.GetTime() + timeReward;
        Time.timeScale = 1;
        SceneManager.LoadScene("Roadmap");
    }

    public void Menu()
    {
        OnReset?.Invoke();
        runData = new RunData(StatsManager.Instance.startStats.maxTime, wp);

        SceneManager.LoadScene("Menu");
    }


}
