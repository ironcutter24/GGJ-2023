using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTilemap : MonoBehaviour
{
    void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Ground");

        var rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody2D>();

        rb.bodyType = RigidbodyType2D.Static;
        //rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        var composite = GetComponent<CompositeCollider2D>();
        if (composite == null)
            composite = gameObject.AddComponent<CompositeCollider2D>();
    }
}
