using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : RhythmObject
{
    [SerializeField]
    MoveDirection moveDirection = MoveDirection.Right;

    enum MoveDirection { Left = -1, Right = 1 }

    private Vector3 gridPosition;

    Player attachedPlayer;

    protected override void Start()
    {
        gridPosition = transform.position;
        direction = (int)moveDirection;
        base.Start();
    }

    public void AttachPlayer(Player target)
    {
        attachedPlayer = target;
    }

    public void DetachPlayer(Player target)
    {
        if (attachedPlayer == target)
            attachedPlayer = null;
    }

    protected override void Move()
    {
        gridPosition += new Vector3(direction * stepSize, 0f, 0f);

        rb.DOMove(new Vector2(direction * stepSize, 0f), AudioManager.PlatformLerpDuration).SetRelative();

        //if (attachedPlayer != null)
        //    attachedPlayer.MoveOverride();
    }

    protected override void RevertToDefaults()
    {
        //currentIndex = startIndex;
        //rb.position = startPosition;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Vector3 a = GetCurrentPosition() - new Vector3((currentIndex - startIndex) * stepSize, 0f, 0f);
        Vector3 b;

        for (int i = 0; i < steps; i++)
        {
            b = a + Vector3.right * stepSize * (int)moveDirection;
            Gizmos.color = i % 2 == 0 ? Color.blue : Color.red;
            Gizmos.DrawLine(a, b);
            a = b;
        }

        Vector3 GetCurrentPosition()
        {
            return (Application.isPlaying ? gridPosition : transform.position);
        }
    }
}
