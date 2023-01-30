using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : RhythmObject
{
    [SerializeField]
    MoveDirection moveDirection = MoveDirection.Right;

    [SerializeField]
    int span;

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
        base.Next();

        if (moveDirection == MoveDirection.Left && currentIndex == 0)
            moveDirection = MoveDirection.Right;
        else
        if (moveDirection == MoveDirection.Right && currentIndex == span - 1)
            moveDirection = MoveDirection.Left;

        currentIndex += (int)moveDirection;

        rb.DOMove(GetPositionFrom(currentIndex), .4f);
    }

    protected override void RevertToDefaults()
    {
        currentIndex = startIndex;
        rb.position = startPosition;
    }

    Vector2 GetPositionFrom(int index)
    {
        return startPosition + Vector2.right * index;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(GetPositionFrom(0), GetPositionFrom(span - 1));
    }
}
