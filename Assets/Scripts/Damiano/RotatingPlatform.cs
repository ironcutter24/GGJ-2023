using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingPlatform : RhythmObject
{
    [SerializeField]
    float startRotation;

    enum TurnDirection { Counterclockwise = -1, Clockwise = 1 }

    [SerializeField]
    TurnDirection turnDirection = TurnDirection.Clockwise;

    protected override void Start()
    {
        base.Start();

        startRotation = rb.rotation;
    }

    protected override void Next()
    {
        base.Next();
        rb.DORotate(-90f * (int)turnDirection, .4f).SetRelative();
    }

    protected override void RevertToDefaults()
    {
        rb.rotation = startRotation;
    }
}
