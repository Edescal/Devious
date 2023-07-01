using UnityEngine;
using Edescal;

public class AnimatorManager : MonoBehaviour
{
    private Player player;
    private Animator animator;
    private RuntimeAnimatorController defaultController;
    private bool rootMotion = false;
    private int rootMotionStateHash = 0;
    private float rootMotionTransition = 0;

    private void Awake()
    {
        player = GetComponent<Player>();
        animator = GetComponent<Animator>();
        defaultController = animator.runtimeAnimatorController;
    }

    private void OnAnimatorMove()
    {
        if (player == null)
        {
            transform.position = animator.rootPosition;
            transform.rotation = animator.rootRotation;
            return;
        }

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
            player.ThirdPersonController.CanMove = true;
            player.Interactor.CanInteract = true;
        }
        else
        {
            player.ThirdPersonController.Position = animator.rootPosition;
            player.ThirdPersonController.Rotation = animator.rootRotation;
        }
    }

    public Animator Animator => animator;

    public void Play(string state) => Play(state, 0.25f); // For debug animations
    public void Play(string state, float transitionTime, int layer = 0, bool applyRootMotion = false)
    {
        animator.CrossFadeInFixedTime(state, transitionTime, layer);

        if (applyRootMotion)
        {
            rootMotion = true;
            animator.applyRootMotion = true;
            rootMotionTransition = transitionTime;
            rootMotionStateHash = Animator.StringToHash(state);
            player.ThirdPersonController.CanMove = false;
            player.Interactor.CanInteract = false;
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