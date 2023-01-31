using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anim_Roots : MonoBehaviour
{
    [SerializeField]
    private GameObject graphics, wallColld;
    [SerializeField]
    private float maxLenght = 8, durationAhead = 10, durationRetreat = 6;
    [SerializeField]
    private bool moveAtStart;
    private BoxCollider2D Collider;
    private bool isAhead, isRetreat;

    Sequence mySequence;

    public bool IsLockInWall { get; private set; } = false;

    void Start()
    {
        Collider = GetComponent<BoxCollider2D>();

        isAhead = false;

        CheckAtStart();
    }

    public void CheckAtStart()
    {
        if (graphics.transform.position == transform.position)
        {
            isRetreat = true;
        }
        else
        {
            isRetreat = false;
        }

        if (moveAtStart)
        {
            Ahead_Root();
        }
    }

    public void Ahead_Root()
    {

        if (!isAhead)
        {
            isRetreat = true;
            isAhead = true;
            mySequence = DOTween.Sequence();
            mySequence.Append(graphics.transform.DOLocalMoveX(maxLenght, durationAhead).OnUpdate(CollUpdate));
        }

    }
    public void Retreat_Root()
    {

        if (!isRetreat)
        {
            isRetreat = true;
            isAhead = true;
            mySequence = DOTween.Sequence();
            mySequence.Append(graphics.transform.DOLocalMoveX(-wallColld.transform.localPosition.x, durationRetreat).OnUpdate(CollUpdateBack));

            IsLockInWall = false;
        }

    }

    void CollUpdate()
    {
        float x = graphics.transform.localPosition.x;
        Collider.offset = new Vector2(x * 0.5f, Collider.offset.y);
        Collider.size = new Vector2(x - 0.02f, Collider.size.y);

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
        if (collision.gameObject.tag != "Player")
        {
            mySequence.Kill();
            isRetreat = false;

            IsLockInWall= true;
        }
    }

}
