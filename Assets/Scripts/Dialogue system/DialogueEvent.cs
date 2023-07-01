using UnityEngine;
using UnityEngine.Events;
using Edescal.Interactables;

namespace Edescal.DialogueSystem
{
    public class DialogueEvent : Interactable
    {
        private DialogueSystem system;
        private Interactor interactor;

        [System.Serializable]
        private class DialogueEventWrapper
        {
            [HideInInspector]
            public string name;
            [Header("Set dialogue:")]
            public Dialogue targetDialogue;
            [Space(10)]
            public UnityEvent onDialogueEnded;
            [Header("Set response events:")]
            public string responseMessage;
            public ResponseEvent[] responseEvents;
        }

        [SerializeField]
        private UnityEvent onDialogueEnd;
        [SerializeField]
        private bool triggerAtStart;
        [field: SerializeField]
        public Dialogue currentDialogue { get; set; }
        [field: SerializeField]
        public Punctuation Punctuation { get; set; }


        [Header("Event handling"), SerializeField]
        private DialogueEventWrapper[] dialogueEvents;

        [System.Serializable]
        public class ResponseEvent
        {
            public string name;
            public UnityEvent responseEvent;
        }

        public string currentResponseMessage
        {
            get
            {
                for (int i = 0; i < dialogueEvents.Length; i++)
                {
                    if (dialogueEvents[i].targetDialogue != currentDialogue) continue;
                    if (dialogueEvents[i].targetDialogue.responseType == ResponseType.NO_OPTIONS) continue;
                    return dialogueEvents[i].responseMessage;
                }
                return $"No response message avaiable for {currentDialogue}";
            }
        }

        public ResponseEvent[] GetResponseEvent(Dialogue target)
        {
            ResponseEvent[] res=null;
            foreach(var evt in dialogueEvents)
            {
                if (evt.targetDialogue == target)
                {
                    res = evt.responseEvents;
                    break;
                }
            }
            return res;
        }

        public override void Interact(Interactor interactor)
        {
            if (currentDialogue == null || system == null) return;

            base.Interact(interactor);
            this.interactor = interactor;
            if (interactor != null)
            {
                if (interactor.TryGetComponent<InputReader>(out var controls))
                {
                    controls.SwitchUI(true);
                }
            }
            system.InitDialogue(this);
        }

        public void DialogueStatus(bool success)
        {
            if (interactor != null)
            {
                if (interactor.TryGetComponent<InputReader>(out var controls))
                {
                    controls.SwitchUI(false);
                }
            }

            if (success && dialogueEvents != null && dialogueEvents.Length > 0)
            {
                foreach (var events in dialogueEvents)
                {
                    if (events.targetDialogue == currentDialogue)
                    {
                        print($"Raised custom events for {currentDialogue.name}");
                        events.onDialogueEnded?.Invoke();
                        break;
                    }
                }
            }

            onDialogueEnd?.Invoke();
        }

        public void ResponseStatus(int index)
        {
            if (dialogueEvents == null || dialogueEvents.Length == 0) return;

            foreach (var events in dialogueEvents)
            {
                if (events.targetDialogue != currentDialogue) continue;

                if (events.responseEvents.Length > 0)
                {
                    index = Mathf.Clamp(index, 0, events.responseEvents.Length - 1);
                    print($"Raised {events.responseEvents[index].name}");
                    events.responseEvents[index].responseEvent?.Invoke();
                    break;
                }
            }
        }

        private void Start()
        {
            if (triggerAtStart)
            {
                Interact(null);
            }
        }

        private void OnValidate()
        {
            if (system == null)
            {
                system = GameObject.FindObjectOfType<DialogueSystem>();
            }

            if (dialogueEvents.Length == 0) return;

            foreach (var customEvent in dialogueEvents)
            {
                //Si no tiene diálogo asignado
                if (customEvent.targetDialogue == null) 
                {
                    customEvent.name = "There's not target dialogue assigned!";
                    customEvent.responseEvents = System.Array.Empty<ResponseEvent>();
                    continue;
                }

                //Si el diálogo explícitamente no puede tener respuestas
                if (customEvent.targetDialogue.responseType == ResponseType.NO_OPTIONS && customEvent.responseEvents.Length > 0)
                {
                    customEvent.name = $"Dialogue {customEvent.targetDialogue.name} doesn't accept responses!";
                    customEvent.responseEvents = System.Array.Empty<ResponseEvent>();
                    continue;
                }

                //En otro caso...
                customEvent.name = $"Custom events - {customEvent.targetDialogue.name}";
                switch (customEvent.targetDialogue.responseType)
                {
                    case ResponseType.OK:
                        if (customEvent.responseEvents.Length != 1)
                        {
                            customEvent.responseEvents = new ResponseEvent[] { new ResponseEvent() };
                            customEvent.responseEvents[0].name = $"OK response for {customEvent.targetDialogue.name}";
                        }
                        break;
                    case ResponseType.OK_CANCEL:
                        if (customEvent.responseEvents.Length != 2)
                        {
                            customEvent.responseEvents = new ResponseEvent[] { new ResponseEvent(), new ResponseEvent() };
                            customEvent.responseEvents[0].name = $"OK response for {customEvent.targetDialogue.name}";
                            customEvent.responseEvents[1].name = $"CANCEL response for {customEvent.targetDialogue.name}";
                        }
                        break;
                    case ResponseType.YES_NO_CANCEL:
                        if (customEvent.responseEvents.Length != 3)
                        {
                            customEvent.responseEvents = new ResponseEvent[] { new ResponseEvent(), new ResponseEvent(), new ResponseEvent() };
                            customEvent.responseEvents[0].name = $"YES response for {customEvent.targetDialogue.name}";
                            customEvent.responseEvents[1].name = $"CANCEL response for {customEvent.targetDialogue.name}"; customEvent.responseEvents[0].name = $"OK response for {customEvent.targetDialogue.name}";
                            customEvent.responseEvents[2].name = $"NO response for {customEvent.targetDialogue.name}";
                        }
                        break;
                }
            }
        }
    }
}