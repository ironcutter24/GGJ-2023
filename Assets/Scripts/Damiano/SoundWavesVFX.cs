using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundWavesVFX : MonoBehaviour
{
    [SerializeField]
    GameObject wavePrefab;

    [SerializeField]
    float minScale = 1, period = 2f;

    List<SpriteRenderer> circles = new List<SpriteRenderer>();

    void Start()
    {
        //StartCoroutine(_WaveAnimation(5));
    }

    public void Play(float range)
    {
        StartCoroutine(_WaveAnimation(range));
    }

    IEnumerator _WaveAnimation(float range)
    {
        float targetScale = range * .4f;

        for (int i = 0; i < 6; i++)
        {
            var c = Instantiate(wavePrefab, transform.position, Quaternion.identity).GetComponent<SpriteRenderer>();
            c.color = new Color(1f, 1f, 1f, 1f);
            c.transform.localScale = Vector3.one * minScale;
            c.transform.DOScale(targetScale, period).SetEase(Ease.OutCubic);
            c.DOColor(new Color(1f, 1f, 1f, 0f), period).SetEase(Ease.OutQuart);

            circles.Add(c);

            yield return new WaitForSeconds(.2f);
        }
    }
}
