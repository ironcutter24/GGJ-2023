using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility.Patterns;

public class AudioManager : Singleton<AudioManager>
{
    private FMOD.Studio.EventInstance FMODEventInstance;

    [SerializeField]
    FMODUnity.EventReference fmodEvent;

    [Space]

    [SerializeField]
    int bpm = 120;

    enum Subdivision { Half = 2, Quarter = 4, Eighth = 8 }

    [SerializeField]
    Subdivision subdivision = Subdivision.Quarter;

    [SerializeField]
    int step = 4;

    public static event Action OnRhythmUpdate;

    private void Start()
    {
        StartGameMusic();
    }

    void StartGameMusic()
    {
        //FMODEventInstance = FMODUnity.RuntimeManager.CreateInstance(fmodEvent);
        //FMODEventInstance.start();
        StartCoroutine(_RhythmUpdate());
    }

    IEnumerator _RhythmUpdate()
    {
        while (true)
        {
            float period = bpm / (int)subdivision / 60f * step;
            yield return new WaitForSeconds(period);
            OnRhythmUpdate();
        }
    }

}
