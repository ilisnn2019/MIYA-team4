using UnityEngine;
using System.Collections;
using System;

public static class ColorUtil
{
    public static IEnumerator LerpColor(Color startColor, Color endColor, Action<Color> func, float duration = 3f)
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            func?.Invoke(Color.Lerp(startColor, endColor, smoothT));
            yield return null;
        }

        func?.Invoke(endColor);
    }
}
