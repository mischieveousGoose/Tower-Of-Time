using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{

    public List<AudioClip> sfx;
    AudioSource source;
    SettingsManager settingsManager;
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        GameObject[] objs = GameObject.FindGameObjectsWithTag("SFXManager");
        for (int i = 0; i < objs.Length; i++)
        {
            if (i != 0) Destroy(objs[i]);
        }

        source = GetComponent<AudioSource>();

    }

    public void PlaySFX(int index)
    {
        if (settingsManager == null) settingsManager = FindFirstObjectByType<SettingsManager>();
        if (source == null) source = GetComponent<AudioSource>();

        source.volume = settingsManager.sfxVolume;
        source.PlayOneShot(sfx[index]);
    }
}
