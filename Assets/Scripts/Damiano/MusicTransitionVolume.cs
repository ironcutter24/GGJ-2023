using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MusicTransitionVolume : MonoBehaviour
{
    [SerializeField, Range(0, 3)]
    int trackIndex = 0;

    public void EnqueueTransition()
    {
        Debug.Log("New track: " + trackIndex);
        AudioManager.Instance.SetTrackIndex(trackIndex);
    }
}
