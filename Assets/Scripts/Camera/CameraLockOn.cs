using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

namespace Edescal
{
    [RequireComponent(typeof(AudioSource))]
    public class CameraLockOn : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform player;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private CameraControls controls;

        [Header("Search settings")]
        [SerializeField] private float targetDistance=7f;
        [SerializeField] private float targetTolerance = 2f;
        [SerializeField] private float viewAngle = 45f;
        [SerializeField] private LayerMask targetLayer;
        [SerializeField] private LayerMask obstaclesLayer;
        [SerializeField] private LockOnTarget currentTarget;
        [SerializeField] private LockOnTarget[] nearTargets;
        [Space(10)]
        [SerializeField] private bool targeting = false;
        [SerializeField] private bool canSwitchTarget;

        [Header("Sound FX")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip targeting_Center;
        [SerializeField] private AudioClip targeting_Cancel;
        [SerializeField] private AudioClip targeting_Object;
        [SerializeField] private AudioClip targeting_Enemy;

        [Header("Event handling")]
        [SerializeField] private UnityEvent<LockOnTarget> onTargetPerformed;
        [SerializeField] private UnityEvent onTargetNotFound;
        [SerializeField] private UnityEvent onTargetCanceled;

        private void Update()
        {
            if (targeting)
            {
                WhileLockOn();
            }
        }

        private void FixedUpdate()
        {
            FindTargets();
        }

        public void CancelTargeting()
        {
            if (!targeting) return;

            targeting = false;
            currentTarget = null;
            onTargetCanceled?.Invoke();
            audioSource.PlayOneShot(targeting_Cancel);
        }

        private void TryToTarget(InputAction.CallbackContext ctx) => TryToTarget();
        public void TryToTarget()
        {
            if (targeting)
            {
                CancelTargeting();
                return;
            }

            targeting = SelectedLockTarget(nearTargets);
            if (!targeting)
            {
                //Reset camera
                onTargetNotFound?.Invoke();
                audioSource.PlayOneShot(targeting_Center);
            }
            else
            {
                onTargetPerformed?.Invoke(currentTarget);
                if(currentTarget.Type== TargetType.Object)
                {
                    audioSource.PlayOneShot(targeting_Object);
                }
                else
                {
                    audioSource.PlayOneShot(targeting_Enemy);
                }
            }

        }

        private void FindTargets()
        {
            Collider[] targets = Physics.OverlapSphere(player.position, targetDistance, targetLayer);
            if (targets.Length > 0)
            {
                var lockOnTargets = new HashSet<LockOnTarget>();
                for(int i = 0; i < targets.Length; i++)
                {
                    var target = targets[i];
                    if(target.TryGetComponent<LockOnTarget>(out var lockOn))
                    {
                        if (!lockOn.enabled) continue;

                        bool validateTarget = ValidateTarget(lockOn.transform.position);
                        if (validateTarget)
                        {
                            lockOnTargets.Add(lockOn);
                        }
                    }
                }

                if (lockOnTargets.Count > 0)
                {
                    nearTargets = lockOnTargets.ToArray();
                    return;
                }
            }

            nearTargets = Array.Empty<LockOnTarget>();
        }

        private void WhileLockOn()
        {
            bool disabled = currentTarget == null || !currentTarget.isActiveAndEnabled;
            if (!disabled)
            {
                float distance = Vector3.Distance(player.position, currentTarget.transform.position);
                disabled = distance >= targetDistance + targetTolerance;
            }

            if (disabled)
            {
                bool nextTarget = SelectedLockTarget(nearTargets);
                if (!nextTarget)
                {
                    //No new target found
                    CancelTargeting();
                    return;
                }
                //New target found
                onTargetPerformed?.Invoke(currentTarget);
                if (currentTarget.Type == TargetType.Object)
                {
                    audioSource.PlayOneShot(targeting_Object);
                }
                else
                {
                    audioSource.PlayOneShot(targeting_Enemy);
                }
                return;
            }

            //float x = Input.GetAxis("Horizontal");
            //float y = Input.GetAxis("Vertical");
            Vector2 input = controls.CameraRotation;
            float inputMagnitude = input.sqrMagnitude;
            if (canSwitchTarget && inputMagnitude>0)
            {
                canSwitchTarget = false;
                ShiftLockOn(input);
            }
            else if(!canSwitchTarget && inputMagnitude==0)
            {
                canSwitchTarget = true;
            }
        }

        private bool ValidateTarget(Vector3 targetPosition)
        {
            //On screen
            var screenPoint = mainCamera.WorldToViewportPoint(targetPosition);
            if(!(screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1))
            {
                return false;
            }

            //No obstacle
            if (Physics.Linecast(mainCamera.transform.position, targetPosition, obstaclesLayer, QueryTriggerInteraction.Ignore))
            {
                return false;
            }

            //On player angle view
            if (viewAngle > 0)
            {
                var direction = targetPosition - player.transform.position;
                float dot = Vector3.Dot(player.forward, direction.normalized);
                if (dot > Mathf.Cos(viewAngle * Mathf.Deg2Rad))
                {
                    return true;
                }
            }

            return true;
        }
    
        private bool SelectedLockTarget(LockOnTarget[] targets)
        {
            LockOnTarget currentTarget = null;
            float currentDistance = targetDistance + targetTolerance;
            foreach(var target in targets)
            {
                if (target == null || !target.isActiveAndEnabled) continue;

                float distance = Vector3.Distance(target.transform.position, transform.position);
                if(distance < currentDistance)
                {
                    currentDistance = distance;
                    currentTarget = target;
                }
            }
            this.currentTarget = currentTarget;
            return this.currentTarget != null;
        }

        private void ShiftLockOn(Vector2 direction)
        {
            if (nearTargets.Length > 0)
            {
                var targets = nearTargets.OrderBy(x => (x.transform.position - transform.position).sqrMagnitude).ToArray();
                var toForward = InputToForward(direction);

                for (int i = 0; i < targets.Length; i++)
                {
                    var t = targets[i];
                    if (t == null || t == currentTarget) continue;

                    Vector3 toTarget = t.transform.position - currentTarget.transform.position;
                    toTarget.y = toForward.y = 0;
                    float _dot = Vector3.Dot(toTarget, toForward);
                    if (_dot > Mathf.Cos(75 * Mathf.Deg2Rad ))
                    {
                        currentTarget = t;
                        onTargetPerformed?.Invoke(currentTarget);
                        if(currentTarget.Type== TargetType.Object)
                        {
                            audioSource.PlayOneShot(targeting_Object);
                        }
                        else
                        {
                            audioSource.PlayOneShot(targeting_Enemy);
                        }
                        break;
                    }
                }
            }
        }

        private Vector3 InputToForward(Vector3 input)
        {
            var right = Vector3.ProjectOnPlane(transform.right, Vector3.up).normalized;
            var forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
            return (right * input.x + forward * input.y).normalized;
        }
    }
}