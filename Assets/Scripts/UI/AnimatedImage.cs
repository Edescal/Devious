using UnityEngine;
using TMPro;

namespace Edescal
{
    public class AnimatedImage : MonoBehaviour
    {
        public float tweenTime = 0.2f;
        public float glowTime = 0.6f;
        public CanvasGroup canvasGroup;
        public CanvasGroup glowCanvas;
        public TMP_Text label;

        private void Start()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            label = GetComponentInChildren<TMP_Text>();
        }

        public void SetLabel(string msg)
        {
            if (label == null) return;
            label.text = msg;
        }

        public void Init()
        {
            LeanTween.cancel(gameObject);
            LeanTween.alphaCanvas(canvasGroup, 1, tweenTime)
                .setEase(LeanTweenType.easeInSine);

            LeanTween.cancel(glowCanvas.gameObject);
            LeanTween.alphaCanvas(glowCanvas, 1, glowTime)
                .setEase(LeanTweenType.easeInSine)
                .setLoopPingPong(-1);
        }

        public void Stop()
        {
            LeanTween.cancel(gameObject);
            LeanTween.alphaCanvas(canvasGroup, 0, tweenTime)
                .setEase(LeanTweenType.easeInSine);

            LeanTween.cancel(glowCanvas.gameObject);
            LeanTween.alphaCanvas(glowCanvas, 0, glowTime)
                .setEase(LeanTweenType.easeInSine);
        }
    }
}