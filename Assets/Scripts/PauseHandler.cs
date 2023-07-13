using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine;

public class PauseHandler : MonoBehaviour
{
    [Header("Pause")]
    [SerializeField] private CanvasGroup pauseScreen;
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private GameObject buttonContainer;
    [SerializeField] private GameObject firstButton;
    [SerializeField] private Button[] buttons;
    [SerializeField] private InputReader input;
    [SerializeField] private SceneWarp sceneWarp;
    private bool paused = false;

    [Header("Pause")]
    [SerializeField] private AudioClip unpauseClip;
    [SerializeField] private AudioClip exitClip;
    private AudioSource audioSource;

    private void Start()
    {
        if (input == null) return;

        input.onPause += Pause;
        input.UI.Cancel.performed += CancelPause;
    }

    private void OnDestroy()
    {
        if (input == null) return;

        input.onPause -= Pause;
        input.UI.Cancel.performed -= CancelPause;
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        pauseScreen.alpha = 0;
        foreach (var b in buttons)
            b.interactable = false;
    }

    private void CancelPause(InputAction.CallbackContext context)
    {
        if (paused)
        {
            Unpause();
        }
    }

    public void Pause()
    {
        if (GameManager.inSceneTransition) return;

        paused = true;
        Time.timeScale = 0;
        LeanTween.alphaCanvas(pauseScreen, 1, fadeDuration)
            .setIgnoreTimeScale(true)
            .setEase(LeanTweenType.easeInOutSine)
            .setOnComplete(() =>
            {
                foreach (var b in buttons)
                    b.interactable = true;
                EventSystem.current.SetSelectedGameObject(firstButton);
                input.SwitchUI(true);
                pauseScreen.blocksRaycasts = true;
            });
    }

    public void Unpause()
    {
        if (GameManager.inSceneTransition) return;

        if (unpauseClip != null)
        {
            audioSource?.PlayOneShot(unpauseClip);
        }

        LeanTween.alphaCanvas(pauseScreen, 0, fadeDuration)
            .setIgnoreTimeScale(true)
            .setEase(LeanTweenType.easeInOutSine)
            .setOnStart(() =>
            {
                input.SwitchUI(false);
                foreach (var b in buttons)
                    b.interactable = false;
            })
            .setOnComplete(() =>
            {
                paused = false;
                Time.timeScale = 1;
                EventSystem.current.SetSelectedGameObject(null);
                pauseScreen.blocksRaycasts= false;
            });
    }

    public void ReturnToMainMenu()
    {
        if (exitClip != null)
        {
            audioSource?.PlayOneShot(exitClip);
        }
        GameManager.instance.ChangeScene(sceneWarp);
    }
}