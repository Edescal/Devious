using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace Edescal
{
    [RequireComponent(typeof(AudioSource))]
    public class SelectableButton : Button
    {
        [Header("Settings")]
        [SerializeField] private TMP_Text buttonLabel;
        [SerializeField] private AnimatedImage animatedImage;

        [Header("Sound FX")]
        [SerializeField] private AudioClip selectedSound;
        [SerializeField] private AudioClip pressedSound;
        [SerializeField] private AudioClip hoverSound;
        private AudioSource audioSource;

        protected override void Start()
        {
            audioSource = GetComponent<AudioSource>();
            buttonLabel = GetComponentInChildren<TMP_Text>();
            animatedImage = GetComponentInChildren<AnimatedImage>();
        }

        public void SetLabel(string label)
        {
            buttonLabel.text = label;
        }

        public override void OnSelect(BaseEventData eventData)
        {
            animatedImage?.Init();
            base.OnSelect(eventData);
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            animatedImage?.Stop();
            if (selectedSound != null && this.isActiveAndEnabled)
            {
                audioSource.PlayOneShot(selectedSound);
            }
            base.OnDeselect(eventData);
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (interactable && pressedSound != null)
                {
                    audioSource.PlayOneShot(pressedSound);
                }
                base.OnPointerClick(eventData);
            }
            else
            {
                Select();
            }

        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (EventSystem.current.currentSelectedGameObject == gameObject) return;

            if (interactable && hoverSound != null)
            {
                audioSource.PlayOneShot(hoverSound);
            }

            base.OnPointerEnter(eventData);
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
