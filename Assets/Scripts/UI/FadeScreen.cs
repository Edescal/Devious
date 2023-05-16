using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Edescal;

public class FadeScreen : MonoBehaviour
{
    public bool fading { get; private set; } = false;

    [SerializeField]
    private CanvasGroup canvasGroup;
    [SerializeField]
    private float fadeTime = 0.75f;

    [ContextMenu("Show")]
    public void Show()
    {
        if (fading) return;
        fading = true;

        LeanTween.alphaCanvas(canvasGroup, 1, fadeTime)
            .setEase(LeanTweenType.easeInOutSine)
            .setIgnoreTimeScale(true)
            .setOnComplete(() =>
            {
                fading = false;
            });
    }

    [ContextMenu("Hide")]
    public void Hide()
    {
        if (fading) return;
        fading = true;

        LeanTween.alphaCanvas(canvasGroup, 0, fadeTime)
            .setEase(LeanTweenType.easeInOutSine)
            .setIgnoreTimeScale(true)
            .setOnComplete(() =>
            {
                fading = false;
            });
    }
}
