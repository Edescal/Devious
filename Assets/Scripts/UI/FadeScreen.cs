using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Edescal;

public class FadeScreen : MonoBehaviour
{
    public bool fading { get; private set; } = false;
    [field:SerializeField]
    public float fadeTime { get; private set; } = 0.75f;
    [SerializeField]
    private CanvasGroup canvasGroup;

    [ContextMenu("Show")]
    public WaitForSecondsRealtime Show()
    {
        if (fading)
        {
            return new WaitForSecondsRealtime(0);
        }

        fading = true;
        LeanTween.alphaCanvas(canvasGroup, 1, fadeTime)
            .setEase(LeanTweenType.easeInOutSine)
            .setIgnoreTimeScale(true)
            .setOnComplete(() =>
            {
                fading = false;
            });
        return new WaitForSecondsRealtime(fadeTime);
    }

    [ContextMenu("Hide")]
    public WaitForSecondsRealtime Hide()
    {
        if (fading)
        {
            return new WaitForSecondsRealtime(0);
        }

        fading = true;
        LeanTween.alphaCanvas(canvasGroup, 0, fadeTime)
            .setEase(LeanTweenType.easeInOutSine)
            .setIgnoreTimeScale(true)
            .setOnComplete(() =>
            {
                fading = false;
            });

        return new WaitForSecondsRealtime(fadeTime);
    }
}