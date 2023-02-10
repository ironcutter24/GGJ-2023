using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerAudioManager : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] StudioEventEmitter stepsSFX;
    [SerializeField] float stepSoundDuration = .2f;
    [SerializeField] StudioEventEmitter jumpSFX, landingSFX, stickingSFX, growSongSFX, shrinkSongSFX, deathOnSpikesSFX, deathInWaterSFX;
}
