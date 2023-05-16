using UnityEngine;
using UnityEngine.Events;

namespace Edescal.Interactables
{
    public class Interactable : MonoBehaviour, IDamageable
    {
        [SerializeField]
        private bool debugThis = true;
        [SerializeField]
        private Outline outline;
        [field: SerializeField]
        public int Priority { get; private set; }
        [SerializeField]
        private UnityEvent onInteracted;

        private void Awake()
        {
            if (outline == null)
                outline = GetComponent<Outline>();
        }

        public void Focus()
        {
            if (outline != null)
                outline.enabled = true;
        }

        public void Unfocus()
        {
            if (outline != null)
                outline.enabled = false;
        }

        public virtual void Interact(Interactor interactor)
        {
            if (!enabled) return;

            if (debugThis)
            {
                print($"Interacted with {gameObject.name}");
            }

            onInteracted?.Invoke();
        }
    
        public virtual void ApplyDamage(int damage, object source)
        {
            print($"Hitted damageable {gameObject.name}");
        }
    }
}
