using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace Edescal.DialogueSystem
{
    public class ResponseBox : MonoBehaviour
    {
        public bool isWaiting { get; private set; }
        public int selectedResponseIndex { get; private set; }

        [SerializeField] private CanvasGroup responseCanvas;
        [SerializeField] private TMP_Text responseLabel;
        [SerializeField] private float fadeTime = 0.3f;
        [Space(10)]
        [SerializeField] 
        private Button[] responseBtns; /* { "Sí", "Cancelar", "No" } */
        private Coroutine coroutine;

        private void Start()
        {
            responseLabel.text = string.Empty;
            responseCanvas.alpha = 0;
            selectedResponseIndex = -1;
            isWaiting = false;

            for (int i = 0; i < responseBtns.Length; i++)
            {
                int index = i;
                responseBtns[index].interactable = false;
                responseBtns[index].onClick.AddListener(() =>
                {
                    selectedResponseIndex = index;
                    isWaiting = false;
                });

            }
        }

        public void Show(ResponseType responseType, string message)
        {
            if (responseType == ResponseType.NO_OPTIONS) return;

            responseLabel.text = message;
            selectedResponseIndex = -1;
            isWaiting = true;

            Action<Button, bool> act = (btn, b) =>
            {
                btn.interactable = b;
                btn.gameObject.SetActive(b);
            };

            switch (responseType)
            {
                case ResponseType.OK:
                    act(responseBtns[0], true);
                    act(responseBtns[1], false);
                    act(responseBtns[2], false);
                    break;
                case ResponseType.OK_CANCEL:
                    act(responseBtns[0], true);
                    act(responseBtns[1], false);
                    act(responseBtns[2], true);

                    break;
                case ResponseType.YES_NO_CANCEL:
                    act(responseBtns[0], true);
                    act(responseBtns[1], true);
                    act(responseBtns[2], true);
                    break;
            }

            LeanTween.cancel(responseCanvas.gameObject);
            LeanTween.alphaCanvas(responseCanvas, 1, fadeTime)
                .setEase(LeanTweenType.easeInSine);

            if(coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
            coroutine = StartCoroutine(WaitForResponse());
        }

        public void Hide()
        {
            foreach(var btn in responseBtns)
            {
                btn.interactable = false;
            }

            LeanTween.cancel(responseCanvas.gameObject);
            LeanTween.alphaCanvas(responseCanvas, 0, fadeTime)
                .setEase(LeanTweenType.easeInSine);
        }

        private IEnumerator WaitForResponse()
        {
            EventSystem.current.SetSelectedGameObject(responseBtns[0].gameObject);
            GameObject lastSelected = EventSystem.current.currentSelectedGameObject;
            while (isWaiting)
            {
                if (EventSystem.current.currentSelectedGameObject == null)
                {
                    EventSystem.current.SetSelectedGameObject(lastSelected);
                }
                lastSelected = EventSystem.current.currentSelectedGameObject;
                yield return null;
            }
            coroutine = null;
        }
    }
}