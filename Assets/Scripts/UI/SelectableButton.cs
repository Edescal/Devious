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
        [SerializeField] private string labelKey;
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
            this.UpdateLanguage();
            base.Start();
        }

        protected override void OnEnable()
        {
            Localization.onLanguageChanged += UpdateLanguage;
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            Localization.onLanguageChanged -= UpdateLanguage;
            base.OnDisable();
        }

        protected void UpdateLanguage()
        {
            if (!Application.isPlaying) return;
            if (buttonLabel != null)
            {
                buttonLabel.text = Localization.GetString(labelKey);
            }
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
            if (selectedSound != null && isActiveAndEnabled && interactable)
            {
                audioSource.PlayOneShot(selectedSound);
            }
            base.OnDeselect(eventData);
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (pressedSound != null && isActiveAndEnabled && interactable)
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

            if (isActiveAndEnabled && interactable && hoverSound != null)
            {
                audioSource.PlayOneShot(hoverSound);
            }

            base.OnPointerEnter(eventData);
        }

        public override void OnSubmit(BaseEventData eventData)
        {
            if (interactable && pressedSound != null)
            {
                audioSource.PlayOneShot(pressedSound);
            }

            base.OnSubmit(eventData);
        }
    }
}
