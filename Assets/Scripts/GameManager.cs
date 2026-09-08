using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public GameObject wp;
    public static GameManager Instance;

    public event Action OnReset;
    public Action OnEnemyKilled;

    public int bossLevel;

    bool completedTutorial = false;
    public RunData runData;

    public Character character;

    public Area[] areas;

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


    public void ChooseCharacter(Character character)
    {
        this.character = character;
        if (completedTutorial)
        {
            Retry();
        }
        else
        {
            RetryTutorial();
        }
    }


    public void Retry()
    {
        OnReset?.Invoke();
        runData = new RunData(StatsManager.Instance.startStats.maxTime, wp);
        SceneTransitions.Instance.TransitionToScene(areas[runData.area].levelScene);
    }

    public void RetryTutorial()
    {
        runData = new RunData(StatsManager.Instance.startStats.maxTime, wp);
        SceneTransitions.Instance.TransitionToScene("Tutorial");
    }

    public void ContinueFromRoadmap(NodeType nodeType)
    {
        Roadmap roadmap = FindFirstObjectByType<Roadmap>();
        runData.rootNode = roadmap.rootNode;
        runData.currentNode = roadmap.currentNode;
        runData.level++;
        runData.levelTimer = 0;

        switch (nodeType)
        {
            case NodeType.Level:
                SceneTransitions.Instance.TransitionToScene(areas[runData.area].levelScene);
                break;
            case NodeType.Shop:
                SceneTransitions.Instance.TransitionToScene("Shop");
                break;
            case NodeType.CombatEncounter:
                SceneTransitions.Instance.TransitionToScene(areas[runData.area].combatEncounterScene);
                break;
            case NodeType.Boss:
                SceneTransitions.Instance.TransitionToScene(areas[runData.area].bossScene);
                break;
        }

    }

    public void Play()
    {
        SceneTransitions.Instance.TransitionToScene("CharacterSelect");
    }

    public void CompleteBoss()
    {
        runData.area++;
        runData.level = 1;
        runData.levelTimer = 0;

        runData.rootNode = null;
        runData.currentNode = null;

        SceneTransitions.Instance.TransitionToScene(areas[runData.area].levelScene);
    }

    public void ContinueFromShop()
    {
        ShopManager shopManager = FindFirstObjectByType<ShopManager>();
        runData.time = shopManager.time;
        runData.primaryWeapon = shopManager.primaryWeapon;
        runData.secondaryWeapon = shopManager.secondaryWeapon;

        Time.timeScale = 1;
        SceneTransitions.Instance.TransitionToScene("Roadmap");
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
        SceneTransitions.Instance.TransitionToScene("Roadmap");
    }

    public void Menu()
    {
        OnReset?.Invoke();
        runData = new RunData(StatsManager.Instance.startStats.maxTime, wp);

        SceneTransitions.Instance.TransitionToScene("Menu");
    }


}

[Serializable]
public struct Area
{
    public string name;
    public string levelScene;
    public string bossScene;
    public string combatEncounterScene;

    public Area(string name, string levelScene, string bossScene, string combatEncounterScene)
    {
        this.name = name;
        this.levelScene = levelScene;
        this.bossScene = bossScene;
        this.combatEncounterScene = combatEncounterScene;
    }
}
