using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingPlatform : RhythmObject
{
    [SerializeField]
    int startRotation;

    [SerializeField]
    int currentRotation;

    enum TurnDirection { Counterclockwise = -1, Clockwise = 1 }

    [SerializeField]
    TurnDirection turnDirection = TurnDirection.Clockwise;

    protected override void Next()
    {
        currentRotation += (int)turnDirection;

        if (currentRotation > 3)
            currentRotation -= 4;
        else
        if (currentRotation < 0)
            currentRotation += 4;

        rb.DORotate(-90f * currentRotation, .4f);
    }
}
