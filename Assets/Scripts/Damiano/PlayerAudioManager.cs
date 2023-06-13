using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    [SerializeField]
    EventReference steps, jump, landing, sticking, growSong, shrinkSong, deathOnSpikes, deathInWater;

    public void PlaySteps() { PlaySFX(steps); }

    public void PlayJump() { PlaySFX(jump); }

    public void PlayLanding() { PlaySFX(landing); }

    public void PlayStick() { PlaySFX(sticking); }

    public void PlayGrowSong() { PlaySFX(growSong); }

    public void PlayShrinkSong() { PlaySFX(shrinkSong); }

    public void PlayDeathOnSpikes() { PlaySFX(deathOnSpikes); }

    public void PlayDeathInWater() { PlaySFX(deathInWater); }

    void PlaySFX(EventReference eventReference)
    {
        var audioEvent = RuntimeManager.CreateInstance(eventReference);
        audioEvent.start();
    }
}
