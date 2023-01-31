using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RhythmObject : MonoBehaviour
{
    Anim_Roots[] childRoots;

    protected Rigidbody2D rb;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        childRoots = GetComponentsInChildren<Anim_Roots>();

        AudioManager.OnRhythmUpdate += Next;
    }

    void OnDestroy()
    {
        AudioManager.OnRhythmUpdate -= Next;
    }

    protected abstract void Next();

    protected bool CanMove()
    {
        foreach (var root in childRoots)
        {
            if (root.IsLockInWall)
                return false;
        }

        return true;
    }

    protected abstract void RevertToDefaults();

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, .25f);
    }
}
