using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anim_Roots : MonoBehaviour
{
    [SerializeField]
    StudioEventEmitter growSFX, shrinkSFX;
    [Space]
    [SerializeField]
    private GameObject graphics;

    [SerializeField]
    LayerMask groundMask;

    [SerializeField, Range(1, 12)]
    private float maxLenght;

    [SerializeField, Range(0, 2)]
    private float minLenght = 1;

    [SerializeField]
    private bool moveAtStart;

    private BoxCollider2D Collider;
    private bool isAhead, isRetreat;

    TweenerCore<Vector3, Vector3, VectorOptions> myTween;
    private List<RhythmObject> attachedPlatforms = new List<RhythmObject>();

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
        RaycastHit2D hit = Physics2D.Raycast(transform.position + transform.right * .5f, transform.right, maxLenght - 1, groundMask);

        if (hit.collider != null)
        {
            Collider.enabled = true;
            isRetreat = isAhead = true;

            growSFX.Play();
            myTween = graphics.transform.DOLocalMoveX(maxLenght, AudioManager.RootsForwardSpeed)
                .OnUpdate(CollUpdate)
                .OnComplete(() => isRetreat = false)
                .SetSpeedBased();
        }
        else
        {
            Debug.LogWarning("Root could not hit ground");
            myTween = graphics.transform.DOLocalMoveX(minLenght, AudioManager.RootsForwardSpeed)
            .OnUpdate(CollUpdate)
                .OnComplete(() => myTween = graphics.transform.DOLocalMoveX(0f, AudioManager.RootsBackwardSpeed)
                .SetSpeedBased())
                .SetSpeedBased();
        }
    }

    public void Retreat_Root()
    {
        if (!isRetreat)
        {
            isRetreat = isAhead = true;
            IsLockInWall = false;

            shrinkSFX.Play();
            myTween = graphics.transform.DOLocalMoveX(0f, AudioManager.RootsBackwardSpeed)
                .OnUpdate(CollUpdateBack)
                .OnComplete(() => Collider.enabled = false)
                .SetSpeedBased();

            if (attachedPlatforms.Count > 0)
            {
                Debug.Log("Detaching root from platforms...");
                foreach (var p in attachedPlatforms)
                {
                    p.DetachRoot(this);
                }
                attachedPlatforms.Clear();
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
        if (groundMask == (groundMask | (1 << collision.gameObject.layer)))
        {
            myTween.Kill();
            isRetreat = false;

            IsLockInWall = true;

            var comp = collision.gameObject.GetComponent<RhythmObject>();
            if (comp != null)
            {
                Debug.Log("Attaching root...");
                comp.AttachRoot(this);
                attachedPlatforms.Add(comp);
            }
        }
    }

}
