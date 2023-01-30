using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    Player target;

    [SerializeField]
    [Range(0f, 1f)]
    float lerpInterpolation = .2f;

    [SerializeField]
    Vector2 offset = Vector2.zero;

    Vector2 targetOffset = Vector2.zero;
    Vector2 TargetPosition => target.transform.position + new Vector3(targetOffset.x + offset.x, targetOffset.y + offset.y, 0f);

    void LateUpdate()
    {
        var newPosition = Vector3.Lerp(transform.position, TargetPosition, lerpInterpolation * 60 * Time.deltaTime);
        newPosition.z = transform.position.z;

        transform.position = newPosition;
    }

    float GetOffsetSignX()
    {
        return true ? -1f : 1f;
    }
}