using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingPlatform : RhythmObject
{
    [SerializeField]
    TurnDirection turnDirection = TurnDirection.Clockwise;

    enum TurnDirection { CounterClockwise = -1, Clockwise = 1 }

    protected override void Start()
    {
        direction = (int)turnDirection;
        base.Start();
    }

    protected override void Move()
    {
        rb.DORotate(-90f * direction * stepSize, AudioManager.LerpDuration).SetRelative();
    }

    protected override void RevertToDefaults()
    {
        Debug.LogWarning("Check rotating platform reset");
        //rb.rotation = (startIndex - currentIndex) * 90;
        //currentIndex = startIndex;
    }
}
