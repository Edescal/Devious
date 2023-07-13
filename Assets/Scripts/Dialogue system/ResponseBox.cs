using System.Collections;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

namespace Edescal.DialogueSystem
{
    public class ResponseBox : MonoBehaviour
    {
        public bool isWaiting { get; private set; }
        public int selectedResponseIndex { get; private set; }

        [SerializeField] private RectTransform buttonContainer;
        [SerializeField] private GameObject buttonPrefab;
        [SerializeField] private CanvasGroup responseCanvas;
        [SerializeField] private TMP_Text responseLabel;
        [SerializeField] private float fadeTime = 0.3f;
        private SelectableButton[] responseBtns;
        private GameObject[] buttons;
        private Coroutine coroutine;

        private void Start()
        {
            responseLabel.text = string.Empty;
            responseCanvas.alpha = 0;
            selectedResponseIndex = -1;
            isWaiting = false;
            responseBtns = new SelectableButton[3];
            buttons = new GameObject[3];
            for(int i = 0; i < 3; i++)
            {
                buttons[i] = Instantiate(buttonPrefab, buttonContainer);
                if (buttons[i] == null)
                {
                    Debug.LogError($"Response button #{i} could'n be instantiated correctly.");
                    continue;
                }

                responseBtns[i] = buttons[i].GetComponentInChildren<SelectableButton>();
                if (responseBtns[i] != null)
                {
                    int index = i;
                    responseBtns[index].interactable = false;
                    responseBtns[index].onClick.AddListener(() =>
                    {
                        foreach (var btn in responseBtns)
                            btn.interactable = false;

                        selectedResponseIndex = index;
                        IEnumerator Delay()
                        {
                            var waitAfterResp = new WaitForSeconds(fadeTime);
                            yield return waitAfterResp;
                            isWaiting = false;
                            if (coroutine != null)
                                StopCoroutine(coroutine);
                            EventSystem.current.SetSelectedGameObject(null);
                        }
                        StartCoroutine(Delay());
                    });
                }
            }
        }

        public void Show(ResponseType responseType, DialogueEvent dialogue)
        {
            if (responseType == ResponseType.NO_OPTIONS) return;

            responseLabel.text = dialogue.currentResponseMessage;
            selectedResponseIndex = -1;
            isWaiting = true;

            foreach (var go in buttons)
                go.SetActive(false);

            var respEvts = dialogue.GetResponseEvent(dialogue.currentDialogue);
            var strings = new string[] { "A", "B", "C" };
            if (respEvts != null)
            {
                for (int i = 0; i < respEvts.Length; i++)
                    strings[i] = respEvts[i].Name;
            }

            Action<int> Set = (n) =>
            {
                for (int i = 0; i < n; i++)
                {
                    responseBtns[i].interactable = false;
                    responseBtns[i].gameObject.SetActive(true);
                    responseBtns[i].SetLabel(strings[i]);
                }
            };

            switch (responseType)
            {
                case ResponseType.OK:
                    Set(1);
                    break;
                case ResponseType.OK_CANCEL:
                    Set(2);
                    break;
                case ResponseType.YES_NO_CANCEL:
                    Set(3);
                    break;
            }

            LeanTween.cancel(responseCanvas.gameObject);
            LeanTween.alphaCanvas(responseCanvas, 1, fadeTime)
                .setEase(LeanTweenType.easeInSine)
                .setOnComplete(() =>
                {
                    foreach (var r in responseBtns)
                    {
                        if (r.isActiveAndEnabled)
                            r.interactable = true;
                    }
                    if (coroutine != null)
                    {
                        StopCoroutine(coroutine);
                        coroutine = null;
                    }
                    coroutine = StartCoroutine(WaitForResponse());
                });
        }

        public void Hide()
        {
            LeanTween.cancel(responseCanvas.gameObject);
            LeanTween.alphaCanvas(responseCanvas, 0, fadeTime)
                .setEase(LeanTweenType.easeInSine);
        }

        private IEnumerator WaitForResponse()
        {
            EventSystem.current.SetSelectedGameObject(responseBtns[0].gameObject);
            //GameObject lastSelected = EventSystem.current.currentSelectedGameObject;
            while (isWaiting)
            {
                /*
                if (EventSystem.current.currentSelectedGameObject == null)
                {
                    EventSystem.current.SetSelectedGameObject(lastSelected);
                }
                lastSelected = EventSystem.current.currentSelectedGameObject;
                */
                yield return null;
            }
            coroutine = null;
        }
    }
}