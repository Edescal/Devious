using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Edescal;

public class OverrideMainCamera : MonoBehaviour
{
    [SerializeField]
    private Camera overrideCamera;
    [SerializeField]
    private float timeToSwapControlls = 0.5f;
    private WaitForSeconds timeSwap;

    private void Start()
    {
        timeSwap = new WaitForSeconds(timeToSwapControlls);
    }

    public void Override(ThirdPersonController controller)
    {
        if (controller == null) return;

        StopAllCoroutines();
        IEnumerator OnChange()
        {
            overrideCamera.enabled = true;
            yield return timeSwap;
            controller.OverrideCamera = overrideCamera.transform;
        }
        StartCoroutine(OnChange());
    }

    public void ReturnCamera(ThirdPersonController controller)
    {
        if (controller == null) return;

        StopAllCoroutines();
        IEnumerator OnChange()
        {
            overrideCamera.enabled = false;
            yield return timeSwap;
            controller.OverrideCamera = null;
        }
        StartCoroutine(OnChange());
    }
}
