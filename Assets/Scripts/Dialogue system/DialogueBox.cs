using UnityEngine;
using UnityEngine.UI;

namespace Edescal.DialogueSystem
{
    public class DialogueBox : MonoBehaviour
    {
        [SerializeField] private CanvasGroup boxCanvas;
        [SerializeField] private Image iconImg, glowImg;
        [SerializeField] private LeanTweenType tweenType;
        [Space(10)]
        [SerializeField] private CanvasGroup iconCanvas;
        [SerializeField] private float fadeTime = 0.3f;
        [Space(10)]
        [SerializeField] private CanvasGroup glowCanvas;
        [SerializeField] private float glowFadeTime = 0.6f;

        [Space(13), SerializeField]
        private DialogueIcon nextIcon, doneIcon;

        [System.Serializable]
        public struct DialogueIcon
        {
            public Sprite iconSprite;
            public Sprite glowSprite;
        }

        public float FadeTime => fadeTime;

        private void Start()
        {
            boxCanvas.alpha = 0;
            iconCanvas.alpha = 0;
            glowCanvas.alpha = 0;
        }

        public void ShowCanvas()
        {
            LeanTween.alphaCanvas(boxCanvas, 1, fadeTime)
                .setEase(tweenType);
        }

        public void HideCanvas()
        {
            LeanTween.alphaCanvas(boxCanvas, 0, fadeTime)
                .setEase(tweenType);
        }

        public void ShowIcon(bool done = false)
        {
            if (done)
            {
                iconImg.sprite = doneIcon.iconSprite;
                glowImg.sprite = doneIcon.glowSprite;
            }
            else
            {
                iconImg.sprite = nextIcon.iconSprite;
                glowImg.sprite = nextIcon.glowSprite;
            }

            LeanTween.cancel(iconCanvas.gameObject);
            LeanTween.alphaCanvas(iconCanvas, 1, glowFadeTime)
                .setEase(tweenType);

            LeanTween.cancel(glowCanvas.gameObject);
            LeanTween.alphaCanvas(glowCanvas, 1, glowFadeTime)
                .setEase(tweenType)
                .setLoopPingPong();

        }

        public void HideIcon(bool done = false)
        {
            LeanTween.cancel(iconCanvas.gameObject);
            LeanTween.alphaCanvas(iconCanvas, 0, glowFadeTime)
                .setEase(tweenType);

            LeanTween.cancel(glowCanvas.gameObject);
            LeanTween.alphaCanvas(glowCanvas, 0, glowFadeTime)
                .setEase(tweenType);

        }
    }
}