
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

    public float startTime;

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

    void Start()
    {
        settingsManager = FindFirstObjectByType<SettingsManager>();
    }


    public void Retry()
    {
        OnReset?.Invoke();
        runData = new RunData(startTime, wp);
        SceneManager.LoadScene("SampleScene");

    }

    public void RetryTutorial()
    {
        runData = new RunData(startTime, wp);
        SceneManager.LoadScene("Tutorial");
    }

    public void ContinueFromRoadmap(string sceneName)
    {
        Roadmap roadmap = FindFirstObjectByType<Roadmap>();
        runData.rootNode = roadmap.rootNode;
        runData.currentNode = roadmap.currentNode;
        runData.level++;
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


        SceneManager.LoadScene("Roadmap");
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

        runData.time = player.GetTime();
        SceneManager.LoadScene("Roadmap");
    }

    public void Menu()
    {
        OnReset?.Invoke();
        runData = new RunData(startTime, wp);

        SceneManager.LoadScene("Menu");
    }


}
