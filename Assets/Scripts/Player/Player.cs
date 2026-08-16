using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float startingTime;
    public float maxTime;
    float timer;

    public Rank rank;

    public TMP_Text timerText;


    void Start()
    {
        timer = GameManager.Instance.runData.time;
        maxTime = StatsManager.Instance.GetStat(StatType.MaxTime);
    }

    public float GetTime() { return timer; }

    void Update()
    {
        timer -= Time.deltaTime;
        timerText.text = (Mathf.Round(timer * 100) / 100).ToString() + "/" + maxTime.ToString();

        if (timer <= 0)
        {
            FindFirstObjectByType<TimeManager>().Lose();
        }

        timer = Mathf.Min(maxTime, timer);
    }

    public void AddTime(float amount)
    {
        timer += amount;
    }
}
