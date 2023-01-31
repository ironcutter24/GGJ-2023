using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClippingPlane : MonoBehaviour
{
    [SerializeField] Material[] sliceMats;

    void Update()
    {
        RefreshMaterials();
    }

    public void SetMaterials(Material[] sliceMats)
    {
        this.sliceMats = sliceMats;
        RefreshMaterials();
    }

    public void RefreshMaterials()
    {
        Plane plane = new Plane(transform.up, transform.position);
        Vector4 planeVisulization = new Vector4(plane.normal.x, plane.normal.y, plane.normal.z, plane.distance);

        for (int i = 0; i < sliceMats.Length; i++)
            sliceMats[i].SetVector("_Plane", planeVisulization);
    }
}
