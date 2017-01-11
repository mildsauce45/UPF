using System;
using System.Collections;
using UnityEngine;

namespace FirstWave.Unity.Core.Utilities
{
    public static class SpriteRendererExtensions
    {
        public static void FadeOut(this SpriteRenderer renderer, MonoBehaviour mb, float duration, Action<SpriteRenderer> callback = null)
        {
            mb.StartCoroutine(FadeCoroutine(renderer, duration, (start, dur) => 1f - Mathf.Clamp01((Time.time - start) / dur), callback));
        }

        public static void FadeIn(this SpriteRenderer renderer, MonoBehaviour mb, float duration, Action<SpriteRenderer> callback = null)
        {
            mb.StartCoroutine(FadeCoroutine(renderer, duration, (start, dur) => Mathf.Clamp01((Time.time - start) / dur), callback));
        }

        private static IEnumerator FadeCoroutine(SpriteRenderer renderer, float duration, Func<float, float, float> fadeFunction, Action<SpriteRenderer> callback = null)
        {
            var start = Time.time;
            while (Time.time <= start + duration)
            {
                var color = renderer.color;
                color.a = fadeFunction(start, duration);
                renderer.color = color;
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
