using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Edescal.Interactables
{
    public class Interactable : MonoBehaviour, IDamageable
    {
        [field: SerializeField]
        public int Priority { get; private set; }
        public string Name => Localization.GetString(_name);

        [SerializeField]
        private string _name;
        [Space(10), SerializeField]
        private float onInteractedDelay = 0;
        [SerializeField]
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

            if (onInteractedDelay > 0)
            {
                IEnumerator OnDelay()
                {
                    var wait = new WaitForSeconds(onInteractedDelay);
                    yield return wait;
                    onInteracted?.Invoke();
                }
                StartCoroutine(OnDelay());
            }
            else onInteracted?.Invoke();
        }
    
        public virtual void ApplyDamage(int damage, object source)
        {
            print($"Hitted damageable {gameObject.name}");
        }
    }
}
