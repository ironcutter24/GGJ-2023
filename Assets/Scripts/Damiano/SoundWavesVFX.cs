using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundWavesVFX : MonoBehaviour
{
    [SerializeField]
    GameObject wavePrefab;

    void Play()
    {
        var renderer = Instantiate(wavePrefab, transform.position, Quaternion.identity).GetComponent<SpriteRenderer>();
        renderer.color = new Color(1f, 1f, 1f, .4f);


    }
}
