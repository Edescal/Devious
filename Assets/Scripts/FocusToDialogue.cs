using System.Collections;
using UnityEngine;

namespace Edescal
{
    public class FocusToDialogue : MonoBehaviour
    {
        public Transform player;
        public ThirdPersonCamera thirdPersonCamera;
        public CameraLockOn cameraLockOn;
        public CameraInput cameraControls;
        public InputReader input;
        public float distance = 1;
        public float rotateDuration = 2f;

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
            if (player != null)
            {
                var dir = Vector3.ProjectOnPlane(target.transform.position - player.position, Vector3.up);
                var rot = Quaternion.LookRotation(dir, Vector3.up);
                var ogRot = player.rotation;
                LeanTween.value(0, 1, rotateDuration)
                    .setEase(LeanTweenType.easeInOutSine)
                    .setOnUpdate((float f) =>
                    {
                        player.rotation = Quaternion.Lerp(ogRot, rot, f);
                    });
            }
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