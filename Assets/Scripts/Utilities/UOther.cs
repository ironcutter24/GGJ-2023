using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    namespace Math
    {
        public static class UMath
        {
            /// <summary>
            /// Maps value from given range to [0, 1] and clamps between 0 and 1 included
            /// </summary>
            public static float Normalize(this float value, float min, float max)
            {
                var app = value.Map(min, max);
                return Mathf.Clamp(app, 0f, 1f);
            }

            /// <summary>
            /// Maps value from given range to [0, 1]
            /// </summary>
            public static float Map(this float value, float min, float max)
            {
                return (value - min) / (max - min);
            }

            public static float DivideByZero(this float dividend, float divider)
            {
                try
                {
                    return dividend / divider;
                }
                catch (System.DivideByZeroException)
                {
                    return float.PositiveInfinity;
                }
            }
        }
    }

    namespace Time
    {
        class UTime
        {
            public static string FormatTime(double time, bool alwaysShowHH = false, bool useDashes = false, string separator = ":")
            {
                var hh = DivisionStep(ref time, 3600);
                var mm = DivisionStep(ref time, 60);
                var ss = DivisionStep(ref time, 1);

                string formatted = "";
                if (alwaysShowHH || hh > 0)
                    formatted += FormatNum(hh) + separator;
                formatted += FormatNum(mm) + separator;
                formatted += FormatNum(ss);
                return formatted;

                int DivisionStep(ref double num, int den)
                {
                    var xx = (int)(num / den);
                    time -= xx * den;
                    return xx;
                }

                string FormatNum(int num)
                {
                    if (useDashes && num == 0) return "--";
                    return (num > 9 ? "" : "0") + num.ToString();
                }
            }
        }

        class Timer
        {
            private float timeFromStart = Mathf.NegativeInfinity;
            private float duration;
            private TimerType timerType = TimerType.Default;
            private float CurrentTime { get { return IsUnscaled ? UnityEngine.Time.unscaledTime : UnityEngine.Time.time; } }

            public bool IsUnscaled { get { return timerType == TimerType.Unscaled; } }
            public float Completion { get { return Math.UMath.Map(ElapsedTime, 0f, duration); } }
            public float RemainingTime { get { return duration - ElapsedTime; } }
            public float ElapsedTime { get { return UnityEngine.Time.time - timeFromStart; } }

            public bool IsExpired
            {
                get { return CurrentTime - timeFromStart > duration; }
            }

            public Timer(TimerType type = TimerType.Default)
            {
                timerType = type;
            }

            public void Set(float duration)
            {
                timeFromStart = CurrentTime;
                this.duration = duration;
            }

            public enum TimerType { Default, Unscaled }
        }
    }

    public static class Util
    {
        public static Vector3Int ScreenSize { get { return Vector3Int.right * Screen.width + Vector3Int.up * Screen.height; } }

        public static Vector3 ScreenCenter { get { return Vector3.right * ScreenSize.x * .5f + Vector3.up * ScreenSize.y * .5f; } }

        public static bool Chance(float chanceOfSuccess)
        {
            return Random.Range(0f, 1f) <= chanceOfSuccess;
        }

        public static void Try(this System.Action action)
        {
            if (action != null)
                action();
        }

        public static void Try(this System.Action<float> action, float parameter)
        {
            if (action != null)
                action(parameter);
        }

        public static bool IsBelowCamera(Transform trs)
        {
            return IsBelowCamera(trs.position);
        }

        public static bool IsBelowCamera(Vector3 pos)
        {
            return pos.y < Camera.main.transform.position.y;
        }

        public static void Tween(MonoBehaviour instance, float from, float to, float time, System.Action<float> callback)
        {
            instance.StartCoroutine(_Tween(from, to, time, callback));


            static IEnumerator _Tween(float from, float to, float time, System.Action<float> callback)
            {
                float mult = 1 / time;
                float state = 0;
                while (state < 1f)
                {
                    callback(Mathf.Lerp(from, to, state));
                    yield return null;
                    state += mult * UnityEngine.Time.deltaTime;
                }
                callback(to);
            }
        }
    }

    static class UCurve
    {
        static float doublePi { get { return Mathf.PI * 2f; } }

        public static AnimationCurve bell = new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(.5f, 1f),
            new Keyframe(1f, 0f)
            );

        public static AnimationCurve sine = new AnimationCurve(
            new Keyframe(0f, 0f, doublePi, doublePi),
            new Keyframe(.25f, 1f, 0f, 0f),
            new Keyframe(.75f, -1f, 0f, 0f),
            new Keyframe(1f, 0f, doublePi, doublePi)
            );
    }

    public class OneTimeEvent
    {
        bool performed = false;

        System.Func<bool> Condition;
        System.Action Callback;

        public OneTimeEvent(System.Func<bool> condition, System.Action callback)
        {
            this.Condition = condition;
            this.Callback = callback;
        }

        public void Evaluate()
        {
            if (!performed && Condition())
            {
                Callback();
                performed = true;
            }
        }

        public void Reset()
        {
            performed = false;
        }
    }
}

public static class ExtensionMethods
{

    

}
