using System.Collections;
using UnityEngine;

namespace Edescal
{
    public class FocusToDialogue : MonoBehaviour
    {
        public ThirdPersonCamera thirdPersonCamera;
        public CameraLockOn cameraLockOn;
        public CameraControls cameraControls;
        public InputReader input;
        public float distance = 1;

        private void Start()
        {
            if (thirdPersonCamera == null
                || cameraLockOn == null
                || cameraControls == null
                || input == null)
            {
                Debug.Break();
            }
        }


        public void Focus(LockOnTarget target)
        {
            thirdPersonCamera.SetTarget(target);
            thirdPersonCamera.SetCameraDistance(distance);
            cameraLockOn.CancelTargeting();
            cameraControls.Disable();
            input.SwitchUI(true);
        }

        public void Unfocus()
        {
            thirdPersonCamera.UnsetTarget();
            cameraControls.Enable();
            input.SwitchUI(false);
        }
    }
}