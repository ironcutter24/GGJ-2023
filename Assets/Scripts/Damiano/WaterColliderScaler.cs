using Bitgem.VFX.StylisedWater;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WaterColliderScaler : MonoBehaviour
{
    [SerializeField]
    BoxCollider2D boxCollider;

    [SerializeField]
    WaterVolumeBox waterVolume;

    void Start()
    {
        FitCollider();
    }

#if UNITY_EDITOR
    void Update()
    {
        if (!Application.isPlaying)
        {
            FitCollider();
        }
    }
#endif

    void FitCollider()
    {
        var size = (Vector2)waterVolume.Dimensions;
        boxCollider.size = size - Vector2.one * .2f;
        boxCollider.offset = (size - Vector2.one) * .5f;
    }
}
