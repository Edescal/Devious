using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Edescal
{
    [RequireComponent(typeof(CameraLockOn))]
    public class CameraControls : MonoBehaviour
    {
        public Vector2 CameraRotation { get; private set; }
        public float ZoomValue { get; private set; }

        [Header("Input System (Input Actions)")]
        [SerializeField]
        private InputActionReference rotation;
        [SerializeField]
        private InputActionReference zoom;
        [SerializeField]
        private InputActionReference lockOn;

        [Header("Legacy Input System (Mouse control)")]
        [SerializeField, Range(0, 2)]
        private float mouseSensitivity = 1;
        
        private CameraLockOn cameraLockOn;

        public void Enable()
        {
            rotation?.action.Enable();
            zoom?.action.Enable();
            lockOn?.action.Enable();
        }

        public void Disable()
        {
            rotation?.action.Disable();
            zoom?.action.Disable();
            lockOn?.action.Disable();
        }

        private void Update()
        {
            if(rotation == null)
            {
                CameraRotation = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * mouseSensitivity;
            }

            if(zoom == null)
            {
                ZoomValue = Input.GetAxis("Mouse ScrollWheel");
            }

            if (lockOn == null)
            {
                if (Input.GetMouseButtonDown(2))
                {
                    cameraLockOn?.TryToTarget();
                }
            }
        }

        private void OnEnable()
        {
            if(rotation != null)
            {
                rotation.action.Enable();
                rotation.action.performed += OnCameraRotate;
            }

            if(zoom != null)
            {
                zoom.action.Enable();
                zoom.action.performed += OnZoom;
            }

            if (lockOn != null)
            {
                if (cameraLockOn == null)
                {
                    cameraLockOn = GetComponent<CameraLockOn>();
                }
                lockOn.action.Enable();
                lockOn.action.performed += OnLockOn;
            }
        }

        private void OnDisable()
        {
            if (rotation != null)
            {
                rotation.action.Disable();
                rotation.action.performed -= OnCameraRotate;
            }

            if (zoom != null)
            {
                zoom.action.Disable();
                zoom.action.performed -= OnZoom;
            }

            if (lockOn != null)
            {
                lockOn.action.Disable();
                lockOn.action.performed -= OnLockOn;
            }
        }

        private void OnCameraRotate(InputAction.CallbackContext ctx)
        {
            CameraRotation = ctx.ReadValue<Vector2>();
        }

        private void OnLockOn(InputAction.CallbackContext ctx)
        {
            cameraLockOn?.TryToTarget();
        }

        private void OnZoom(InputAction.CallbackContext ctx)
        {
            ZoomValue = ctx.ReadValue<float>() * mouseSensitivity;;
        }
    }
}
