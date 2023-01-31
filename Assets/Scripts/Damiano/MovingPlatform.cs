using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : RhythmObject
{
    [SerializeField]
    MoveDirection moveDirection = MoveDirection.Right;

    [SerializeField]
    int steps = 4, stepSize = 1;

    [SerializeField]
    int currentIndex;

    int startIndex;

    Vector2 startPosition;

    enum MoveDirection { Left = -1, Right = 1 }

    protected override void Start()
    {
        base.Start();
        startPosition = rb.position;
        startIndex = currentIndex;
    }

    protected override void Next()
    {
        if (CanMove())
        {
            if (moveDirection == MoveDirection.Left && currentIndex == 0)
                moveDirection = MoveDirection.Right;
            else
            if (moveDirection == MoveDirection.Right && currentIndex == steps - 1)
                moveDirection = MoveDirection.Left;

            currentIndex += (int)moveDirection;
            rb.DOMove(GetPositionFrom(currentIndex), .4f);
        }
    }

    protected override void RevertToDefaults()
    {
        currentIndex = startIndex;
        rb.position = startPosition;
    }

    Vector2 GetPositionFrom(int index)
    {
        return startPosition + Vector2.right * index * stepSize;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.DrawLine(GetPositionFrom(0), GetPositionFrom(steps - 1));
    }
}
