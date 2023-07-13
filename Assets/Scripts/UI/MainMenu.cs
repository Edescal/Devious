using UnityEngine.EventSystems;
using UnityEngine;
using Cinemachine;

namespace Edescal
{
    public class MainMenu : MonoBehaviour
    {
        public CanvasGroup buttonsCanvas;
        public SelectableButton[] buttons;
        public SelectableButton newGameButton, langButton;

        [Header("FOV FX")]
        public CinemachineVirtualCamera virtualCamera;
        public float targetFOV = 60;
        public float duration = 2f;
        public bool ignoreTimeScale;
        public LeanTweenType curve;

        void Start()
        {
            buttonsCanvas.alpha = 0;
            foreach (var b in buttons)
                b.interactable = false;

            if (virtualCamera == null) return;

            LeanTween.value(virtualCamera.m_Lens.FieldOfView, targetFOV, duration)
                .setEase(curve)
                .setOnUpdate((float f) =>
                {
                    virtualCamera.m_Lens.FieldOfView = f;
                })
                .setOnComplete(() =>
                {
                    virtualCamera.m_Lens.FieldOfView = targetFOV;
                    LeanTween.alphaCanvas(buttonsCanvas, 1, 0.4f)
                        .setEase(curve)
                        .setOnComplete(() =>
                        {
                            foreach (var b in buttons)
                                b.interactable = true;
                            EventSystem.current.SetSelectedGameObject(newGameButton.gameObject);
                        });
                })
                .setIgnoreTimeScale(ignoreTimeScale);
        }

        public void NewGame()
        {
            GameManager.instance.NewGame();
            foreach (var b in buttons)
                b.interactable = false;
        }

        public void ChangeLanguage()
        {
            if (langButton == null) return;

            switch (Localization.CurrentLanguage)
            {
                case Languages.EN:
                    Localization.SetLanguage(Languages.ESP);
                    break;
                case Languages.ESP:
                    Localization.SetLanguage(Languages.EN);
                    break;
                default:
                    Localization.SetLanguage(Languages.ESP);
                    break;
            }
        }

        public void QuitGame()
        {
            foreach (var b in buttons)
                b.interactable = false;
            Application.Quit();
        }
    }
}