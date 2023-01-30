using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundWavesVFX : MonoBehaviour
{
    [SerializeField]
    LineRenderer circleRenderer;

    void Play()
    {
        DrawCircle(8, 1f);
    }

    void DrawCircle(int steps, float radius)
    {
        circleRenderer.positionCount = steps;

        for (int currentSteps = 0; currentSteps < steps; currentSteps++)
        {
            float progress = (float)currentSteps / steps;

            float currentRadian = progress * 2 * Mathf.PI;

            Vector2 scaled = new Vector2(
                Mathf.Cos(currentRadian),
                Mathf.Sin(currentRadian)
                );

            Vector2 pos = scaled * radius;

            Vector3 currentPosition = new Vector3(pos.x, pos.y, 0f);

            circleRenderer.SetPosition(currentSteps, currentPosition);
        }
    }
}
