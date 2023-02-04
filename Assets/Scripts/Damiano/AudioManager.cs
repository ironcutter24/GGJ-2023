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

    [Header("Music settings")]
    [SerializeField]
    int bpm = 120;

    enum Subdivision { Half = 2, Quarter = 4, Eighth = 8 }

    [SerializeField]
    Subdivision subdivision = Subdivision.Quarter;

    [SerializeField]
    int step = 4;
    [Space]
    [SerializeField]
    bool isLevelOnlyMode = false;

    [Header("Platforms settings")]
    [SerializeField]
    private float platformLerpDuration = .4f, rootsForwardSpeed = 16, rootsBackwardSpeed = 12;
    public static float PlatformLerpDuration => _instance.platformLerpDuration;
    public static float RootsForwardSpeed => _instance.rootsForwardSpeed;
    public static float RootsBackwardSpeed => _instance.rootsBackwardSpeed;


    public static event Action OnRhythmUpdate;

    Coroutine rhythmCoroutine = null;

    private void Start()
    {
        FMODEventInstance = FMODUnity.RuntimeManager.CreateInstance(fmodEvent);
        FMODEventInstance.setParameterByName("Lvl", 1);
        FMODEventInstance.setParameterByName("MenuTheme", isLevelOnlyMode ? 1f : 0f);
        FMODEventInstance.start();

        rhythmCoroutine = StartCoroutine(_RhythmUpdate());
    }

    public void GoToLevel()
    {
        FMODEventInstance.setParameterByName("Lvl", 1);
        FMODEventInstance.setParameterByName("MenuTheme", 1f);

        Cursor.visible = false;
    }

    public void GoToMenu()
    {
        FMODEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

        StopCoroutine(rhythmCoroutine);
        rhythmCoroutine = null;

        Cursor.visible = true;
        Start();
    }

    public void SetTrackIndex(int trackIndex)
    {
        FMODEventInstance.setParameterByName("Lvl", trackIndex);
    }

    IEnumerator _RhythmUpdate()
    {
        while (true)
        {
            float period = bpm / (int)subdivision / 60f * step;
            float rootLerpDuration = 12 / RootsForwardSpeed;

            //if (period - platformLerpDuration - rootLerpDuration <= 0)
            //    throw new Exception("Roots buffer window is larger than audio update period!");

            yield return new WaitForSeconds(platformLerpDuration);

            IsUpdating = false;

            yield return new WaitForSeconds(period - platformLerpDuration - rootLerpDuration);

            IsUpdating = true;

            yield return new WaitForSeconds(rootLerpDuration);

            // -> Updating event
            OnRhythmUpdate?.Invoke();
        }
    }

    public static bool IsUpdating { get; private set; } = false;
    IEnumerator _FlagManager()
    {
        IsUpdating = true;
        yield return new WaitForSeconds(platformLerpDuration);
        IsUpdating = false;
    }
}
