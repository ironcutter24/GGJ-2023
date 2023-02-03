using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingPlatform : RhythmObject
{
    [SerializeField]
    TurnDirection turnDirection = TurnDirection.Clockwise;

    [SerializeField]
    GameObject CogGraphics;

    enum TurnDirection { CounterClockwise = -1, Clockwise = 1 }

    protected override void Start()
    {
        direction = (int)turnDirection;
        base.Start();
    }

    protected override void Move()
    {
        rb.DORotate(-90f * direction * stepSize, AudioManager.PlatformLerpDuration).SetRelative();
    }

    protected override void CannotMove()
    {
        base.CannotMove();

        CogGraphics.transform.DORotate(Vector3.forward * 10f * (-direction), .1f).SetRelative()
            .OnComplete(TurnBack);

        void TurnBack()
        {
            CogGraphics.transform.DORotate(Vector3.forward * 10f * direction, .1f).SetRelative();
        }
    }

    protected override void RevertToDefaults()
    {
        Debug.LogWarning("Check rotating platform reset");
        //rb.rotation = (startIndex - currentIndex) * 90;
        //currentIndex = startIndex;
    }
}
