using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RankManager : MonoBehaviour
{
    public GameObject rankUI;
    public TMP_Text rankText;
    public TMP_Text bonusText;

    public float[] levelThresholds;
    public float[] timeRewards;

    [Header("Combat Encounter")]
    public float[] survivalThresholds;
    public float[] eliteThresholds;
    public float[] killAllThresholds;

    float timeReward;

    public void Continue()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "Tutorial")
        {
            GameManager.Instance.ContinueFromLevel();
            return;
        }

        FindFirstObjectByType<TimeManager>().BeatLevel();

        Rank rank = sceneName == "Combat" ? CalculateCombatRank() : CalculateLevelRank();
        rankText.text = rank.HumanName();
        timeReward = timeRewards[(int)rank];

        bonusText.text = "+" + timeReward.ToString() + "s";
        FindFirstObjectByType<Player>().rank = rank;


        rankUI.SetActive(true);
    }

    public void FinishLevel()
    {
        GameManager.Instance.ContinueFromLevel(timeReward);
    }

    void Start()
    {
        if (rankUI != null)
            rankUI.SetActive(false);
    }

    Rank CalculateLevelRank()
    {
        float time = GameManager.Instance.runData.levelTimer / FindFirstObjectByType<LevelSpawner>().levelLength;
        for (int i = 0; i < levelThresholds.Length; i++)
        {
            if (time < levelThresholds[i]) return (Rank)i;
        }
        return Rank.F;

    }

    Rank CalculateCombatRank()
    {
        CombatEncounter combatEncounter = FindFirstObjectByType<CombatEncounter>();
        EncounterType type = combatEncounter.encounterType;

        // Get whether rank will be scored off time or kills based on encounter type
        float scoringVal = type == EncounterType.Survive ? combatEncounter.kills : combatEncounter.timer;

        // get the correct rank thresholds depending on encounter type
        float[] scoringArray = type switch
        {
            EncounterType.KillAll => killAllThresholds,
            EncounterType.Elite => eliteThresholds,
            EncounterType.Survive => survivalThresholds
        };

        // Check if its something where you want to have a low score like time
        bool isLowScorer = type != EncounterType.Survive;

        if (isLowScorer)
            for (int i = 0; i < scoringArray.Length; i++)
            {
                if (scoringVal < scoringArray[i])
                    return (Rank)i;
            }

        else
            for (int i = scoringArray.Length - 1; i >= 0; i--)
            {
                if (scoringVal > scoringArray[i])
                    return (Rank)i;
            }

        return Rank.F;

    }
}

public enum Rank
{
    S, A, B, C, D, F
}
