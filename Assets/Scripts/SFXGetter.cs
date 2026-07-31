using UnityEngine;

public class SFXGetter : MonoBehaviour
{
    SFXManager sfxManager;

    void Start()
    {
        sfxManager = FindFirstObjectByType<SFXManager>();

    }

    public void PlaySFX(int i)
    {
        sfxManager.PlaySFX(i);
    }
}
