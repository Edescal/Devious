using System;
using System.Collections;
using UnityEngine;
using Edescal;

public class AnimatorManager : MonoBehaviour
{
    public Animator Animator => animator;

    private Animator animator;
    private RuntimeAnimatorController defaultController;
    private bool trackAnimation = false;
    private bool rootMotion = false;
    private int rootMotionStateHash = 0;
    private float rootMotionTransition = 0;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        defaultController = animator.runtimeAnimatorController;
    }

    private void OnAnimatorMove()
    {
        if (!trackAnimation) return;

        if (rootMotionTransition > 0)
        {
            rootMotionTransition -= Time.deltaTime;
            return;
        }

        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.shortNameHash != rootMotionStateHash || stateInfo.normalizedTime >= 1)
        {
            trackAnimation = false;
            if (rootMotion)
            {
                animator.applyRootMotion = false;
                rootMotion = false;
            }
        }
        else if (rootMotion)
        {
            transform.position = animator.rootPosition;
            transform.rotation = animator.rootRotation;
        }
    }

    public void Play(string state, float transitionTime, int layer = 0, bool applyRootMotion = true, Action onStart = null, Action onEnd = null)
    {
        animator.CrossFadeInFixedTime(state, transitionTime, layer);

        if (applyRootMotion && (onStart != null || onEnd != null))
        {
            trackAnimation = true;
            rootMotionTransition = transitionTime;
            rootMotionStateHash = Animator.StringToHash(state);
        }

        if (applyRootMotion)
        {
            rootMotion = true;
            animator.applyRootMotion = true;
        }

        if (onStart != null || onEnd != null)
        {
            IEnumerator Promise()
            {
                onStart?.Invoke();
                while (animator.applyRootMotion) yield return null;
                onEnd?.Invoke();
            }
            StartCoroutine(Promise());
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
