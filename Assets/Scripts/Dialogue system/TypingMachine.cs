using System;
using System.Collections;
using UnityEngine;
using TMPro;

namespace Edescal.DialogueSystem
{
    [Serializable]
    public sealed class TypingMachine
    {
        public bool isRunning { get; set; }
        public bool isPressingFaster { get; set; }

        [Space(-10), Header("-> Settings:")]
        [SerializeField] private TMP_Text UIText;
        [SerializeField] private Punctuation defaultPunctuation;
        [SerializeField] private float timeBetweenChars = 0.03f;
        [SerializeField] private float fasterTimeBtwnChars = 0.005f;

        private Coroutine coroutine;

        public void Start(string text, Punctuation punctuation, MonoBehaviour mono)
        {
            if (isRunning) return;
            isRunning = true;
            Reset(mono);
            coroutine = mono.StartCoroutine(TypingCoroutine(text, punctuation));
        }

        public void Reset(MonoBehaviour mono)
        {
            isPressingFaster = false;
            UIText.text = string.Empty;

            if(coroutine != null)
            {
                mono.StopCoroutine(coroutine);
                coroutine = null;
            }
        }

        private IEnumerator TypingCoroutine(string text, Punctuation punctuation)
        {
            UIText.text = text;
            UIText.maxVisibleCharacters = 0;
            UIText.ForceMeshUpdate();

            var info = UIText.textInfo;
            int charNumber = info.characterCount;
            int counter = 0;

            while(counter < charNumber)
            {
                int visibleCount = (counter % charNumber) + 1;
                UIText.maxVisibleCharacters = visibleCount;

                bool isLast = counter == charNumber - 1;
                if (!isLast)
                {
                    char character = info.characterInfo[counter].character;
                    char nextChar = info.characterInfo[counter + 1].character;

                    //Check for punctuation
                    float time;
                    bool shouldPause = punctuation == null ?
                        CheckPunctuation(character, nextChar, out time, defaultPunctuation) :
                        CheckPunctuation(character, nextChar, out time, punctuation);

                    if (shouldPause)
                    {
                        var pause = new WaitForSeconds(time);
                        yield return pause;
                    }

                    float waitValue = isPressingFaster ? fasterTimeBtwnChars : timeBetweenChars;
                    if (waitValue < 0.03f)
                    {
                        yield return null;
                    }
                    else
                    {
                        var wait = new WaitForSeconds(waitValue);
                        yield return wait;
                    }
                }

                counter++;
            }

            //Fin de la corrutina
            UIText.maxVisibleCharacters = UIText.textInfo.characterCount;
            isRunning = false;
            coroutine = null;
        }

        private bool CheckPunctuation(char character, char nextChar, out float timeValue, Punctuation punctuation = null)
        {
            if (punctuation != null)
            {
                float value = 0f;
                bool current = punctuation?.ContainsChar(character, out value) == true;
                bool next = punctuation?.ContainsChar(nextChar, out _) == true;
                //Si el siguiente char es el mismo no se toma en cuenta la pausa ...
                if (current && !next)
                {
                    timeValue = !isPressingFaster ? value : value / 2f;
                    return true;
                }
            }

            timeValue = 0;
            return false;
        }
    }
}