using UnityEngine;
using UnityEngine.Events;

namespace Edescal
{
    public class Health : MonoBehaviour
    {
        [SerializeField]
        private int currentHealth;
        [SerializeField]
        private int maxHealth;
        [SerializeField]
        private UnityEvent<int, int> onHealthUpdate;
        [SerializeField]
        private UnityEvent onZeroHealth;

        public int CurrentHealth
        {
            get => currentHealth;
            set
            {
                currentHealth = value;
                if (currentHealth > maxHealth)
                {
                    currentHealth = maxHealth;
                }
                else if (currentHealth <= 0)
                {
                    currentHealth = 0;
                    onZeroHealth?.Invoke();
                }
                onHealthUpdate?.Invoke(currentHealth, maxHealth);
            }
        }

        private void Start()
        {
            currentHealth = maxHealth;
            onHealthUpdate?.Invoke(currentHealth, maxHealth);
        }
    }
}