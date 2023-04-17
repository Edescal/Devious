using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverrideMainCamera : MonoBehaviour
{
    public Camera overrideCamera;
    public float timeToSwapControlls = 0.5f;
    private ThirdPersonController controller;
    private WaitForSeconds timeSwap;

    private void Start()
    {
        timeSwap = new WaitForSeconds(timeToSwapControlls);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (controller == null)
        {
            controller = other.GetComponent<ThirdPersonController>();
        }

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

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

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
