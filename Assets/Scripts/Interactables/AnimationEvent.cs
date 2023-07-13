using UnityEngine;
using UnityEngine.Events;

namespace Edescal.Interactables
{
    public class AnimationEvent : Interactable
    {
        [SerializeField]
        private UnityEvent onAnimationEnded;
        [SerializeField]
        private string state;

        public override void Interact(Interactor interactor)
        {
            if(interactor.TryGetComponent<AnimatorManager>(out var animator))
            {
                var controller = interactor.GetComponent<ThirdPersonController>();
                System.Action start = () =>
                {
                    interactor.CanInteract = false;
                    controller.CanMove = false;
                    base.Interact(interactor);
                };
                System.Action end = () =>
                {
                    interactor.CanInteract = true;
                    controller.CanMove = true;
                    onAnimationEnded?.Invoke();
                };
                animator.Play(state, 0.25f, 0, true, start, end);
            }
        }
    }
}
