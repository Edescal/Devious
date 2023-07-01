using UnityEngine;
using UnityEngine.Events;

namespace Edescal.Interactables
{
    public class Interactable : MonoBehaviour, IDamageable
    {
        [field: SerializeField]
        public string Name { get; private set; }
        [field: SerializeField]
        public int Priority { get; private set; }

        [Space(10), SerializeField]
        private UnityEvent onInteracted;
        private Outline outline;

        private void Awake()
        {
            if (outline == null)
                outline = GetComponent<Outline>();

            if (outline!=null)
                outline.enabled = false;
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

            onInteracted?.Invoke();
        }
    
        public virtual void ApplyDamage(int damage, object source)
        {
            print($"Hitted damageable {gameObject.name}");
        }
    }
}
