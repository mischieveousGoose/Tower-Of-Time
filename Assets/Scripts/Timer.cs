using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{

    public float startingTime;
    float timer;

    public TMP_Text timerText;
    LevelLoader levelLoader;

    void Start()
    {
        levelLoader = FindFirstObjectByType<LevelLoader>();
        timer = levelLoader.time;
    }

    public float GetTime() { return timer; }

    void Update()
    {
        timer -= Time.deltaTime;
        timerText.text = (Mathf.Round(timer * 100) / 100).ToString();

        if (timer <= 0)
        {
            FindFirstObjectByType<TimeManager>().Lose();
        }
    }

    public void AddTime(float amount)
    {
        timer += amount;
    }
}
