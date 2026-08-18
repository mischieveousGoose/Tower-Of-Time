using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransitions : MonoBehaviour
{
    public static SceneTransitions Instance;

    [SerializeField] private Animator transitionAnimator;
    [SerializeField] private float transitionTime = 1f;

    private bool isTransitioning = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        PlayAnimation("TransitionIn");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayAnimation("TransitionIn");
    }

    private void PlayAnimation(string animationName)
    {
        if (transitionAnimator == null)
        {
            transitionAnimator = GetComponentInChildren<Animator>(true);
        }

        if (transitionAnimator != null)
        {
            transitionAnimator.Play(animationName);
        }
    }

    public void TransitionToScene(string sceneName)
    {
        if (!isTransitioning)
        {
            StartCoroutine(Transition(sceneName));
        }
    }

    private IEnumerator Transition(string sceneName)
    {
        isTransitioning = true;
        Time.timeScale = 1f;
        PlayAnimation("TransitionOut");

        yield return new WaitForSecondsRealtime(transitionTime);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        PlayAnimation("TransitionIn");
        yield return new WaitForSecondsRealtime(transitionTime);

        isTransitioning = false;
    }
}
