using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : RhythmObject
{
    Vector2 startPosition;

    [SerializeField]
    int span;

    [SerializeField]
    int currentPosition;

    enum MoveDirection { Left = -1, Right = 1 }

    [SerializeField]
    MoveDirection moveDirection = MoveDirection.Right;

    protected override void Start()
    {
        base.Start();
        startPosition = rb.position;
    }

    protected override void Next()
    {
        if (moveDirection == MoveDirection.Left && currentPosition == 0)
            moveDirection = MoveDirection.Right;
        else
        if (moveDirection == MoveDirection.Right && currentPosition == span - 1)
            moveDirection = MoveDirection.Left;

        currentPosition += (int)moveDirection;

        rb.DOMove(startPosition + Vector2.right * currentPosition, .4f);
    }
}
