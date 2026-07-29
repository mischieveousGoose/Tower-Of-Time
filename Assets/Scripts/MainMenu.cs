using UnityEngine;

public class MainMenu : MonoBehaviour
{
    LevelLoader loader;

    void Start()
    {
        loader = FindFirstObjectByType<LevelLoader>();
    }

    public void Play()
    {
        loader.Play();
    }
}
