using System.Collections;
using UnityEngine;

namespace Edescal
{
    public class FocusToDialogue : MonoBehaviour
    {
        public ThirdPersonCamera thirdPersonCamera;
        public CameraLockOn cameraLockOn;
        public CameraInput cameraControls;
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
                print("Cant focus to dialogue");
            }
        }


        public void Focus(LockOnTarget target)
        {
            cameraLockOn.CancelTargeting();
            thirdPersonCamera.SetTarget(target);
            thirdPersonCamera.SetCameraDistance(distance);
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