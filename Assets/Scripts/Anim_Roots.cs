using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Anim_Roots : MonoBehaviour
{
    public GameObject graphics, wallColld;
    [SerializeField]
    private float maxLenght = 8, duration = 10;
    private BoxCollider2D Collider;

    Sequence mySequence;
    void Start()
    {
        Collider = GetComponent<BoxCollider2D>();
        mySequence = DOTween.Sequence();
      
        mySequence
            .Append(graphics.transform.DOLocalMoveX(maxLenght, duration).OnUpdate(CollUpdate));
            //.Append(graphics.transform.DORotate(new Vector3(180, 180, 180), 0.5f).SetLoops(20));


    }

    void CollUpdate()
    {
        float x = graphics.transform.localPosition.x;
        Collider.offset = new Vector2(x * 0.5f, Collider.offset.y);
        Collider.size = new Vector2(x, Collider.size.y);

        graphics.transform.Rotate(new Vector3(0, -60, 0) * Time.deltaTime, Space.Self);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        mySequence.Kill();
     
    }

}
