using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RhythmObject : MonoBehaviour
{
    protected Rigidbody2D rb;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        AudioManager.OnRhythmUpdate += Next;
    }

    void OnDestroy()
    {
        AudioManager.OnRhythmUpdate -= Next;
    }

    protected virtual void Next()
    {
        // Check for children roots
    }

    protected abstract void RevertToDefaults();
}
