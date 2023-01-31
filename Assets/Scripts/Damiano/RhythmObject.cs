using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RhythmObject : MonoBehaviour
{
    [SerializeField]
    bool loops = true;

    [SerializeField]
    protected int steps = 4, stepSize = 1;

    protected int startIndex, currentIndex, direction;

    protected Rigidbody2D rb;
    Anim_Roots[] childRoots;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        childRoots = GetComponentsInChildren<Anim_Roots>();

        AudioManager.OnRhythmUpdate += Next;

        startIndex = GetBoundIndex(direction);
        currentIndex = startIndex;
    }

    private void OnDestroy()
    {
        AudioManager.OnRhythmUpdate -= Next;
    }

    private void Next()
    {
        if (!CanMove())
            return;

        if (loops)
            UpdateCurrentIndex();

        Move();
    }

    protected abstract void Move();

    protected abstract void RevertToDefaults();

    protected void UpdateCurrentIndex()
    {
        if (direction == -1 && currentIndex == 0)
            direction = 1;
        else
        if (direction == 1 && currentIndex == steps)
            direction = -1;

        currentIndex += direction;
    }

    protected int GetBoundIndex(int direction)
    {
        switch (direction)
        {
            case 1:
                return 0;
            case -1:
                return steps;
            default:
                throw new System.Exception("This is not a valid direction");
        }
    }

    protected bool CanMove()
    {
        foreach (var root in childRoots)
        {
            if (root.IsLockInWall)
                return false;
        }
        return true;
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, .25f);
    }
}
