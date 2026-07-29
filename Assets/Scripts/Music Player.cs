using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public List<AudioClip> music;
    int index;
    AudioSource source;
    SettingsManager settingsManager;

    void Start()
    {
        source = GetComponent<AudioSource>();
        index = Random.Range(0, music.Count);

    }

    void Update()
    {
        if (settingsManager == null)
        {
            settingsManager = FindFirstObjectByType<SettingsManager>();
        }
        source.volume = settingsManager.musicVolume;
        if (!source.isPlaying)
        {
            index++;
            if (index >= music.Count) index = 0;
            source.clip = music[index];
            source.Play();
        }
    }
}
