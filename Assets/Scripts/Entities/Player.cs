using System;
using UnityEngine;
using Edescal.Interactables;

namespace Edescal
{
    public class Player : MonoBehaviour, IEntity
    {
        [field: SerializeField] public Inventory Inventory { get; private set; }
        [field: SerializeField] public Health Health { get; private set; }
        [field: SerializeField] public AnimatorManager AnimatorManager { get; private set; }
        [field: SerializeField] public ThirdPersonController ThirdPersonController { get; private set; }
        [field: SerializeField] public ThirdPersonCamera ThirdPersonCamera { get; private set; }
        [field: SerializeField] public CameraLockOn CameraLockOn { get; private set; }
        [field: SerializeField] public Interactor Interactor { get; private set; }

        public string animStateDebug="";

        public void ApplyDamage(int damage, object source)
        {
            print($"Damage of {damage} applied to {gameObject.name} from {source}");
            if (Health == null) return;

            Health.CurrentHealth -= damage;
        }

        public void DebugAnimCallback()
        {
            Action start = () =>
            {
                Debug.Log("Invoke start anim");
                ThirdPersonController.CanMove = false;
                Interactor.CanInteract = false;
            };
            Action end = () =>
            {
                Debug.Log("Invoke end anim");
                ThirdPersonController.CanMove = true;
                Interactor.CanInteract = true;
            };
            AnimatorManager.Play(animStateDebug, 0.25f, 0, true, start, end);
        }
    }
}