using System;
using System.Collections;
using UnityEngine;

namespace Edescal
{
    [RequireComponent(typeof(CameraControls))]
    public class ThirdPersonCamera : MonoBehaviour
    {
        [field:SerializeField, Tooltip("Enable/Disable input")]
        public bool CanMove { get; set; } = true;

        [Header("References")]
        [SerializeField]
        private Transform mainCamera;
        [SerializeField]
        private Transform player;
        CameraControls input;

        [Header("Camera controller settings")]
        [Range(0f, 0.3f), SerializeField]
        private float cameraSmoothness = 0.1f;
        [SerializeField, Range(0f, 10f)]
        private float cameraDistance = 3f;
        [Range(0, 360), SerializeField]
        private int rotationSpeed = 260;
        [Range(0f, 0.1f), SerializeField]
        private float acceleration = 0.1f;
        [Range(1, 10), SerializeField]
        private int zoomSensitivity = 3;
        [SerializeField]
        private Vector2 cameraAngleLimit = new Vector2(-30, 60);
        [SerializeField]
        private Vector2 zoomRange = new Vector2(1, 5);
        float x_damp = 0;
        float y_damp = 0;
        float zoom_damp = 0;
        Vector3 cam_velocity = Vector3.zero;
        Vector2 currentRotation = Vector2.zero;

        [Header("Collision settings")]
        [SerializeField] 
        private LayerMask ignoreCollisionMask;
        [SerializeField] 
        private float collisionOffsetValue = 0.5f;
        [SerializeField] 
        private float cameraCollisionRadius = 0.5f;
        [SerializeField] 
        private float minDistanceToPlayer = 0.2f;

        [Header("Targeting settings")]
        [SerializeField] 
        private LockOnTarget target;
        [Range(0, 360), SerializeField] 
        private int angleLimit = 45;
        [Range(0f, 10f), SerializeField] 
        private float readjustmentSpeed = 4;
        [Range(0, 720), SerializeField]
        private int readjustmentAngleSpeed = 45;
        float readjustment_damp;
        bool readjusting;

        private void Start()
        {
            input = GetComponent<CameraControls>();
            SetRotation(transform.rotation);
        }

        private void OnValidate()
        {
            if(zoomRange.x < 0)
            {
                zoomRange.x = 0;
            }
            else if(zoomRange.x > zoomRange.y)
            {
                zoomRange.y = zoomRange.x;
            }
            else if (zoomRange.y < zoomRange.x)
            {
                zoomRange.y = zoomRange.x;
            }
            else if(zoomRange.y > 10)
            {
                zoomRange.y = 10;
            }

            cameraDistance = Mathf.Clamp(cameraDistance, zoomRange.x, zoomRange.y);
            mainCamera.localPosition = new Vector3(0, 0, -cameraDistance);
        }

        private void LateUpdate()
        {
            var followTarget = target != null ? (player.position + target.transform.position) / 2f : player.position;
            transform.position = Vector3.SmoothDamp(transform.position, followTarget, ref cam_velocity, cameraSmoothness);

            if (target == null && !readjusting)
            {
                float x = input.CameraRotation.x;
                float y = input.CameraRotation.y;

                float ref_x_damp = 0, ref_y_damp = 0;
                x_damp = Mathf.SmoothDamp(x_damp, CanMove ? x : 0, ref ref_x_damp, acceleration);
                y_damp = Mathf.SmoothDamp(y_damp, CanMove ? y : 0, ref ref_y_damp, acceleration);

                currentRotation.y += x_damp * rotationSpeed * Time.deltaTime;
                currentRotation.x -= y_damp * rotationSpeed * Time.deltaTime;
                currentRotation.x = Mathf.Clamp(currentRotation.x, cameraAngleLimit.x, cameraAngleLimit.y);

                transform.localRotation = Quaternion.Euler(currentRotation);
            }
            else if (target != null)
            {
                readjusting = false;
                if(!target.enabled || !target.isActiveAndEnabled)
                {
                    target = null;
                }
                else
                {
                    LookAtTarget(target.transform.position);
                }
            }

            //Zoom
            float ref_zoom = 0;
            float zoomValue = input.ZoomValue;

            zoom_damp = Mathf.SmoothDamp(zoom_damp, zoomValue, ref ref_zoom, acceleration);
            cameraDistance += zoom_damp * zoomSensitivity * Time.deltaTime;
            cameraDistance = Mathf.Clamp(cameraDistance, zoomRange.x, zoomRange.y);

            //Collision check
            if (mainCamera == null) return;

            //Adjust Z-Axis position
            float finalDistance = cameraDistance;
            if (Physics.SphereCast(transform.position + (transform.forward * cameraCollisionRadius), cameraCollisionRadius, -transform.forward, out var hit, cameraDistance, ~ignoreCollisionMask, QueryTriggerInteraction.Ignore))
            {
                finalDistance = Vector3.Distance(transform.position, hit.point);
            }

            finalDistance -= collisionOffsetValue;
            if (-finalDistance > -minDistanceToPlayer)
            {
                finalDistance = minDistanceToPlayer;
            }

            //Set the final adjusted position
            mainCamera.localPosition = new Vector3(0, 0, -finalDistance);

            /*
             *  Visual debug: collision-check
             * 
             *  Debug.DrawRay(transform.position, -(transform.forward * finalDistance), Color.greem)
             *  Debug.DrawRay(origin, transform.up, Color.green);
             *  Debug.DrawRay(origin, transform.right * crossDistance, Color.blue);
             *  Debug.DrawRay(origin, -transform.right * crossDistance, Color.yellow);
             */
        }

