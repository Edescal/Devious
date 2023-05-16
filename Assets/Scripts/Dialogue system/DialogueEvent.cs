using UnityEngine;
using UnityEngine.Events;
using Edescal.Interactables;

namespace Edescal.DialogueSystem
{
    public class DialogueEvent : Interactable
    {
        [SerializeField]
        private UnityEvent onDialogueEnd;

        [field: Header("Settings"), SerializeField]
        public Dialogue currentDialogue { get; set; }
        [SerializeField]
        private DialogueSystem system;
        private Interactor interactor;

        [Header("Event handling"), SerializeField]
        private DialogueEventWrapper[] dialogueEvents;
        
        [System.Serializable]
        private class DialogueEventWrapper
        {
            [HideInInspector]
            public string name;
            [Header("Raising events when dialogue has ended")]
            public Dialogue targetDialogue;
            public UnityEvent customEvent;

            [Header("Set custom event for each target dialogue response")]
            public string responseMessage;
            public ResponseEvent[] responseEvents;
        }

        [System.Serializable]
        public class ResponseEvent
        {
            [HideInInspector]
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

        public override void Interact(Interactor interactor)
        {
            if (currentDialogue == null || system == null) return;

            base.Interact(interactor);
            this.interactor = interactor;
            if(interactor.TryGetComponent<InputReader>(out var controls))
            {
                controls.SwitchUI(true);
            }

            system.InitDialogue(this);
        }

        public void DialogueStatus(bool success)
        {
            if (interactor.TryGetComponent<InputReader>(out var controls))
            {
                controls.SwitchUI(false);
            }

            if (success && dialogueEvents != null && dialogueEvents.Length > 0)
            {
                foreach (var events in dialogueEvents)
                {
                    if (events.targetDialogue == currentDialogue)
                    {
                        print($"Raised custom events for {currentDialogue.name}");
                        events.customEvent?.Invoke();
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

        private void OnValidate()
        {
            if (dialogueEvents == null) return;
            if (dialogueEvents.Length == 0) return;

            foreach (var customEvent in dialogueEvents)
            {
                if (customEvent.targetDialogue == null || (customEvent.targetDialogue.responseType == ResponseType.NO_OPTIONS && customEvent.responseEvents.Length > 0)) 
                {
                    customEvent.name = "There's not target dialogue assigned!";
                    customEvent.responseEvents = new ResponseEvent[0];
                    continue;
                }
                customEvent.name = $"Custom events for target: {customEvent.targetDialogue.name}";


                switch (customEvent.targetDialogue.responseType)
                {
                    case ResponseType.OK:
                        if (customEvent.responseEvents.Length == 1) continue;

                        customEvent.responseEvents = new ResponseEvent[] { new ResponseEvent() };
                        customEvent.responseEvents[0].name = $"OK response for {customEvent.targetDialogue.name}";
                        break;
                    case ResponseType.OK_CANCEL:
                        if (customEvent.responseEvents.Length == 2) continue;

                        customEvent.responseEvents = new ResponseEvent[] { new ResponseEvent(), new ResponseEvent() };
                        customEvent.responseEvents[0].name = $"OK response for {customEvent.targetDialogue.name}";
                        customEvent.responseEvents[1].name = $"CANCEL response for {customEvent.targetDialogue.name}";
                        break;
                    case ResponseType.YES_NO_CANCEL:
                        if (customEvent.responseEvents.Length == 3) continue;

                        customEvent.responseEvents = new ResponseEvent[] { new ResponseEvent(), new ResponseEvent(), new ResponseEvent() };
                        customEvent.responseEvents[0].name = $"YES response for {customEvent.targetDialogue.name}";
                        customEvent.responseEvents[1].name = $"CANCEL response for {customEvent.targetDialogue.name}"; customEvent.responseEvents[0].name = $"OK response for {customEvent.targetDialogue.name}";
                        customEvent.responseEvents[2].name = $"NO response for {customEvent.targetDialogue.name}";
                        break;
                    default:
                        if(customEvent.responseEvents.Length > 0)
                        {
                            customEvent.responseEvents = System.Array.Empty<ResponseEvent>();
                        }
                        customEvent.responseMessage = $"{customEvent.targetDialogue.name} has no responses.";
                        break;
                }
            }
        }
    }
}