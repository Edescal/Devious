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
    public void Show(out float time)
    {
        if (fading)
        {
            time = 0;
            return;
        }

        fading = true;
        time = fadeTime;
        LeanTween.alphaCanvas(canvasGroup, 1, fadeTime)
            .setEase(LeanTweenType.easeInOutSine)
            .setIgnoreTimeScale(true)
            .setOnComplete(() =>
            {
                fading = false;
            });
    }

    [ContextMenu("Hide")]
    public void Hide(out float time)
    {
        if (fading)
        {
            time = 0;
            return;
        }

        fading = true;
        time = fadeTime;
        LeanTween.alphaCanvas(canvasGroup, 0, fadeTime)
            .setEase(LeanTweenType.easeInOutSine)
            .setIgnoreTimeScale(true)
            .setOnUpdate((float f) => print("Fading"))
            .setOnComplete(() =>
            {
                fading = false;
            });
    }
}
