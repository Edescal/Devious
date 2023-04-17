using UnityEngine;

namespace Edescal
{
    public class TargetLockOnIcon : MonoBehaviour
    {
        public RectTransform rect;
        public RectTransform canvas;
        public LockOnTarget target;
        new public Camera camera;

        public float tweenTime = 0.2f;
        public float glowTime = 0.6f;
        public CanvasGroup canvasGroup;
        public CanvasGroup glowCanvas;

        private void Start()
        {
            camera = Camera.main;
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
        }

        public void Init()
        {
            LeanTween.cancel(gameObject);
            LeanTween.alphaCanvas(canvasGroup, 1, tweenTime)
                .setEase(LeanTweenType.easeInSine);

            LeanTween.cancel(glowCanvas.gameObject);
            glowCanvas.alpha = 0;
            LeanTween.alphaCanvas(glowCanvas, 1, glowTime)
                .setEase(LeanTweenType.easeInSine)
                .setOnUpdate((float f)=> {
                    Vector2 position = camera.WorldToScreenPoint(target.transform.position);
                    position.x *= canvas.rect.width / (float)camera.pixelWidth;
                    position.y *= canvas.rect.height / (float)camera.pixelHeight;
                    rect.anchoredPosition = position - canvas.sizeDelta / 2f;

                })
                .setLoopPingPong(-1);
        }

        public void Stop()
        {
            LeanTween.cancel(gameObject);
            LeanTween.alphaCanvas(canvasGroup, 0, tweenTime)
                .setEase(LeanTweenType.easeInSine);

            LeanTween.cancel(glowCanvas.gameObject);
            LeanTween.alphaCanvas(glowCanvas, 0, glowTime)
                .setEase(LeanTweenType.easeInSine);
        }

        public void SetTarget(Transform t)
        {
            if (t.TryGetComponent<LockOnTarget>(out var target))
            {
                SetTarget(target);
            }
        }

        public void SetTarget(LockOnTarget target)
        {
            if (!target.enabled && !target.isActiveAndEnabled) return;

            this.target = target;

            Init();
        }

        public void UnsetTarget()
        {
            this.target = null;

            Stop();
        }
    }
}