        private void LookAtTarget(Vector3 target)
        {
            var mid = (player.position + target) / 2f;
            var playerToTarget = Vector3.ProjectOnPlane((target - player.position), Vector3.up).normalized;
            var camToMid = Vector3.ProjectOnPlane((mid - mainCamera.position), Vector3.up).normalized;

            var rightLerp = Quaternion.AngleAxis(angleLimit, Vector3.up) * playerToTarget;
            var leftLerp = Quaternion.AngleAxis(-angleLimit, Vector3.up) * playerToTarget;
            float dot = Vector3.Dot(playerToTarget, camToMid);

            /*
             *  Visual debug: targeting re-adjustment
             * 
             *  Debug.DrawRay(mid, playerToTarget, Color.red);
             *  Debug.DrawRay(mid, camToMid, Color.yellow);
             *  Debug.DrawRay(mid, rightLerp, Color.red);
             *  Debug.DrawRay(mid, leftLerp, Color.red);
             *  print(Mathf.Acos(dot) * Mathf.Rad2Deg );
             */

            if (dot < Mathf.Cos(angleLimit * Mathf.Deg2Rad))
            {
                readjustment_damp = 1;
            }
            else
            {
                readjustment_damp = 0;
            }

            bool playerIsAtRight = transform.InverseTransformPoint(player.position).x >= 0;
            var rot = Quaternion.LookRotation(playerIsAtRight ? rightLerp : leftLerp, Vector3.up);
            var slerp = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * readjustmentSpeed * readjustment_damp);

            var sideVector = playerIsAtRight ?
                Quaternion.AngleAxis(angleLimit, Vector3.up) * (target - player.position).normalized :
                Quaternion.AngleAxis(-angleLimit, Vector3.up) * (target - player.position).normalized;

            var v_rot = Quaternion.LookRotation(sideVector, Vector3.up);
            var v_slerp = Quaternion.Slerp(transform.rotation, v_rot, Time.deltaTime * readjustmentSpeed);
            SetRotation(Quaternion.Euler(v_slerp.eulerAngles.x, slerp.eulerAngles.y, 0));
        }

        public void SetRotation(Quaternion newRotation)
        {
            currentRotation.x = newRotation.eulerAngles.x;
            if (currentRotation.x > 180)
            {
                currentRotation.x -= 360;
            }
            currentRotation.x = Mathf.Clamp(currentRotation.x, cameraAngleLimit.x, cameraAngleLimit.y);
            currentRotation.y = newRotation.eulerAngles.y;
            transform.rotation = Quaternion.Euler(currentRotation);
        }

        public void SetTarget(Transform t)
        {
            if(t.TryGetComponent<LockOnTarget>(out var target))
            {
                SetTarget(target);
            }
        }

        public void SetTarget(LockOnTarget target)
        {
            if (!target.enabled && !target.isActiveAndEnabled) return;

            this.target = target;
        }

        public void UnsetTarget()
        {
            this.target = null;
        }

        public void ReadjustRotation()
        {
            x_damp = y_damp = 0;
            IEnumerator OnReadjust()
            {
                readjusting = true;
                var forwardRot = Quaternion.LookRotation(player.forward, Vector3.up) * Quaternion.Euler(new Vector3(20, 0, 0));
                while(Quaternion.Angle(transform.rotation, forwardRot) > 0.1f)
                {
                    if (!readjusting) yield break;

                    var slerp = Quaternion.RotateTowards(transform.rotation, forwardRot, readjustmentAngleSpeed * Time.deltaTime);
                    SetRotation(slerp);
                    yield return null;
                }
                readjusting = false;
            }
            
            StopAllCoroutines();
            StartCoroutine(OnReadjust());
        }
    
        public void SetCameraDistance(float value)
        {
            IEnumerator OnSet()
            {
                float initialValue = cameraDistance;
                float c = 0;
                while(c < 1f)
                {
                    c += Time.deltaTime;
                    float t = c / 1;
                    if(t > 1)
                    {
                        t = 1;
                    }
                    t = -(Mathf.Cos(Mathf.PI * t) - 1) / 2;
                    cameraDistance = Mathf.Lerp(initialValue, value, t);
                    yield return null;
                }
            }
            StopAllCoroutines();
            StartCoroutine(OnSet());
        }
    }
}
