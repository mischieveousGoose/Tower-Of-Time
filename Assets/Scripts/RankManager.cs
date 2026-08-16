using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RankManager : MonoBehaviour
{
    public GameObject rankUI;
    public TMP_Text rankText;
    public TMP_Text bonusText;

    public float[] timePerRoomForRanks;
    public float[] timeRewards;

    float timeReward;

    public void Continue()
    {
        if (SceneManager.GetActiveScene().name == "Tutorial" || SceneManager.GetActiveScene().name == "Combat") GameManager.Instance.ContinueFromLevel();

        FindFirstObjectByType<TimeManager>().BeatLevel();

        Rank rank = CalculateRank();
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

    Rank CalculateRank()
    {
        float time = GameManager.Instance.runData.levelTimer / FindFirstObjectByType<LevelSpawner>().levelLength;
        for (int i = 0; i < timePerRoomForRanks.Length; i++)
        {
            if (time < timePerRoomForRanks[i]) return (Rank)i;
        }
        return Rank.F;

    }
}

public enum Rank
{
    S, A, B, C, D, F
}
