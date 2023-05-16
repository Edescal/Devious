using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Edescal;

public class GameManager : MonoBehaviour
{
    public FadeScreen fadeScreen;
    public Player player;

    public void WarpPlayer(Transform point)
    {
        IEnumerator OnWarp()
        {
            Time.timeScale = 0;
            
            fadeScreen.Show();
            while (fadeScreen.fading)
            {
                yield return null;
            }

            Time.timeScale = 1;

            player.ThirdPersonController.CanMove = false;
            player.ThirdPersonController.SetPosition = point.position;
            player.ThirdPersonController.CanMove = true;
            player?.ThirdPersonCamera?.ResetPosition();
            player?.AnimatorManager.Animator.SetBool("falling", false);
            player?.AnimatorManager.Animator.SetTrigger("default");

            fadeScreen.Hide();
            while (fadeScreen.fading)
            {
                yield return null;
            }
        }

        StartCoroutine(OnWarp());
    }

    public void FocusOnTarget(LockOnTarget target)
    {
        player.CameraLockOn.CancelTargeting();
        player.ThirdPersonCamera.SetTarget(target);
        player.ThirdPersonController.SetTarget(target);
        player.Interactor.CanInteract = false;
        player.CameraControls.Disable();
    }

    public void Unfocus()
    {
        player.ThirdPersonCamera.UnsetTarget();
        player.ThirdPersonController.UnsetTarget();
        player.CameraControls.Enable();
        player.Interactor.CanInteract = true;
    }
}
