using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public static Vector2 LastActivated { get; private set; }

    public static void Set(Vector2 position)
    {
        LastActivated = position;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            LastActivated = transform.position;
        }
    }
}
