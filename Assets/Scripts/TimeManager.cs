using UnityEngine;
using System.Collections;
using TMPro;


public class TimeManager : MonoBehaviour
{

    public TMP_Text countdownText;
    public GameObject LoseUI;
    public GameObject pauseMenu;
    GameManager gameManager => GameManager.Instance;

    bool inCountdown = false;
    bool hasLost = false;
    bool isPaused;
    public GameObject winScreen;
    public TMP_Text finalTime;

    Player player;

    bool hasWon;
    DialogueManager dialogueManager;

    SFXManager sfxManager;



    void Start()
    {
        sfxManager = FindFirstObjectByType<SFXManager>();
        StartCoroutine(Countdown());
        player = FindFirstObjectByType<Player>();
        dialogueManager = FindFirstObjectByType<DialogueManager>();
    }

    void Update()
    {

        bool inDialogue = false;
        if (dialogueManager != null) inDialogue = dialogueManager.startedDialogue;

        if (Input.GetKeyDown(KeyCode.Q) && !hasLost && !inCountdown && !hasWon && !inDialogue)
        {
            sfxManager.PlaySFX(3);
            isPaused = !isPaused;
        }
        if (hasLost || inCountdown || isPaused || hasWon || inDialogue)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }

        pauseMenu.SetActive(isPaused);
    }
    IEnumerator Countdown()
    {
        inCountdown = true;
        countdownText.gameObject.SetActive(true);
        countdownText.text = "3";

        yield return new WaitForSecondsRealtime(1f);
        countdownText.text = "2";

        yield return new WaitForSecondsRealtime(1f);
        countdownText.text = "1";

        yield return new WaitForSecondsRealtime(1f);
        inCountdown = false;
        countdownText.gameObject.SetActive(false);

    }

    public void Lose()
    {
        if (hasLost) return;
        sfxManager.PlaySFX(6);
        hasLost = true;
        LoseUI.SetActive(true);
    }

    public void Win()
    {
        if (hasWon) return;
        sfxManager.PlaySFX(7);
        hasWon = true;
        finalTime.text = "Final Time: " + (Mathf.Round(player.GetTime() * 100) / 100).ToString() + "s";
        winScreen.SetActive(true);

    }

    public void Retry()
    {
        sfxManager.PlaySFX(3);
        gameManager.Retry();
    }

    public void RetryTutorial()
    {
        sfxManager.PlaySFX(3);
        gameManager.RetryTutorial();
    }

    public void Menu()
    {
        sfxManager.PlaySFX(3);
        gameManager.Menu();
    }

    public void Resume()
    {
        sfxManager.PlaySFX(3);
        isPaused = false;
    }
}
