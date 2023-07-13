using System.Collections;
using UnityEngine.UI;
using UnityEngine;

namespace Edescal.UI
{
    public class ScreenToRenderTexture : MonoBehaviour
    {
        public Camera mainCamera;
        public RenderTexture renderTexture;
        public RectTransform rectTransform;
        public Image image;
        public Vector2Int pngSize;
        public string pngPath;

        void Start()
        {
            mainCamera = Camera.main;
            print(renderTexture.sRGB);
        }

        [ContextMenu("Snapshot")]
        public void Snapshot()
        {
            Rect rect = new Rect(0, 0, renderTexture.width, renderTexture.height);
            Sprite sprite = Sprite.Create(getSnapshot(), rect, new Vector2(1, 1));
            image.sprite = sprite;
        }

        public Texture2D getSnapshot()
        {
            Texture2D screenshot = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);

            mainCamera.targetTexture = renderTexture;
            mainCamera.Render();
            RenderTexture.active = renderTexture;

            Graphics.CopyTexture(renderTexture, screenshot);

            mainCamera.targetTexture = null;
            RenderTexture.active = null;

            return screenshot;
        }

        [ContextMenu("Capture to PNG")]
        public void SavePNG()
        {
            IEnumerator Saving()
            {
                Rect rect = new Rect(0, 0, pngSize.x, pngSize.y);
                Texture2D screenshot = new Texture2D(pngSize.x, pngSize.y, TextureFormat.RGB24, false);
                yield return new WaitForEndOfFrame();
                screenshot.ReadPixels(rect, 0, 0);
                screenshot.Apply();

                byte[] bytes = screenshot.EncodeToPNG();
                System.IO.File.WriteAllBytes($"{Application.dataPath}/Screenshots/Screen_{System.DateTime.UtcNow.ToString()}.png", bytes);
            }
            StartCoroutine(Saving());
        }

        public void Effect()
        {
            IEnumerator Sequence()
            {
                Time.timeScale = 0;
                Snapshot();

                image.enabled = true;
                var complete = new Vector2(Screen.width, Screen.height);
                rectTransform.sizeDelta = complete;
                
                var wait = new WaitForSecondsRealtime(1);
                yield return wait;

                var half = complete / 2;
                LeanTween.size(rectTransform, half, 0.5f)
                    .setEase(LeanTweenType.easeOutSine)
                    .setIgnoreTimeScale(true);

                wait = new WaitForSecondsRealtime(0.5f);
                yield return wait;

                wait = new WaitForSecondsRealtime(1);
                yield return wait;
                image.enabled = false;

                Time.timeScale = 1;
            }
            StartCoroutine(Sequence());
        }
    }
}