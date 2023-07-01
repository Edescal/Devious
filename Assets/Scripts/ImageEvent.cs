using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Edescal
{
    public class ImageEvent : MonoBehaviour
    {
        public static ImageEvent instance { get; private set; }
        
        public InputActionReference submit;
        public float fadeTime = 0.3f;
        private bool eventTriggered = false;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void Init(ImageEventData data)
        {
            if (eventTriggered) return;
            eventTriggered = true;
            Time.timeScale = 0;

            IEnumerator OnInit()
            {
                float c = fadeTime;
                while (c > 0)
                {
                    float t = c / fadeTime;
                    data.canvas.alpha = Mathf.Lerp(1, 0, t);
                    c -= Time.unscaledDeltaTime;
                    yield return null;
                }

                submit.action.Enable();
                bool submitted = false;
                while (!submitted)
                {
                    yield return null;
                    submitted = submit.action.WasPerformedThisFrame();
                }
                submit.action.Disable();

                c = fadeTime;
                while (c > 0)
                {
                    float t = c / fadeTime;
                    data.canvas.alpha = Mathf.Lerp(0, 1, t);
                    c -= Time.unscaledDeltaTime;
                    yield return null;
                }

                eventTriggered = false;
                Time.timeScale = 1;
            }
            StartCoroutine(OnInit());
        }
    }
}