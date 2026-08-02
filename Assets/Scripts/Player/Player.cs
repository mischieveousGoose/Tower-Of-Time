using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float startingTime;
    float timer;

    public TMP_Text timerText;


    void Start()
    {
        timer = GameManager.Instance.time;
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
