using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine;

namespace Edescal.DialogueSystem
{
    public class DialogueSystem : MonoBehaviour
    {
        public Language language;
        public bool isRunning { get; private set; }
        public bool abort { get; set; }

        [Header("References")]
        [SerializeField] private DialogueBox dialogueBox;
        [SerializeField] private ResponseBox responseBox;
        [SerializeField] private TypingMachine typingMachine;

        [Header("Timing settings")]
        [SerializeField] private float timeToStart = 0.5f;
        [SerializeField] private float timeBetweenDialogues = 0.5f;
        [SerializeField] private float timeBeforeResponses = 1f;
        [SerializeField] private float timeToEnd = 1f;

        [Header("Input binding")]
        [SerializeField] private InputActionReference submit;

        [Header("Sound FX")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip dialogueInput;
        [SerializeField] private AudioClip nextDialogue;
        [SerializeField] private AudioClip stopDialogue;
        private Coroutine coroutine, secondaryCo;

        private void Start()
        {
            StopDialogue();
            secondaryCo = null;
        }

        public void InitDialogue(DialogueEvent @event)
        {
            if (isRunning) return;
            isRunning = true;
            abort = false;
            submit.action.Enable();

            typingMachine.Reset(this);
            
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
            coroutine = StartCoroutine(DialogueSequence(@event));

            if (secondaryCo != null)
            {
                StopCoroutine(secondaryCo);
                secondaryCo = null;
            }
            secondaryCo = StartCoroutine(CheckForAbort(@event));
        }

        public void StopDialogue()
        {
            submit.action.Disable();
            isRunning = false;
            abort = false;

            typingMachine.Reset(this);

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }

        private IEnumerator DialogueSequence(DialogueEvent dialogueEvent)
        {
            var waitStart = new WaitForSeconds(timeToStart);
            var waitBetween = new WaitForSeconds(timeBetweenDialogues);
            var waitBeforeResponses = new WaitForSeconds(timeBeforeResponses);
            var waitToEnd = new WaitForSeconds(timeToEnd);

            Dialogue dialogue = dialogueEvent.currentDialogue;
            string[] dialogueTexts = dialogue.dialogues;

            yield return waitStart;
            dialogueBox.ShowCanvas();

            for (int i = 0; i < dialogueTexts.Length; i++)
            {
                bool isLastLine = i == dialogueTexts.Length - 1;
                yield return waitBetween;

                var routine = CheckCurrentLine(dialogueTexts[i], isLastLine);
                yield return routine;

                var waitInput = CheckForInput(isLastLine && dialogue.responseType != ResponseType.NO_OPTIONS);
                yield return waitInput;
            }
            
            dialogueBox.HideCanvas();

            //Si el dialogo tiene respuestas...
            if (dialogue.responseType != ResponseType.NO_OPTIONS)
            {
                yield return waitBeforeResponses;
                responseBox.Show(dialogue.responseType, dialogueEvent.currentResponseMessage);
                while (responseBox.isWaiting)
                {
                    yield return null;
                }
                responseBox.Hide();
            }

            yield return waitToEnd;
            StopDialogue();
        }

        private IEnumerator CheckCurrentLine(string text, bool isLast)
        {
            typingMachine.Start(text, this);

            while (typingMachine.isRunning)
            {
                typingMachine.isPressingFaster = submit.action.IsPressed();
                yield return null;
            }

            AudioClip clip = isLast ? stopDialogue : nextDialogue;
            if (clip != null)
            {
                audioSource?.PlayOneShot(clip);
            }
        }

        private IEnumerator CheckForInput(bool isLast)
        {
            dialogueBox.ShowIcon(isLast);
            //mostrar icono de input
            bool pressedInput = false;
            while (!pressedInput)
            {
                yield return null;
                if (submit.action.WasPerformedThisFrame())
                {
                    pressedInput = true;
                    if (audioSource != null && dialogueInput != null)
                        audioSource.PlayOneShot(dialogueInput);
                }
            }
            //quitar icono de input
            dialogueBox.HideIcon();
        }
    
        private IEnumerator CheckForAbort(DialogueEvent @event)
        {
            while (isRunning)
            {
                yield return null;
                if (abort)
                {
                    typingMachine.isRunning = false;
                    @event.ResponseStatus(responseBox.selectedResponseIndex);
                    @event.DialogueStatus(false);
                    this.StopDialogue();
                    dialogueBox.HideCanvas();
                    dialogueBox.HideIcon();
                    yield break;
                }
            }
            @event.ResponseStatus(responseBox.selectedResponseIndex);
            @event.DialogueStatus(true);
        }
    }
}