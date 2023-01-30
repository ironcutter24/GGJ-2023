using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    GameObject target;

    [SerializeField]
    [Range(0f, 1f)]
    float lerpInterpolation = .2f;

    [SerializeField]
    float verticalOffset = 0f;

    Vector2 targetOffset = Vector2.zero;
    Vector2 TargetPosition => target.transform.position + new Vector3(targetOffset.x, targetOffset.y + verticalOffset, 0f);

    void LateUpdate()
    {
        var target = Vector3.Lerp(transform.position, TargetPosition, lerpInterpolation * 60 * Time.deltaTime);
        target.z = transform.position.z;

        transform.position = target;
    }
}