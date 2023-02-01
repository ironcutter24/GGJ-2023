using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundWavesVFX : MonoBehaviour
{
    [SerializeField]
    GameObject wavePrefab;

    [SerializeField]
    float minScale = 1;

    [SerializeField]
    int numberOfWaves = 6;

    List<SpriteRenderer> circles = new List<SpriteRenderer>();

    void Start()
    {
        for (int i = 0; i < numberOfWaves; i++)
        {
            var c = Instantiate(wavePrefab, transform.position, Quaternion.identity).GetComponent<SpriteRenderer>();
            c.color = new Color(1f, 1f, 1f, 0f);
            circles.Add(c);
        }

        //StartCoroutine(_WaveAnimation(5));
    }

    Coroutine wavesAnimCoroutine;
    public void Play(float range, float duration)
    {
        if (wavesAnimCoroutine == null)
            wavesAnimCoroutine = StartCoroutine(_WaveAnimation(range, duration));
    }

    IEnumerator _WaveAnimation(float range, float duration)
    {
        float targetScale = range * .4f;
        float timeBetweenWaves = duration / numberOfWaves;

        for (int i = 0; i < numberOfWaves; i++)
        {
            var c = circles[i];
            c.transform.position = transform.position;
            c.transform.localScale = Vector3.one * minScale;
            c.color = new Color(1f, 1f, 1f, 1f);

            c.transform.DOScale(targetScale, duration).SetEase(Ease.OutCubic);
            c.DOColor(new Color(1f, 1f, 1f, 0f), duration).SetEase(Ease.OutQuart);

            yield return new WaitForSeconds(timeBetweenWaves);
        }

        wavesAnimCoroutine = null;
    }
}
