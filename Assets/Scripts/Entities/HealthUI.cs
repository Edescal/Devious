using UnityEngine;
using UnityEngine.UI;

namespace Edescal
{
    public class HealthUI : MonoBehaviour
    {
        [SerializeField] 
        private Slider slider;
        [SerializeField] 
        private Image img;
        [SerializeField] 
        private Gradient color;

        public void UpdateHealth(int currentHealth, int maxHealth)
        {
            float t = (float)currentHealth / (float)maxHealth;
            if (t < 0)
            {
                t = 0;
            }

            LeanTween.cancel(gameObject);
            LeanTween.value(gameObject, slider.value, t, 0.3f)
                .setEase(LeanTweenType.easeInQuad)
                .setOnUpdate((float f) =>
                {
                    img.color = color.Evaluate(f);
                    slider.value = f;
                }).setOnComplete(() => slider.value = t);
            
        }
    }
}