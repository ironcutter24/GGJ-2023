using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicVolumeDetector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("AudioVolume"))
        {
            collision.gameObject.GetComponent<MusicTransitionVolume>().EnqueueTransition();
        }
    }
}
