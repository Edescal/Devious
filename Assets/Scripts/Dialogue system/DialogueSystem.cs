using System;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine;

namespace Edescal.DialogueSystem
{
    public class DialogueSystem : MonoBehaviour
    {
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
        [SerializeField] private InputReader input;

        [Header("Sound FX")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip dialogueInput, nextDialogue, stopDialogue;
        private WaitForSeconds waitStart, waitBetween, waitBeforeResponses, waitToEnd;
        private Coroutine coroutine;

        private void Awake()
        {
            StopDialogue();
            waitStart = new WaitForSeconds(timeToStart);
            waitBetween = new WaitForSeconds(timeBetweenDialogues);
            waitBeforeResponses = new WaitForSeconds(timeBeforeResponses);
            waitToEnd = new WaitForSeconds(timeToEnd);
            Debug.Log("# Dialogue system initiated!");
        }

        public void StartDialogue(DialogueEvent @event)
        {
            isRunning = true;
            abort = false;

            typingMachine.Reset(this);

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
            coroutine = StartCoroutine(DialogueSequence(@event));
        }

        public void StopDialogue()
        {
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
            Dialogue dialogue = dialogueEvent.currentDialogue;
            string[] dialogueTexts = dialogue.GetDialogues();
            yield return waitStart;

            //Si no hay diálogos se salta directo a las respuestas...
            if (dialogueTexts.Length > 0)
            {
                dialogueBox.ShowCanvas();
                for (int i = 0; i < dialogueTexts.Length; i++)
                {
                    bool isLastLine = i == dialogueTexts.Length - 1;
                    yield return waitBetween;

                    var routine = CheckCurrentLine(dialogueTexts[i], dialogueEvent.Punctuation, isLastLine);
                    yield return routine;

                    var waitInput = CheckForInput(isLastLine);
                    yield return waitInput;
                }
                dialogueBox.HideCanvas();
                dialogueEvent.CustomEvent();
            }

            //Si el dialogo tiene respuestas...
            if (dialogue.responseType != ResponseType.NO_OPTIONS)
            {
                yield return waitBeforeResponses;
                responseBox.Show(dialogue.responseType, dialogueEvent);
                while (responseBox.isWaiting)
                {
                    yield return null;
                }
                responseBox.Hide();
                dialogueEvent.ResponseSelected(responseBox.selectedResponseIndex);
            }

            yield return waitToEnd;
            
            dialogueEvent.DialogueEnd();
            StopDialogue();
        }

        private IEnumerator CheckCurrentLine(string text, Punctuation data, bool isLast)
        {
            typingMachine.Start(text, data, this);
            Action<InputAction.CallbackContext> forceEnd = ctx => typingMachine.forceEnd = true;

            input.UI.Cancel.canceled += forceEnd;
            while (typingMachine.isRunning)
            {
                typingMachine.isPressingFaster = input.UI.Submit.IsPressed();
                yield return null;
            }
            input.UI.Cancel.canceled -= forceEnd;

            AudioClip clip = isLast ? stopDialogue : nextDialogue;
            if (clip != null)
            {
                audioSource?.PlayOneShot(clip);
            }
        }

        private IEnumerator CheckForInput(bool isLast)
        {
            dialogueBox.ShowIcon(isLast);
            var wait = new WaitForSeconds(dialogueBox.FadeTime);
            yield return wait;

            //mostrar icono de input
            bool pressedInput = false;
            while (!pressedInput)
            {
                yield return null;
                if (input.UI.Submit.WasPerformedThisFrame() || input.UI.Cancel.WasPerformedThisFrame())
                {
                    pressedInput = true;
                    if (audioSource != null && dialogueInput != null)
                        audioSource.PlayOneShot(dialogueInput);
                }
            }
            //quitar icono de input
            dialogueBox.HideIcon();
            wait = new WaitForSeconds(dialogueBox.FadeTime);
            yield return wait;
        }
        /*
        private IEnumerator CheckForAbort(DialogueEvent @event)
        {
            while (isRunning)
            {
                yield return null;
                if (abort)
                {
                    typingMachine.isRunning = false;
                    @event.DialogueStatus(false);
                    dialogueBox.HideCanvas();
                    dialogueBox.HideIcon();
                    StopDialogue();
                    yield break;
                }
            }

            @event.DialogueStatus(true);
        }
        */
    }
}