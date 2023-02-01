using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RhythmObject : MonoBehaviour
{
    [SerializeField]
    bool isLoop = true;

    [SerializeField]
    protected int steps = 4, stepSize = 1;

    protected int startIndex, currentIndex, direction;

    List<Anim_Roots> attachedRoots = new List<Anim_Roots>();
    Anim_Roots[] childRoots;

    protected Rigidbody2D rb;

    private bool HasReachedStart => direction == -1 && currentIndex == 0;
    private bool HasReachedEnd => direction == 1 && currentIndex == steps;


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

    public void AttachRoot(Anim_Roots root)
    {
        attachedRoots.Add(root);
    }

    public void DetachRoot(Anim_Roots root)
    {
        attachedRoots.RemoveAll(x => x == root);
    }

    private void Next()
    {
        if (CanMove())
        {
            if (isLoop)
                UpdateCurrentIndex();

            Move();
        }
    }

    protected abstract void Move();

    protected abstract void RevertToDefaults();

    protected void UpdateCurrentIndex()
    {
        if (HasReachedStart || HasReachedEnd)
            direction *= -1;

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

        if (attachedRoots.Count > 0)
            return false;

        return true;
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, .25f);
    }
}
