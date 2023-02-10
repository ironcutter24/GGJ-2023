using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    StudioListener audioListener;
    [Space]
    [SerializeField]
    Player target;

    [SerializeField]
    float interpolation = 2.80f;

    [SerializeField]
    Vector2 offset = Vector2.zero;

    [SerializeField]
    float cameraDistance = 22f;

    Vector2 targetOffset = Vector2.zero;
    Vector2 TargetPosition => target.transform.position + new Vector3(targetOffset.x + offset.x * GetOffsetSignX(), targetOffset.y + offset.y, 0f);

    void Start()
    {
        if (target == null)
            target = GameObject.Find("Player").GetComponent<Player>();

        SetTargetPosition2D(TargetPosition);
    }

    void LateUpdate()
    {
        SetTargetPosition2D(Vector3.Lerp(transform.position, TargetPosition, interpolation * Time.deltaTime));
        audioListener.transform.localPosition = Vector3.forward * cameraDistance;
    }

    void SetTargetPosition2D(Vector2 newPosition)
    {
        transform.position = new Vector3(newPosition.x, newPosition.y, -cameraDistance);
    }

    float GetOffsetSignX()
    {
        return target.IsFacingRight ? 1f : -1f;
    }
}