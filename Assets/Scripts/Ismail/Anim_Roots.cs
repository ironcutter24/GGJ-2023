using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anim_Roots : MonoBehaviour
{
    [SerializeField]
    private GameObject graphics;

    [SerializeField, Range(1, 12)]
    private float maxLenght = 8;

    [SerializeField]
    private bool moveAtStart;

    private BoxCollider2D Collider;
    private bool isAhead, isRetreat;

    TweenerCore<Vector3, Vector3, VectorOptions> myTween;
    private RhythmObject attachedPlatform = null;

    public bool IsLockInWall { get; private set; } = false;

    void Start()
    {
        Collider = GetComponent<BoxCollider2D>();
        Collider.enabled = false;
        CheckAtStart();
    }

    public void CheckAtStart()
    {
        isRetreat = graphics.transform.position == transform.position;

        if (moveAtStart)
        {
            Ahead_Root();
        }
    }

    public void Ahead_Root()
    {
        if (!isAhead && extendQueueCoroutine == null)
        {
            if (AudioManager.IsUpdating)
            {
                Debug.Log("Root extended during update");
                extendQueueCoroutine = StartCoroutine(_ExtendQueue());
            }
            else
                Extend();
        }
    }

    Coroutine extendQueueCoroutine = null;
    IEnumerator _ExtendQueue()
    {
        while (AudioManager.IsUpdating)
            yield return null;

        Extend();
        extendQueueCoroutine = null;
    }

    void Extend()
    {
        Collider.enabled = true;
        isRetreat = isAhead = true;

        myTween = graphics.transform.DOLocalMoveX(maxLenght, AudioManager.RootsForwardSpeed)
            .OnUpdate(CollUpdate)
            .OnComplete(() => isRetreat = false)
            .SetSpeedBased();
    }

    public void Retreat_Root()
    {
        if (!isRetreat)
        {
            isRetreat = isAhead = true;
            IsLockInWall = false;

            myTween = graphics.transform.DOLocalMoveX(0f, AudioManager.RootsBackwardSpeed)
                .OnUpdate(CollUpdateBack)
                .OnComplete(() => Collider.enabled = false)
                .SetSpeedBased();

            if (attachedPlatform != null)
            {
                Debug.Log("Detaching root...");
                attachedPlatform.DetachRoot(this);
                attachedPlatform = null;
            }
        }
    }

    void CollUpdate()
    {
        float x = graphics.transform.localPosition.x;
        Collider.offset = new Vector2(x * 0.5f, Collider.offset.y);
        Collider.size = new Vector2(x - 0.1f, Collider.size.y);

        graphics.transform.Rotate(new Vector3(0, -60, 0) * Time.deltaTime, Space.Self);
    }

    void CollUpdateBack()
    {
        float x = graphics.transform.localPosition.x + 0.3f;
        Collider.offset = new Vector2(x * 0.5f, Collider.offset.y);
        Collider.size = new Vector2(x + 0.1f, Collider.size.y);

        graphics.transform.Rotate(new Vector3(0, 60, 0) * Time.deltaTime, Space.Self);
        if (graphics.transform.position == transform.position)
            isAhead = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            myTween.Kill();
            isRetreat = false;

            IsLockInWall = true;

            attachedPlatform = collision.gameObject.GetComponent<RhythmObject>();
            if (attachedPlatform != null)
            {
                Debug.Log("Attaching root...");
                attachedPlatform.AttachRoot(this);
            }
        }
    }

}
