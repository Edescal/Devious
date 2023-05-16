using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace Edescal
{
    [RequireComponent(typeof(AudioSource))]
    public class SelectableButton : Button
    {
        [Header("Decoratime image canvas")]
        [SerializeField] private TMP_Text buttonLabel;
        [SerializeField] private CanvasGroup imageCanvas;
        [SerializeField] private float fadeTime = 0.3f;

        [Header("Glow FX settings")]
        [SerializeField] private CanvasGroup glowCanvas;
        [SerializeField] private float glowTime = 0.7f;
        [SerializeField] private LeanTweenType tweenType;

        [Header("Sound FX")]
        [SerializeField] private AudioClip selectedSound;
        [SerializeField] private AudioClip pressedSound;
        private AudioSource audioSource;

        protected override void Start()
        {
            audioSource = GetComponent<AudioSource>();
            buttonLabel = GetComponentInChildren<TMP_Text>();
            imageCanvas.alpha = 0;
            glowCanvas.alpha = 0;
        }

        public void SetLabel(string label)
        {
            buttonLabel.text = label;
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);

            if (selectedSound != null && this.isActiveAndEnabled)
            {
                audioSource.PlayOneShot(selectedSound);
            }

            LeanTween.cancel(imageCanvas.gameObject);
            LeanTween.cancel(glowCanvas.gameObject);
            LeanTween.alphaCanvas(imageCanvas, 1, fadeTime)
                .setEase(tweenType);

            glowCanvas.alpha = 0;
            LeanTween.alphaCanvas(glowCanvas, 1, glowTime)
                .setEase(tweenType).setLoopPingPong();
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);

            LeanTween.cancel(imageCanvas.gameObject);
            LeanTween.cancel(glowCanvas.gameObject);
            LeanTween.alphaCanvas(imageCanvas, 0, fadeTime)
                .setEase(tweenType);
            LeanTween.alphaCanvas(glowCanvas, 0, fadeTime)
                .setEase(tweenType);
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (this.interactable && pressedSound != null)
            {
                audioSource.PlayOneShot(pressedSound);
            }

            base.OnPointerClick(eventData);
        }

        public override void OnSubmit(BaseEventData eventData)
        {
            if (pressedSound != null)
            {
                audioSource.PlayOneShot(pressedSound);
            }

            base.OnSubmit(eventData);
        }
    }
}
