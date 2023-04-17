using System.Collections.Generic;
using UnityEngine;

namespace Edescal.Interactables
{
    public class Interactor : MonoBehaviour
    {
        public bool CanInteract { get; set; } = true;

        [SerializeField] private InputReader controls;
        [SerializeField] private bool debugThis = true;
        [SerializeField] private List<Interactable> interactables;
        [SerializeField] private Interactable currentInteractable;
        [SerializeField] private AnimatedImage interactorLabel;

        [Header("Box Cast Settings")]
        [SerializeField] private Vector3 halfBox = new Vector3(0.5f, 0.5f, 0.5f);
        [SerializeField] private float distanceCheck = 0.3f;
        [SerializeField] private float verticalOffset = 0.75f;
        [SerializeField] private LayerMask targetLayers;
        [SerializeField] private LayerMask obstacleLayers;

        private void OnEnable()
        {
            if(controls==null)
            {
                Debug.LogError($"Unassigned reference: PlayerControls in Interactor script - {gameObject.name}");
                return;
            }

            controls.onInteracted += TriggerInteraction;
        }

        private void OnDisable()
        {
            if (controls == null)
            {
                Debug.LogError($"Unassigned reference: PlayerControls in Interactor script - {gameObject.name}");
                return;
            }

            controls.onInteracted -= TriggerInteraction;
        }

        private void FixedUpdate()
        {
            FindInteractables();
        }

        private void OnDrawGizmos()
        {
            if (!debugThis) return;

            var origin = transform.position + (transform.forward * distanceCheck) + (transform.up * verticalOffset);
            Gizmos.color = interactables.Count > 0 ? Color.green : Color.red;
            Gizmos.matrix = Matrix4x4.TRS(origin, transform.rotation, transform.lossyScale);
            Gizmos.DrawWireCube(Vector3.zero, halfBox * 2);
            Gizmos.color = interactables.Count > 0 ? new Color(0, 1, 0, 0.3f) : new Color(1, 0, 0, 0.3f);
            Gizmos.DrawCube(Vector3.zero, halfBox * 2);
        }

        private void TriggerInteraction()
        {
            if (!CanInteract) return;

            if(currentInteractable != null)
            {
                currentInteractable.Interact(this);
            }
        }

        private void FindInteractables()
        {
            interactables.Clear();
            if (CanInteract && controls.PlayerMapEnabled)
            {
                var origin = transform.position + (transform.forward * distanceCheck) + (transform.up * verticalOffset);
                var hits = Physics.OverlapBox(origin, halfBox, transform.localRotation, targetLayers);
                if (hits.Length > 0)
                {
                    foreach (var hit in hits)
                    {
                        if (ValidateHit(hit, out var interactable))
                        {
                            if (!interactables.Contains(interactable))
                                interactables.Add(interactable);
                        }
                    }

                    if (interactables.Count > 0)
                    {
                        if(currentInteractable == null)
                        {
                            interactorLabel?.Init();
                        }
                        currentInteractable = SetCurrentInteractable();
                        currentInteractable.Focus();
                        return;
                    }
                }
            }

            if (currentInteractable != null)
            {
                interactorLabel?.Stop();
                currentInteractable.Unfocus();
            }
            currentInteractable = null;
        }

        private bool ValidateHit(Collider hit, out Interactable validated)
        {
            if(hit.TryGetComponent<Interactable>(out var interactable))
            {
                if (interactable.enabled)
                {
                    var playerHead = transform.position + (transform.up * 1.3f);
                    if(!Physics.Linecast(playerHead, interactable.transform.position, obstacleLayers))
                    {
                        validated = interactable;
                        return true;
                    }
                }
            }
            validated = null;
            return false;
        }

        private Interactable SetCurrentInteractable()
        {
            Interactable current = null;
            int priority = int.MinValue;
            foreach(var interactable in interactables)
            {
                if(interactable.Priority > priority)
                {
                    priority = interactable.Priority;
                    current = interactable;
                }
            }
            return current;
        }
    }
}