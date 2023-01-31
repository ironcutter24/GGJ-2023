using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : RhythmObject
{
    [SerializeField]
    MoveDirection moveDirection = MoveDirection.Right;

    enum MoveDirection { Left = -1, Right = 1 }

    protected override void Start()
    {
        direction = (int)moveDirection;
        base.Start();
    }

    protected override void Move()
    {
        rb.DOMove(new Vector2((int)moveDirection * stepSize, 0f), .4f).SetRelative();
    }

    protected override void RevertToDefaults()
    {
        currentIndex = startIndex;
        //rb.position = startPosition;
    }
}
