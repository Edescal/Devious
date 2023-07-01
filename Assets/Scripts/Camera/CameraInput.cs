using UnityEngine;
using UnityEngine.InputSystem;

namespace Edescal
{
    public class CameraInput : MonoBehaviour, Controls.ICameraActions
    {
        public Vector2 Rotation { get; private set; }
        public float Zoom { get; private set; }
        public event System.Action onLock;

        [Header("Input binding"), SerializeField]
        private InputActionReference rotateInput;
        [SerializeField]
        private InputActionReference zoomInput;
        [SerializeField]
        private InputActionReference targetingInput;

        public void Enable()
        {
            rotateInput?.action.Enable();
            zoomInput?.action.Enable();
            targetingInput?.action.Enable();
        }

        public void Disable()
        {
            rotateInput?.action.Disable();
            zoomInput?.action.Disable();
            targetingInput?.action.Disable();
        }

        private void OnEnable()
        {
            if (rotateInput != null)
            {
                rotateInput.action.Enable();
                rotateInput.action.performed += OnRotate;
            }
            if (zoomInput != null)
            {
                zoomInput.action.Enable();
                zoomInput.action.performed += OnZoom;
            }

            if (targetingInput != null)
            {
                targetingInput?.action.Enable();
                targetingInput.action.performed += OnLockOn;
            }
        }

        private void OnDisable()
        {
            if (rotateInput != null)
            {
                rotateInput.action.Disable();
                rotateInput.action.performed -= OnRotate;
            }
            if (zoomInput != null)
            {
                zoomInput.action.Disable();
                zoomInput.action.performed -= OnZoom;
            }

            if (targetingInput != null)
            {
                targetingInput?.action.Disable();
                targetingInput.action.performed -= OnLockOn;
            }
        }

        public void OnRotate(InputAction.CallbackContext context) => Rotation = context.ReadValue<Vector2>();

        public void OnLockOn(InputAction.CallbackContext context) => onLock?.Invoke();

        public void OnZoom(InputAction.CallbackContext context) => Zoom = context.ReadValue<float>();
    }
}
