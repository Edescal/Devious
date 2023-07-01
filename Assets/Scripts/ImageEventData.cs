using UnityEngine;

namespace Edescal
{
    public class ImageEventData : MonoBehaviour
    {
        private ImageEvent imageEvent;
        [field: SerializeField] public CanvasGroup canvas { get; private set; }

        private void Start()
        {
            imageEvent = ImageEvent.instance;
            if (imageEvent == null)
            {
                Debug.LogError("ImageEvent singleton not found!");
                Debug.Break();
            }
        }

        public void Send()
        {
            if (imageEvent == null) return;

            imageEvent.Init(this);
        }
    }
}