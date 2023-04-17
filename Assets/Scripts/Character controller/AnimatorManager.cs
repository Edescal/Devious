using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    private Animator animator;
    private RuntimeAnimatorController defaultController;
    private ThirdPersonController thirdPersonController;
    private bool rootMotion = false;
    private int rootMotionStateHash = 0;
    private float rootMotionTransition = 0;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        thirdPersonController = GetComponent<ThirdPersonController>();
        defaultController = animator.runtimeAnimatorController;
    }

    private void OnAnimatorMove()
    {
        if (!rootMotion) return;

        if (rootMotionTransition > 0)
        {
            rootMotionTransition -= Time.deltaTime;
            return;
        }

        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.shortNameHash != rootMotionStateHash)
        {
            animator.applyRootMotion = false;
            rootMotion = false;
            thirdPersonController.CanMove = true;
        }
        else
        {
            thirdPersonController.SetPosition = animator.rootPosition;
            thirdPersonController.SetRotation = animator.rootRotation;
        }
    }

    public void Play(string state, float transitionTime, int layer = 0, bool applyRootMotion = false)
    {
        animator.CrossFadeInFixedTime(state, transitionTime, layer);

        if (applyRootMotion)
        {
            animator.applyRootMotion = true;
            rootMotion = true;
            rootMotionTransition = transitionTime;
            rootMotionStateHash = Animator.StringToHash(state);
            thirdPersonController.CanMove = false;
        }
    }

    public void OverrideController(AnimatorOverrideController overrideController)
    {
        if (overrideController == null)
        {
            animator.runtimeAnimatorController = defaultController;
            return;
        }

        animator.runtimeAnimatorController = overrideController;
    }
}