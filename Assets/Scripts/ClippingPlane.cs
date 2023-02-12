using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClippingPlane : MonoBehaviour
{
    MeshRenderer[] renderers;

    void Start()
    {
        renderers = GetComponentsInChildren<MeshRenderer>(true);

        foreach (var r in renderers)
        {
            for (int i = 0; i < r.materials.Length; i++)
            {
                r.materials[i] = new Material(r.materials[i]);

                r.materials[i].SetVector("_Point", transform.position);
                r.materials[i].SetVector("_Normal", transform.right);
            }
        }
    }

    void FixedUpdate()
    {
        foreach (var r in renderers)
        {
            for (int i = 0; i < r.materials.Length; i++)
            {
                r.materials[i].SetVector("_Point", transform.position);
                r.materials[i].SetVector("_Normal", transform.right);
            }
        }
    }
}
